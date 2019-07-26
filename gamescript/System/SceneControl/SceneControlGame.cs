using System;
using System.Collections.Generic;
using UnityEngine;
class SceneControlGame : SceneControl
{
    public override SceneControlType SceneType { get { return SceneControlType.SceneControl_Game; } }
    public static UiSceneGameLoading.LoadingType loadingMode = UiSceneGameLoading.LoadingType.Type_LoadingGameNew;

    public bool IsStartWork {get;set;}
    protected override void Awake()
    {
        IsStartWork = false;
        base.Awake();
        ////设置使用那把枪
        //if (IGamerProfile.Instance != null)
        //{
        //    sniperGunHanddle.GunIndex = IGamerProfile.gameCharacter.gunDataList[IGamerProfile.Instance.gameEviroment.gunIndex].gunRealIndex;
        //}
        //关闭为当前监听源
        //MusicPlayer.activeMyListener = false;
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void LinkSceneInfo()
    {

    }
    //这里的场景初始化只是做场景的构造，对场景的配置不应该放在这里
    public override void Initialization()
    {
        base.Initialization();

        //开始音乐播放
        MusicPlayer.Stop(true);
        MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
        MusicPlayer.Play("game.mp3", true);

        //设置环境参数
        IGamerProfile.Instance.gameEviroment.fireCount = 0;
        IGamerProfile.Instance.gameEviroment.killCount = 0;
        IGamerProfile.Instance.gameEviroment.useTime = (float)IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                                    levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].time;
                                            
        //加载游戏主UI
        UiSceneUICamera.Instance.CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameMain);
        //载入完成了去掉加载界面
        GuiUiSceneBase loading = UiSceneUICamera.Instance.FindPoolingScene((int)UiSceneUICamera.UISceneId.Id_UIGameLoading);
        if (loading != null)
        {
            UiSceneUICamera.Instance.RemovePoolingScene((int)UiSceneUICamera.UISceneId.Id_UIGameLoading);
            ((IUniGameBootFace1)loading.gameObject.GetComponent<IUniGameBootFace1>()).CloseGameBootFace();
        }
        //首先在延迟时间后调整枪的角度
        Invoke("AujstGunZoom", 2.0f);
        
    }
    private void AujstGunZoom()
    {
        Invoke("StartWrok", 0.5f);
    }
    private void StartWrok()
    {
        IsStartWork = true;
    }
    
    protected override void OnDestroyScene()
    {
        base.OnDestroyScene();
    }
}
