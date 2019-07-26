using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


class GameSceneControl : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Game; } }
    protected override void Start()
    {
        base.Start();
    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        //如果有载入进度界面，需要删除
        //UiSceneUICamera.Instance.ReleasePoolingScene((int)UiSceneUICamera.UISceneId.Id_UIGameLoading);
    }
}
