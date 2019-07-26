using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
//这个类用来做资源卸载使用
//一般在切换场景的时候使用
//需要在协同程序里工作，调用方法为
//yield return StartCoroutine(obj.UnloadUnusedAssets());
//下面执行切换场景等操作
class UniGameResourcesReleaseUnusedAssets:MonoBehaviourIgnoreGui
{
    protected AsyncOperation releaseAsyncOperation = null;
    public bool isDone { get { return releaseAsyncOperation == null ? false : releaseAsyncOperation.isDone; } }
    public float progress { get { return releaseAsyncOperation == null ? 0.0f : releaseAsyncOperation.progress; } }
    protected IEnumerator UnloadUnusedAssets()
    {
        releaseAsyncOperation = Resources.UnloadUnusedAssets();
        yield return releaseAsyncOperation;
        GC.Collect();
    }
    public void CallUnloadUnusedAssets()
    {
        if (releaseAsyncOperation != null && !releaseAsyncOperation.isDone)
            return;
        StartCoroutine(UnloadUnusedAssets());
    }
}
