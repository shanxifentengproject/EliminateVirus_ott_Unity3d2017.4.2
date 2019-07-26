using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class UiSceneUICamera : GuiUiSceneManager
{
    public enum UISceneId
    {
        Id_UICamera = 1,
        Id_UIGameStart,
        Id_UIGameMap,
        Id_UIGameCharacter,
        Id_UIGameMain,
        Id_UIGameHelp,
        Id_UIGameClose,
        Id_UIGameBack,
        Id_UIGameOver,
        Id_UIGameLoading,
        Id_UIGameRechargeAsk,
        Id_UIGameBuyButtleAsk,
        Id_UIGameAward,
        Id_UIGameLoginAward,
        Id_UICharacterLevelSale,
        Id_UINotEnoughMoney,
    }

    private static UiSceneUICamera myInstance = null;
    public static UiSceneUICamera Instance
    {
        get
        {
            return myInstance;
        }
        set
        {
            myInstance = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        UiSceneUICamera.Instance = this;
    }


    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UICamera; } }
    
    //获取UI摄像机，UI上的控件需要重写此函数
    public override GuiUiSceneBase UICamreaPtr
    {
        get
        {
            return this;
        }
    }
    //获取当前全局的选择对象
    public override GuiSelectFlag AllocSelectFlag()
    {
        return LoadResource_UIPrefabs("SelectFlag.prefab").GetComponent<GuiSelectFlag>();
    }
    
  
    protected override void OnInitializationUI()
    {
        
    }
    public GuiUiSceneBase CreateAloneScene(UISceneId id, params object[] args)
    {
        switch(id)
        {
            case UISceneId.Id_UIGameStart:
                return ActiveAloneUiScene("startgame.prefab", args);
            case UiSceneUICamera.UISceneId.Id_UIGameMap:
                return ActiveAloneUiScene("gamemap.prefab", args);
            case UiSceneUICamera.UISceneId.Id_UIGameCharacter:
                return ActiveAloneUiScene("gamecharacter.prefab", args);
            case UiSceneUICamera.UISceneId.Id_UIGameMain:
                return ActiveAloneUiScene("gamemain.prefab", args);
            case UiSceneUICamera.UISceneId.Id_UIGameOver:
                return ActiveAloneUiScene("gameover.prefab", args);
        }
        return null;
    }
    public void ReleaseAloneScene()
    {
        ActiveAloneUiScene("");
    }

    public GuiUiSceneBase CreatePoolingScene(UISceneId id,params object[] args)
    {
        switch (id)
        {
            case UISceneId.Id_UIGameLoading:
                return ActivePoolingScene("gameloading.prefab", args);
        }
        return null;
    }
    
    //屏幕过渡效果
    public void FadeScreen()
    {
        LoadResource_UIPrefabs("FadeScreen.prefab");
    }

    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate() 
    {
        return true; 
    }

    //加载下一关的地图
    public void LoadNextLevel()
    {
        //开始音乐播放
        MusicPlayer.Stop(true);
        //检测如果地图有更新则需要更换新地图
        if (IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] ==
                        IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex])
        {
            //切换到下一张地图
            IGamerProfile.Instance.gameEviroment.mapIndex += 1;
            if (IGamerProfile.Instance.gameEviroment.mapIndex >= IGamerProfile.gameLevel.mapMaxLevel.Length)
            {
                Debug.Log("更新地图!");
                IGamerProfile.Instance.gameEviroment.mapIndex = 0;

            }
        }
        //子关卡选择档案存储的最后一关
        IGamerProfile.Instance.gameEviroment.mapLevelIndex = IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex];
        if (IGamerProfile.Instance.gameEviroment.mapLevelIndex >= IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex])
        {
            IGamerProfile.Instance.gameEviroment.mapLevelIndex = IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex] - 1;
        }
        //设置为返回游戏的状态
        UiSceneSelectGameCharacter.selectCharacterMode = UiSceneSelectGameCharacter.SelectCharacterMode.Mode_NextGame;
        //进入加载界面
        ((UiSceneUICamera)UIManager).CreatePoolingScene(UiSceneUICamera.UISceneId.Id_UIGameLoading, UiSceneGameLoading.LoadingType.Type_LoadingUICharacter);
        //删除主UI界面
        ((UiSceneUICamera)UIManager).ReleaseAloneScene();
    }
    //重新加载当前地图
    public void RestartLoadLevel()
    {
        //开始音乐播放
        MusicPlayer.Stop(true);
        Debug.Log("重新开始!");
        //设置为进行下一关的模式
        UiSceneSelectGameCharacter.selectCharacterMode = UiSceneSelectGameCharacter.SelectCharacterMode.Mode_NextGame;
        //进入加载界面
        ((UiSceneUICamera)UIManager).CreatePoolingScene(UiSceneUICamera.UISceneId.Id_UIGameLoading, UiSceneGameLoading.LoadingType.Type_LoadingUICharacter);
        //删除主UI界面
        ((UiSceneUICamera)UIManager).ReleaseAloneScene();
    }
    //返回到UI
    public void LoadUILevel(UiSceneGameLoading.LoadingType type)
    {
        //开始音乐播放
        MusicPlayer.Stop(true);
        //设置为返回游戏的状态
        UiSceneSelectGameCharacter.selectCharacterMode = UiSceneSelectGameCharacter.SelectCharacterMode.Mode_IntoGame;
        //进入加载界面
        ((UiSceneUICamera)UIManager).CreatePoolingScene(UiSceneUICamera.UISceneId.Id_UIGameLoading, type);
        //删除主UI界面
        ((UiSceneUICamera)UIManager).ReleaseAloneScene();
    }
}
