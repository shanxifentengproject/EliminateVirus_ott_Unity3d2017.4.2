using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneGameLoading : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameLoading; } }

    public enum LoadingType
    {
        Type_LoadingGameNew,
        Type_LoadingGameRestart,
        Type_LoadingUIMap,
        Type_LoadingUICharacter,
    }

    public IUniGameBootFace1 loadTool;
    private LoadingType loadingType = LoadingType.Type_LoadingGameNew;
    //设置传递的参数
    public override void SetTransferParameter(params object[] args)
    {
        loadingType = (LoadingType)args[0];
    }

    protected override void OnInitializationUI()
    {
        this.Invoke("LoadLevel", 0.5f);
    }

    protected void LoadLevel()
    {
        switch (loadingType)
        {
            case LoadingType.Type_LoadingGameNew:
                {
                    SceneControlGame.loadingMode = LoadingType.Type_LoadingGameNew;
                    AsyncOperation asy = UnityEngine.Application.LoadLevelAsync(IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].scenename);
                    loadTool.SetStartLevelLoad(asy);
                }
                break;
            case LoadingType.Type_LoadingGameRestart:
                {
                    SceneControlGame.loadingMode = LoadingType.Type_LoadingGameRestart;
                    AsyncOperation asy = UnityEngine.Application.LoadLevelAsync(IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].scenename);
                    loadTool.SetStartLevelLoad(asy);
                }
                break;
            case LoadingType.Type_LoadingUIMap:
                {
                    RaceSceneControl.firstUISceneId = UiSceneUICamera.UISceneId.Id_UIGameMap;
                    AsyncOperation asy = UnityEngine.Application.LoadLevelAsync(SystemCommand.FirstSceneName);
                    loadTool.SetStartLevelLoad(asy);
                }
                break;
            case LoadingType.Type_LoadingUICharacter:
                {
                    RaceSceneControl.firstUISceneId = UiSceneUICamera.UISceneId.Id_UIGameCharacter;
                    AsyncOperation asy = UnityEngine.Application.LoadLevelAsync(SystemCommand.FirstSceneName);
                    loadTool.SetStartLevelLoad(asy);
                }
                break;
        }
    }

}
