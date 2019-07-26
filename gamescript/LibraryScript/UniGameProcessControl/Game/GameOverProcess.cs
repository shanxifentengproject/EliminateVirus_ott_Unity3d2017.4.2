using System;
using System.Collections.Generic;
using UnityEngine;


class GameOverProcess : UniProcessModalEvent
{
    public override ModalProcessType processType { get { return ModalProcessType.Process_GameOver; } }
    public GameOverProcess()
        : base()
    {

    }
    //初始化函数
    public override void Initialization()
    {
        //进行一次内存释放
        //Debug.Log("进行一次内存释放");
        UniGameResources.UnloadUnusedAssets();
        base.Initialization();
    }
}
