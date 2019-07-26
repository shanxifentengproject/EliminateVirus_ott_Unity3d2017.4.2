using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
partial class GameRoot
{
    public static UniGameResources gameResource = null;
    public static ConsoleCenter m_ConsoleCenter = null;
    private static bool isRunedGameRootInitialization = false;
    public static void StartInitialization()
    {
        if (isRunedGameRootInitialization)
            return;
        isRunedGameRootInitialization = true;
        Initialization();

    }


    public static EventHandler GameRootInitializationSucceedEvent = null;
    //当前系统编号
    public static HelperInterface.ProduceActivateId produceActivateId;
    public static void Initialization()
    {
        try
        {
            gameResource = new UniGameResources();
            gameResource.Initialization();
        }
        catch (System.Exception ex)
        {
            Error(string.Format("加载资源清单错误:{0}", ex.ToString()));
            return;
        }
        try
        {
            //载入系统设置
            SystemCommand.Initialization(gameResource.LoadResource_XmlFile("systemcommand.xml"));
            //加载游戏默认配置
            UniGameOptionsDefine.LoadGameOptionsDefault(gameResource);
        }
        catch (System.Exception ex)
        {
            Error(string.Format("载入系统设置错误:{0}", ex.ToString()));
            return;
        }
        try
        {
            //加载游戏当前配置
            UniGameOptionsDefine.LoadGameOptionsDefine();
            //加载游戏记录
            UniGameRecordData.LoadAllRecord(gameResource);
            //初始化控制台
            //控制台除了控制和保留一些中间值
            //控制加载过程会覆写一些固化在配置文件内的系统设定
            m_ConsoleCenter = new ConsoleCenter();
            m_ConsoleCenter.Initialization();
        }
        catch (System.Exception ex)
        {
            Error(string.Format("初始化控制台错误:{0}", ex.ToString()));
            return;
        }
        //初始化输入设备
        try
        {
            InputDevice.Initialization();
        }
        catch (System.Exception ex)
        {
            Error(string.Format("初始化输入设备错误:{0}", ex.ToString()));
            return;
        }
        //进行语言清单初始化
        try
        {
            //设置游戏选项里的语言定义
            gameResource.currentLanguageDefine = UniGameOptionsDefine.gameLanguage;
            //载入语言清单
            gameResource.LoadLanguageResourcesInventory();
        }
        catch (System.Exception ex)
        {
            Error(string.Format("加载系统语言清单错误:{0}", ex.ToString()));
            return;
        }
        //载入游戏配置数据
        try
        {
            IGamerProfile.LoadGameConfigData();
        }
        catch (System.Exception ex)
        {
            Error(string.Format("加载游戏配置数据错误:{0}", ex.ToString()));
            return;
        }


        if (GameRootInitializationSucceedEvent != null)
        {
            GameRootInitializationSucceedEvent(null, null);
        }
    }

    private static bool isRunedGameEnvironmentInitialization = false;
    public static MusicPlayer musicPlayer = null;
    public static SoundEffectPlayer soundEffectPlayer = null;
    public static InputDevice inputDevice = null;
    public static UniGameProcessControl gameProcessControl = null;
    //UI摄像机
    public static UniUIOrthographicCamera uiOrthographicCamera = null;
    public static UiSceneUICamera uiSceneUICamera
    {
        get
        {
            return uiOrthographicCamera.GetComponent<UiSceneUICamera>();
        }
    }
    //层管理
    //public static ILayerManager layerManager = null;
    
    
    private static void GameEnvironmentInitialization()
    {
        if (isRunedGameEnvironmentInitialization)
            return;
        isRunedGameEnvironmentInitialization = true;

        //layerManager = new ILayerManager();

        musicPlayer = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("MusicPlayer.prefab"),
                                    null, typeof(MusicPlayer)) as MusicPlayer;
        soundEffectPlayer = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("SoundEffectPlayer.prefab"),
                                    null, typeof(SoundEffectPlayer)) as SoundEffectPlayer;
        inputDevice = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("InputDevice.prefab"),
                                    null, typeof(InputDevice)) as InputDevice;
        gameProcessControl = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("GameProcessControl.prefab"),
                                    null, typeof(UniGameProcessControl)) as UniGameProcessControl;
        uiOrthographicCamera = UniGameResources.NormalizePrefabs(UniGameResources.currentUniGameResources.LoadResource_Prefabs("UICamera.prefab"),
                                    null, typeof(UniUIOrthographicCamera)) as UniUIOrthographicCamera;
    }
}
