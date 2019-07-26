using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;

class IUniGameBootFace1 : UniGameBootFace
{
    public GuiPlaneAnimationProgressBar progressBar;
    //载入进度
    protected AsyncOperation asyncLoad = null;
    //载入进度占进度表示中的百分比
    public float realProgressRate = 0.4f;
    //虚拟载入时间
    public float virtualProgressTime = 2.0f;

    protected override void Start()
    {
        base.Start();
    }
    public override void CloseGameBootFace()
    {
        if (asyncLoad != null)
        {
            asyncLoad = null;
        }
        progressBar.SetProgressBar(1.0f, true);
        base.CloseGameBootFace();
        
    }
    private float progressCurrentTime;
    public delegate void DelegateOnLevelLoadComplete();
    private DelegateOnLevelLoadComplete levelLoadCompleteFuntion = null;
    public void SetStartLevelLoad(AsyncOperation asy)
    {
        SetStartLevelLoad(asy, null);
    }
    public void SetStartLevelLoad(AsyncOperation asy,DelegateOnLevelLoadComplete callbackFun)
    {
        asyncLoad = asy;
        progressCurrentTime = 0.0f;
        if (callbackFun != null)
        {
            levelLoadCompleteFuntion += callbackFun;
        }
    }
    protected virtual void Update()
    {
        if (asyncLoad != null)
        {
            if (asyncLoad.isDone)
            {
                asyncLoad = null;
                progressBar.SetProgressBar(1.0f, true);
                if (levelLoadCompleteFuntion != null)
                {
                    levelLoadCompleteFuntion();
                    levelLoadCompleteFuntion = null;
                }
                return;
            }
            float realProgressValue = asyncLoad.progress;
            //计算虚拟进度条
            progressCurrentTime += Time.deltaTime;
            float virtualProgressValue = UnityEngine.Mathf.Lerp(0.0f, 1.0f, progressCurrentTime / virtualProgressTime);
            progressBar.SetProgressBar(
                        virtualProgressValue * (1.0f - realProgressRate) + realProgressValue * realProgressRate, 
                        true);
        }
    }
}



//class IUniGameBootFace1 : UniGameBootFace
//{
//    public GuiPlaneAnimationProgressBar progressBar;
//    protected AsyncOperation asyncLoad = null;
//    protected Coroutine LoadLevelCoroutine = null;
//    protected override void Start()
//    {
//        base.Start();
//    }
//    public override void CloseGameBootFace()
//    {
//        base.CloseGameBootFace();
//        if (LoadLevelCoroutine != null)
//        {
//            StopCoroutine(LoadLevelCoroutine);
//            LoadLevelCoroutine = null;
//            asyncLoad = null;
//        }
//        ((IUniGameBootFace1)SceneControl.gameBootFace).progressBar.SetProgressBar(1.0f, true);
//    }
//    public void SetStartLevelLoad(AsyncOperation asy)
//    {
//        asyncLoad = asy;
//        LoadLevelCoroutine = StartCoroutine("LoadLevel");
//    }
//    private int Counter = 0;
//    IEnumerator LoadLevel()
//    {
//        while (asyncLoad != null && !asyncLoad.isDone)
//        {
//            ((IUniGameBootFace1)SceneControl.gameBootFace).progressBar.SetProgressBar(asyncLoad.progress, true);
//            //Debug.Log(Counter);
//            //Counter += 1;

//            yield return new WaitForEndOfFrame();
//        }
//        ((IUniGameBootFace1)SceneControl.gameBootFace).progressBar.SetProgressBar(1.0f, true);
//        asyncLoad = null;
//        LoadLevelCoroutine = null;
//    }
//    protected virtual void Update()
//    {
//        Debug.Log(Counter);
//        Counter += 1;
//    }
//}
