//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using System.Collections;
//using FTLibrary.Resources;
//public class ResourcesInTimeDownloadTaskQueue : MonoBehaviourIgnoreGui
//{
//    //下载表
//    private Dictionary<uint, ResourcesInTimeDownloadTask> taskList = new Dictionary<uint, ResourcesInTimeDownloadTask>(128);
//    //下载队列
//    private List<ResourcesInTimeDownloadTask> waitDownloadList = new List<ResourcesInTimeDownloadTask>(128);
//    //正在下载的队列
//    private List<ResourcesInTimeDownloadTask> nowDownloadList = new List<ResourcesInTimeDownloadTask>(UniGameResourcesDefine.InTimeDownloadMaxTask);
//    //已经下载到本地的资源清单
//    public Dictionary<uint, int> downloadLoaclResourcesList = new Dictionary<uint, int>(512);


//    //加入下载队列
//    private void AddTask(ResourcesInTimeDownloadTask Task)
//    {

//        bool bIsAdd = false;
//        for (int i = 0; i < waitDownloadList.Count; i++)
//        {
//            if (Task.downloadLevel > waitDownloadList[i].downloadLevel)
//            {
//                waitDownloadList.Insert(i, Task);
//                bIsAdd = true;
//                break;
//            }
//        }
//        if (!bIsAdd)
//        {
//            waitDownloadList.Add(Task);
//        }
//    }
//    //重新排列一次下载队列
//    private void MoveTask(ResourcesInTimeDownloadTask Task)
//    {
//        for (int i = 0; i < waitDownloadList.Count;i++ )
//        {
//            if (waitDownloadList[i].resourcesId == Task.resourcesId)
//            {
//                waitDownloadList.RemoveAt(i);
//                break;
//            }
//        }
//        AddTask(Task);
//    }
//    //加入一个下载任务
//    internal void AddDownloadTask(int downloadLevel, ref GameResourcesNode data, Type type, IPackageInTimeDownload waitobj, bool isInstantiate)
//    {
//        //检测是否存在这个任务
//        ResourcesInTimeDownloadTask Task;
//        if (taskList.TryGetValue(data.id,out Task))
//        {
//            Task.waitList.Add(waitobj);
//            //检测是否正在下载，如果不是需要移动一次
//            if (Task.IsDownloading)
//                return;
//            if (Task.downloadLevel >= downloadLevel)
//            {
//                Task.downloadLevel += UniGameResourcesDefine.InTimeAddTiveDownloadLevel;
//            }
//            else
//            {
//                Task.downloadLevel = downloadLevel;
//            }
//            MoveTask(Task);
//            return;
//        }
//        //不存在需要新添加一个下载任务
//        Task = new ResourcesInTimeDownloadTask(downloadLevel, ref data, type, isInstantiate);
//        taskList.Add(Task.resourcesId, Task);
//        AddTask(Task);
//        Task.waitList.Add(waitobj);
//    }
//    protected void Update()
//    {
//        //当前队列空了
//        if (nowDownloadList.Count == 0)
//        {
//            if (waitDownloadList.Count == 0)//没有需要下载的
//            {
//                return;
//            }
//            //填充当前下载队列
//            while (nowDownloadList.Count < UniGameResourcesDefine.InTimeDownloadMaxTask && waitDownloadList.Count != 0)
//            {
//                ResourcesInTimeDownloadTask Task = waitDownloadList[0];
//                waitDownloadList.RemoveAt(0);
//                Task.InTimeStartDownload();
//                nowDownloadList.Add(Task);
//            }
//        }
//        else
//        {
//            //扫描看那些完成了
//            for (int i=0;i<nowDownloadList.Count;i++)
//            {
//                if (nowDownloadList[i].error != null ||
//                     nowDownloadList[i].isDone)
//                {
//                    ResourcesInTimeDownloadTask Task = nowDownloadList[i];
//                    nowDownloadList.RemoveAt(i);
//                    i-=1;
//                    taskList.Remove(Task.resourcesId);

//                    if (Task.error != null)
//                    {
//                        Debug.LogError(string.Format("download resources:{0} err:{1}!", Task.resourcesName, Task.error));
//                        Task.IsDownloadsucceed = false;
//                    }
//                    else
//                    {
//                        Task.IsDownloadsucceed = true;
//                        //如果下载成功了就加入本地清单
//                        downloadLoaclResourcesList.Add(Task.resourcesId, Task.resourcesVersion);
//                    }
//                    //通知下载完成
//                    Task.InTimeDownloadComplete();
//                    //结束下载
//                    Task.InTimeReleaseDownload();
                    
//                }
//            }
//            //填充当前下载队列
//            while (nowDownloadList.Count < UniGameResourcesDefine.InTimeDownloadMaxTask && waitDownloadList.Count != 0)
//            {
//                ResourcesInTimeDownloadTask Task = waitDownloadList[0];
//                waitDownloadList.RemoveAt(0);
//                Task.InTimeStartDownload();
//                nowDownloadList.Add(Task);
//            }
//        }
        
//    }

    
//}
