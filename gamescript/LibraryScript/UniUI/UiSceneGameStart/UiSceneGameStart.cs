using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneGameStart : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameStart; } }
    private enum ButtonId
    {
        Id_StartGame = 0,
        Id_GameHelp = 1,
        Id_GameClose = 2,
    }
    private GuiExtendButtonGroup buttonGroup = null;
    private FTLibrary.Time.TimeLocker gameCenterCheckTimer = new FTLibrary.Time.TimeLocker(1000);
    protected override void OnInitializationUI()
    {
        buttonGroup = GetComponent<GuiExtendButtonGroup>();
        buttonGroup.selectFuntion += OnButtonSelectOk;
        //如果档案还没有准备好则隐藏选择光标
        buttonGroup.IsWorkDo = false;

        //尝试激活手机控制二维码
        InputDevice.Instance.OpenQRCodeCreator = true;
    }
    protected virtual void Update()
    {
        if (!gameCenterCheckTimer.IsLocked)
        {
            gameCenterCheckTimer.IsLocked = true;
            if (!buttonGroup.IsWorkDo)
            {
                buttonGroup.IsWorkDo = IGameCenterEviroment.IsAllReady;
                if (buttonGroup.IsWorkDo)
                {
                    //不在检测了
                    gameCenterCheckTimer = new FTLibrary.Time.TimeLocker(Int32.MaxValue);
                    gameCenterCheckTimer.IsLocked = true;
                }
            }
        }
    }
    private void OnButtonSelectOk(int index)
    {
        switch(index)
        {
            case (int)ButtonId.Id_StartGame:
                SoundEffectPlayer.Play("buttonok.wav");
                if ((GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_VeryHigh) &&
                        IGameCenterEviroment.effectLoginAward)
                {
                    //让本身停止工作
                    buttonGroup.IsWorkDo = false;
                    LoadResource_UIPrefabs("loginaward.prefab");
                }
                else
                {
                    IntoGame();
                }
                break;
            case (int)ButtonId.Id_GameHelp:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    //让本身停止工作
                    buttonGroup.IsWorkDo = false;
                    LoadResource_UIPrefabs("gamehelp.prefab");                   
                }
                break;
            case (int)ButtonId.Id_GameClose:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    //让本身停止工作
                    buttonGroup.IsWorkDo = false;
                    LoadResource_UIPrefabs("gameclose.prefab");
                }
                break;
        }
    }

    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate()
    {
        if (InputDevice.ButtonBack && buttonGroup.IsWorkDo)
        {
            OnButtonSelectOk((int)ButtonId.Id_GameClose);
            return false;
        }
        return true;

    }
    
    //一个子UI删除
    public override void OnChildUIRelease(GuiUiSceneBase ui)
    {
        if (ui is UiSceneGameHelp)
        {
            //重新设置会光标
            buttonGroup.IsWorkDo = true;
        }
        else if (ui is UiSceneGameClose)
        {
            //重新设置会光标
            buttonGroup.IsWorkDo = true;
        }
        else if (ui is UiSceneLoginAward)
        {
            IntoGame();
        }
    }
    private void IntoGame()
    {
        //闪白
        ((UiSceneUICamera)UIManager).FadeScreen();
        //设置为进入游戏模式
        UiSceneSelectGameCharacter.selectCharacterMode = UiSceneSelectGameCharacter.SelectCharacterMode.Mode_IntoGame;
        //进入角色选择界面
        ((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameCharacter);
    }

    protected override void OnDestroy()
    {
        //关闭手机控制二维码
        InputDevice.Instance.OpenQRCodeCreator = false;
        base.OnDestroy();
    }
}
