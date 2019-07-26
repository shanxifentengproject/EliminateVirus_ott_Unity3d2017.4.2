//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using FTLibrary.Resources;
//using FTLibrary.Text;

//class ResourcesInTimeDownloadTask
//{
//    //关联的资源定义
//    private GameResourcesNode resourcesData;
//    //下载级别，数值越高，越往前排
//    public int downloadLevel = 0;
//    //资源ID
//    public uint resourcesId { get { return resourcesData.id; } }
//    public string resourcesName { get { return resourcesData.name; } }
//    public int resourcesVersion { get { return resourcesData.version; } }
//    //资源类型
//    private Type resourcesType;
//    //是否拷贝
//    private bool isInstantiate;
//    public ResourcesInTimeDownloadTask(int downloadlevel, ref GameResourcesNode data, Type type, bool isinstantiate)
//    {
//        resourcesData = data;
//        downloadLevel = downloadlevel;
//        resourcesType = type;
//        isInstantiate = isinstantiate;
//    }
    
//    private List<IPackageInTimeDownload> m_WaitList = null;
//    public List<IPackageInTimeDownload> waitList
//    {
//        get
//        {
//            if (m_WaitList != null)
//                return m_WaitList;
//            m_WaitList = new List<IPackageInTimeDownload>(4);
//            return m_WaitList;
//        }
//    }


//    //是否正在下载
//    public bool IsDownloading { get; set; }
//    //是否下载成功了
//    public bool IsDownloadsucceed { get; set; }
    
//    private WWW downloadWWW = null;
//    public void InTimeStartDownload()
//    {
//        IsDownloading = true;
//        downloadWWW = WWW.LoadFromCacheOrDownload(UniGameResources.InTime_ResourcesPathToPackageNetPath(ref resourcesData), resourcesVersion);
//    }
//    public void InTimeReleaseDownload()
//    {
//        if (downloadWWW != null)
//        {
//            downloadWWW.Dispose();
//            downloadWWW = null;
//        }
//    }
//    public bool isDone { get { return downloadWWW == null ? true : downloadWWW.isDone; } }
//    public string error { get { return downloadWWW == null ? null : downloadWWW.error; } }
//    //下载完成
//    public void InTimeDownloadComplete()
//    {
//        //通知所有等待的对象
//        if (m_WaitList != null && m_WaitList.Count != 0)
//        {
//            if (IsDownloadsucceed && downloadWWW != null)
//            {
//                AssetBundle assetBundle = downloadWWW.assetBundle;
//                UnityEngine.Object ret = assetBundle.Load(resourcesData.path, resourcesType);
//                for (int i = 0; i < m_WaitList.Count; i++)
//                {
//                    if (isInstantiate)
//                    {
//                        m_WaitList[i].OnPackageInTimeDownloaded(resourcesId, GameObject.Instantiate(ret), true);
//                    }
//                    else
//                    {
//                        m_WaitList[i].OnPackageInTimeDownloaded(resourcesId, ret, true);
//                    }
//                }
//                assetBundle.Unload(false);
//                UnityEngine.Object.DestroyObject(assetBundle);
//            }
//            else
//            {
//                for (int i = 0; i < m_WaitList.Count; i++)
//                {
//                    m_WaitList[i].OnPackageInTimeDownloaded(resourcesId, null, false);
//                }
//            }
//            m_WaitList.Clear();
//            m_WaitList = null;
//        }
//    }
//}

