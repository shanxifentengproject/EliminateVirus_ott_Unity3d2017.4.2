//using System;
//using System.Collections.Generic;
//using System.Text;
//using FTLibrary.Text;
//using UnityEngine;
//using System.Collections;
//public class UniGameStartResourcesDownLoader : UniGameResourcesDownLoader
//{
//    //是否开始工作了
//    protected bool m_IsDownloading = false;
//    protected override bool IsDownloading { get { return m_IsDownloading; } }
//    protected IEnumerator CheckNetResourcesPackage()
//    {
//        //下载资源包清单
//        WWW www = new WWW(UniGameResources.ConnectPath(UniGameResources.downloadUrlPath, UniGameResourcesDefine.NetPackageFileName));
//        yield return www;
//        if (www.error != null)
//        {
//            OnDownloadErr(ResourcesLoaderErr.Err_DownloadPackageListFileErr, www.error);
//            www.Dispose();
//            yield break;
//        }
//        //将这个文件缓冲到本地
//        UniGameResources.WriteSafeFile(UniGameResources.ConnectPath(UniGameResources.PersistentDataPath, UniGameResourcesDefine.BufferNetPackageFileName), www.bytes);
//        //释放托管资源
//        www.Dispose();
//        //标记为本地已经存在缓冲文件了
//        UniGameResources.isBufferNetPackageFile = true;
//        //重建资源包表
//        checkUniGameResources = new UniGameResources();
//        checkUniGameResources.BuildResourcesPackageTable();
//        //检测是否存在必须更新的资源
//#if _Develop
//            downloadPackageList = null;
//#endif //_Develop
//#if _Release
//            downloadPackageList = checkUniGameResources.FilterPackage_ConstraintNew();
//#endif//_Release
//        if (!IsDownloadedOver)
//        {
//            m_IsDownloading = true;
//            OnStartDownload();
//            yield return StartCoroutine(DownloadPackageProc());
//        }
//        else
//        {
//            //也需要保存一次本地文件
//            checkUniGameResources.SaveLocalPackageFile();
//        }
//        //不需要下载，调用结束函数
//        Invoke("OnDownloadOver", 0.1f);
//    }
//    protected override void Release()
//    {
//        base.Release();
//        if (checkUniGameResources != null)
//        {
//            checkUniGameResources.Release();
//            checkUniGameResources = null;
//            UniGameResources.currentUniGameResources = null;
//        }
//    }
//}
