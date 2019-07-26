using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Text;
using UnityEngine;
using System.Collections;
//用来下载资源包的组件
//这个组件使用协同程序下载指定的资源包列表
//并且打开，同时对资源包管理对象增加引用
//当这个对象被删除后则释放对资源包管理对象的引用
[AddComponentMenu("UniGameResources/UniGameResourceLoad/UniGameResourcesDownLoader")]
class UniGameResourcesDownLoader : MonoBehaviourIgnoreGui
{
    //准备下载的资源包
    private List<UniGameResourcesPackage> needDownloadPackageList = new List<UniGameResourcesPackage>(8);
    //已经下载完成的资源包
    private List<UniGameResourcesPackage> overDownloadPackageList = new List<UniGameResourcesPackage>(8);
    //正在进行下载的组件
    private WWW downloadNowWWW = null;
    //获取当前完成进度
    public float Progress
    {
        get
        {
            int totalCount = (needDownloadPackageList.Count + overDownloadPackageList.Count);
            if (totalCount == 0)
                return 1.0f;
            float total = totalCount * 1.0f;
            float over = overDownloadPackageList.Count * 1.0f;
            if (downloadNowWWW == null)
            {
                return over / total;
            }
            else
            {
                return (over + downloadNowWWW.progress) / total;
            }
        }
    }
    public IEnumerator DownloadPackage(params string[] part)
    {
        List<string> list = new List<string>(8);
        for (int i = 0; i < part.Length;i++ )
        {
            list.Add(part[i]);
        }
        yield return StartCoroutine(DownloadPackageWithList(list));
    }
    public IEnumerator DownloadPackageWithList(List<string> list)
    {
        if (needDownloadPackageList.Count != 0 || overDownloadPackageList.Count != 0)
        {
            Debug.LogError("Is Have Loader Package!");
            yield break;
        }
        if (list.Count == 0)
        {
            Debug.LogError("Download What?!");
            yield break;
        }
        List<UniGameResourcesPackage> packageList = new List<UniGameResourcesPackage>(8);
        for (int i = 0; i < list.Count; i++)
        {
            UniGameResourcesPackage package = UniGameResources.FindSystemResourcesPackageTable(list[i]);
            if (package == null)
            {
                Debug.LogError(string.Format("not find package:{0}", list[i]));
                yield break;
            }
            packageList.Add(package);
        }
        yield return StartCoroutine(DownloadPackageWithList(packageList));
    }
    public IEnumerator DownloadPackageWithList(List<UniGameResourcesPackage> packageList)
    {
        if (needDownloadPackageList.Count != 0 || overDownloadPackageList.Count != 0)
        {
            Debug.LogError("Is Have Loader Package!");
            yield break;
        }
        for (int i = 0; i < packageList.Count; i++)
        {
            UniGameResourcesPackage package = packageList[i];
            package.LockPackage();
            needDownloadPackageList.Add(package);
        }
        //开始下载过程
        do 
        {
            UniGameResourcesPackage package = needDownloadPackageList[0];
            //检测是否已经存在资源对象了
            //存在就不要下载了
            if (package.currentAssetBundle == null)
            {
                if (package.assetBundleType == UniGameResourcesPackage.AssetBundleType.Type_Local)
                {
                    if (UniGameResources.ForceUpdateAllGameAssetBundle)
                    {
                        downloadNowWWW = WWW.LoadFromCacheOrDownload(UniGameResources.ConnectPath(UniGameResources.StreamingAssetsPath, package.packagePath),
                                UniGameResources.version_GameAssetBundleInventoryPackageFile);
                    }
                    else
                    {
                        downloadNowWWW = WWW.LoadFromCacheOrDownload(UniGameResources.ConnectPath(UniGameResources.StreamingAssetsPath, package.packagePath),
                                package.assetBundleVersion);
                    }
                    
                }
                else if (package.assetBundleType == UniGameResourcesPackage.AssetBundleType.Type_Net)
                {
                    if (UniGameResources.ForceUpdateAllGameAssetBundle)
                    {
                        downloadNowWWW = WWW.LoadFromCacheOrDownload(UniGameResources.ConnectPath(UniGameResources.downloadUrlPath, package.packagePath),
                                UniGameResources.version_GameAssetBundleInventoryPackageFile);
                    }
                    else
                    {
                        downloadNowWWW = WWW.LoadFromCacheOrDownload(UniGameResources.ConnectPath(UniGameResources.downloadUrlPath, package.packagePath),
                                package.assetBundleVersion);
                    }
                    
                }
                else
                {
                    Debug.LogError("assetBundleType Err!");
                    yield break;
                }
                //等待完成
                yield return downloadNowWWW;
                if (downloadNowWWW.error != null)
                {
                    Debug.LogError(string.Format("download package err:{0}", downloadNowWWW.error));
                    downloadNowWWW.Dispose();
                    downloadNowWWW = null;
                    yield break;
                }
                else if (!downloadNowWWW.isDone)
                {
                    Debug.LogError("download package can not download over!");
                    downloadNowWWW.Dispose();
                    downloadNowWWW = null;
                    yield break;
                }
                package.currentAssetBundle = downloadNowWWW.assetBundle;
                downloadNowWWW.Dispose();
                downloadNowWWW = null;

            }
            overDownloadPackageList.Add(package);
            needDownloadPackageList.RemoveAt(0);
            
        } while (needDownloadPackageList.Count != 0);
    }
    protected virtual void OnDestroy()
    {
        //解锁所有已经锁定的包
        for (int i = 0; i < needDownloadPackageList.Count;i++ )
        {
            needDownloadPackageList[i].UnLockPackage();
        }
        needDownloadPackageList.Clear();
        for (int i = 0; i < overDownloadPackageList.Count;i++ )
        {
            overDownloadPackageList[i].UnLockPackage();
        }
        overDownloadPackageList.Clear();
    }
}


//public enum ResourcesLoaderErr
//{
//    Err_DownloadPackageListFileErr,     //下载资源清单文件失败
//    Err_DownloadPackageFileErr,         //下载资源文件失败
//}
//public class UniGameResourcesDownLoader : MonoBehaviourIgnoreGui
//{
//    //当前使用的资源管理对象
//    internal UniGameResources checkUniGameResources = null;
//    //需要下载的队列
//    private List<FTLibrary.Resources.GameResourcesPackage> m_DownloadPackageList = null;
//    //需要下载的总文件尺寸
//    protected long downloadTotalFileSize = 0;
//    protected List<FTLibrary.Resources.GameResourcesPackage> downloadPackageList
//    {
//        get { return m_DownloadPackageList; }
//        set
//        {
//            m_DownloadPackageList = value;
//            if (m_DownloadPackageList != null)
//            {
//                m_DownloadOverPackageList = new List<FTLibrary.Resources.GameResourcesPackage>(m_DownloadPackageList.Count);
//                downloadTotalFileSize = FTLibrary.Resources.GameResourcesPackage.AccountRealSize(m_DownloadPackageList);
//                downloadOverFileSize = 0;
//            }
//            else
//            {
//                downloadTotalFileSize = 0;
//                downloadOverFileSize = 0;
//            }
            
//        }
//    }
//    //已经下载完毕的队列
//    private List<FTLibrary.Resources.GameResourcesPackage> m_DownloadOverPackageList = null;
//    //已经下载完毕的文件尺寸
//    protected long downloadOverFileSize = 0;
//    private void AddDownloadOverPackage(FTLibrary.Resources.GameResourcesPackage package)
//    {
//        m_DownloadOverPackageList.Add(package);
//        //计算已下载尺寸
//        downloadOverFileSize = FTLibrary.Resources.GameResourcesPackage.AccountLocalSize(m_DownloadOverPackageList);
//    }
//    //当前正在下载的资源包
//    private FTLibrary.Resources.GameResourcesPackage downloadNowPackage = null;
//    //当前使用的WWW对象
//    private WWW downloadNowWWW = null;
//    //计算已经下载的文件尺寸
//    protected long DownloadedFileSize
//    {
//        get
//        {
//            long ret = downloadOverFileSize;
//            if (downloadNowPackage != null && downloadNowWWW != null)
//            {
//                ret += (long)(((double)downloadNowPackage.packageRealSize) * downloadNowWWW.progress);
//            }
//            return ret;
//        }
//    }
//    protected float DownloadedProgress
//    {
//        get
//        {
//            if (downloadTotalFileSize == 0)
//                return 1.0f;
//            return (float)(((double)DownloadedFileSize) / ((double)downloadTotalFileSize));
//        }
//    }
//    //是否下载完成
//    protected bool IsDownloadedOver
//    {
//        get
//        {
//            if (downloadPackageList == null)
//                return true;
//            return downloadPackageList.Count == 0;
//        }
//    }
//    protected IEnumerator DownloadPackageProc()
//    {
//        do 
//        {
//            downloadNowPackage = downloadPackageList[0];
//            downloadNowWWW = WWW.LoadFromCacheOrDownload(UniGameResources.ConnectPath(UniGameResources.downloadUrlPath, downloadNowPackage.packagePath),
//                            downloadNowPackage.packageRealVersion);
//            yield return downloadNowWWW;
//            if (downloadNowWWW.error != null)
//            {
//                OnDownloadErr(ResourcesLoaderErr.Err_DownloadPackageFileErr, downloadNowWWW.error);
//                downloadNowWWW.Dispose();
//                downloadNowPackage = null;
//                yield break;
//            }
//            downloadNowWWW.Dispose();
//            downloadNowWWW = null;
//            //从下载队列删除
//            downloadPackageList.RemoveAt(0);
//            //加入到完成队列
//            AddDownloadOverPackage(downloadNowPackage);
//            //资源包刷新版本
//            downloadNowPackage.UpdateNewVersion();
//            //保存一次本地缓冲文件，这个过程很重要，如果发生下载中断了
//            //则下次下载这个包会被认为最新的
//            checkUniGameResources.SaveLocalPackageFile();
//            downloadNowPackage = null;

//        } while (!IsDownloadedOver);
//    }
//    //错误处理
//    protected virtual void OnDownloadErr(ResourcesLoaderErr errcode, string err)
//    {

//    }
//    protected virtual void Release()
//    {
//        //这里卸载所有资源
//        if (m_DownloadPackageList != null)
//        {
//            m_DownloadPackageList.Clear();
//            m_DownloadPackageList = null;
//        }
//        if (m_DownloadOverPackageList != null)
//        {
//            m_DownloadOverPackageList.Clear();
//            m_DownloadOverPackageList = null;
//        }
//        downloadNowPackage = null;
//        if (downloadNowWWW != null)
//        {
//            downloadNowWWW.Dispose();
//            downloadNowWWW = null;
//        }
//    }
//    protected virtual void OnDestroy()
//    {
//        Release();
//    }
//    //是否开始工作了
//    protected virtual bool IsDownloading { get { return false; } }
//    //开始下载
//    protected virtual void OnStartDownload()
//    {

//    }
//    //资源包下载检测完成
//    protected virtual void OnDownloadOver()
//    {
//        Release();
//    }
    
//}
