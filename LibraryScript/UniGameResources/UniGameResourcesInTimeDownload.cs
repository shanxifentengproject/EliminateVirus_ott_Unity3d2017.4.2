using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Resources;
using UnityEngine;
using FTLibrary.XML;
using FTLibrary.Text;
partial class UniGameResources : GameResources
{
    ////当前已经载入本地的资源包表
    //private ResourcesInTimeDownloadTaskQueue downloadTaskQueue = null;
    //public void InitializationInTimeDownload()
    //{ 
    //    GameObject go = new GameObject("ResourcesPackageInTimeChecker", typeof(ResourcesInTimeDownloadTaskQueue));
    //    go.transform.position = Vector3.zero;
    //    go.transform.rotation = Quaternion.identity;
    //    UnityEngine.Object.DontDestroyOnLoad(go);
    //    downloadTaskQueue = go.GetComponent<ResourcesInTimeDownloadTaskQueue>();
    //}
    //public static string InTime_ResourcesPathToPackageNetPath(ref GameResourcesNode data)
    //{
    //    //将后缀名替换为unity3d
    //    //从后往前超找点
    //    string resourcesPath = data.path;
    //    int index;
    //    for (index = resourcesPath.Length - 1; index > 0;index-- )
    //    {
    //        if (resourcesPath[index] == '.')
    //            break;
    //    }
    //    if (index == 0)
    //        throw new Exception("resources path err!");
    //    return UniGameResources.ConnectPath(UniGameResources.downloadUrlPath, data.packageName, "\\", resourcesPath.Substring(0, index + 1), UniGameResourcesDefine.InTimePackageFileFilter);
    //}
    ////立即下载载入模式
    //public UnityEngine.Object InTime_LoadResource_Immediately(ref GameResourcesNode data, Type type, out AssetBundle assetBundle)
    //{
    //    //根据给定的资源直接打开
    //    //如果资源没有下载就在循环里直接下载一直到下载完成
    //    string resourcesPackageNetPath = InTime_ResourcesPathToPackageNetPath(ref data);
    //    WWW www=null;
    //    if (data.version == -1)
    //    {
    //        www = new WWW(resourcesPackageNetPath);
    //    }
    //    else
    //    {
    //        www = WWW.LoadFromCacheOrDownload(resourcesPackageNetPath, data.version);
    //    }
    //    //这里如果必须等待包下载完成
    //    //如果是异步加载模式的情况下到这里其实包已经下载完成了
    //    //如果是同步下载模式这里需要循环直到包下载完成或者出现错误
    //    do
    //    {
    //        if (www.error != null)
    //            throw new Exception(string.Format("load resources package err,err:%s", www.error));
    //        if (www.isDone)
    //            break;
    //    } while (true);
    //    assetBundle = www.assetBundle;
    //    www.Dispose();
    //    return assetBundle.Load(data.path, type);
    //}
    //public void InTime_LoadResource_ImmediatelyRelease(AssetBundle assetBundle)
    //{
    //    assetBundle.Unload(false);
    //    UnityEngine.Object.DestroyObject(assetBundle);
    //}
    ////异步载入资源的模式
    ///*
    // * 1.检测本地是否已经下载了这个文件，如果已经下载了对比版本，版本没有超出则直接加载本地的
    // * 2,如果本地没有或者。版本要求更新的就需要重新下载了
    // * 3,下载的时候检查是否已经存在任务在下载队列中了，如果存在就等待，否则就添加
    // * 
    // * */
    //public void InTime_LoadResource_Delay(int downloadLevel, ref GameResourcesNode data, Type type, IPackageInTimeDownload waitobj, bool isInstantiate)
    //{
    //    int version;
    //    if (downloadTaskQueue.downloadLoaclResourcesList.TryGetValue(data.id, out version))
    //    {
    //        if (data.version <= version)
    //        {
    //            AssetBundle assetBundle;
    //            UnityEngine.Object ret = InTime_LoadResource_Immediately(ref data, type, out assetBundle);
    //            if (isInstantiate)
    //            {
    //                waitobj.OnPackageInTimeDownloaded(data.id, GameObject.Instantiate(ret), true);
    //            }
    //            else
    //            {
    //                waitobj.OnPackageInTimeDownloaded(data.id, ret, true);
    //            }
    //            InTime_LoadResource_ImmediatelyRelease(assetBundle);
    //        }
    //        else
    //        {
    //            downloadTaskQueue.downloadLoaclResourcesList.Remove(data.id);
    //        }
    //    }
    //    downloadTaskQueue.AddDownloadTask(downloadLevel, ref data, type, waitobj, isInstantiate);
    //}

}
