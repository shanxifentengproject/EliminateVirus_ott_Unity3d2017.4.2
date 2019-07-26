using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class RaceSceneControl : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Race; } }
    public static UiSceneUICamera.UISceneId firstUISceneId = UiSceneUICamera.UISceneId.Id_UIGameStart;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        UiSceneUICamera.Instance.CreateAloneScene(RaceSceneControl.firstUISceneId);
        //最后都初始化完成后取消启动画面
        if (SceneControl.gameBootFace != null)
        {
            SceneControl.gameBootFace.CloseGameBootFace();
            SceneControl.gameBootFace = null;
        }
        //如果有载入进度界面，需要删除
        UiSceneUICamera.Instance.ReleasePoolingScene((int)UiSceneUICamera.UISceneId.Id_UIGameLoading);
        //激活为当前监听源
        MusicPlayer.activeMyListener = true;
        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
        MusicPlayer.Play("ui.mp3", true);
    }
}
