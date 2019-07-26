using System;
using System.IO;
class UniGameOptionsDefine
{
    
    //游戏基本配置
    public static UniGameOptionsFile gameOptionsFile = null;
    public static UniInsertCoinsOptionsFile insertCoinsOptionsFile = null;

    public static void LoadGameOptionsDefault(UniGameResources gameResources)
    {
        try
        {
            //加载默认设置
            UniGameOptionsFile.LoadGameOptionsDefaultInfo("GameOptions.xml", gameResources);
            UniInsertCoinsOptionsFile.LoadInsertCoinsOptionsDefaultInfo("InsertCoinsOptions.xml", gameResources);
            gameOptionsFile = new UniGameOptionsFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath, "GameOptions\\GameOptions.dat"),
                                gameResources);
            insertCoinsOptionsFile = new UniInsertCoinsOptionsFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath, "GameOptions\\InsertCoinsOptions.dat"),
                                gameResources);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }
    //加载游戏配置选项信息
    public static void LoadGameOptionsDefine()
    {
        try
        {
            gameOptionsFile.LoadOptions();
            insertCoinsOptionsFile.LoadOptions();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }
    public static void RemoveAllOptions()
    {
        gameOptionsFile.RemoveOptions();
        insertCoinsOptionsFile.RemoveOptions();
    }

    public static void SerializeRead(UniOptionsFileBase[] fileList, MemoryStream s)
    {
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        for (int i = 0; i < fileList.Length;i++ )
        {
            writer.Write(fileList[i].OptionsType);
            fileList[i].SerializeRead(writer);
        }
    }
    public static void SerializeWrite(MemoryStream s)
    {
        s.Seek(0, SeekOrigin.Begin);
        BinaryReader reader = new BinaryReader(s);
        try
        {
            do
            {
                uint type = reader.ReadUInt32();
                UniOptionsFileBase file;
                if (type == gameOptionsFile.OptionsType)
                    file = gameOptionsFile;
                else if (type == insertCoinsOptionsFile.OptionsType)
                    file = insertCoinsOptionsFile;
                else
                    throw new Exception("unknow UniOptionsFile type!");
                file.SerializeWrite(reader);
            } while (reader.BaseStream.Position < reader.BaseStream.Length);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }

    //游戏难度设定
    public static UniGameOptionsFile.GameDifficulty gameDifficulty
    {
        get
        {
            return gameOptionsFile.gameDifficulty;
        }
        set
        {
            gameOptionsFile.gameDifficulty = value;
        }
    }
    //游戏语言设定
    //语言定义
    //中文
    private const uint ProductLanguage_Chinese = 0x00000001;
    //英文
    private const uint ProductLanguage_English = 0x00000002;
    //通用语言定义，这样就可以在产品里自己调了
    //case 0x7119C3FF://ProductLanguage_Common CRC32HashCode
    private const uint ProductLanguage_Common = 0x7119C3FF;
    private static uint deviceLanguageId
    {
        get
        {
#if _SupportDeviceVerify
            return FTLibrary.EliteDevice.EliteDevice.GetDeviceLanguage();
#else
            return ProductLanguage_Chinese;
#endif //_SupportDeviceVerify
            
            
        }
    }
    public static UniGameResources.LanguageDefine gameLanguage
    {
        get 
        {

            UniGameResources.LanguageDefine ret;
            switch (deviceLanguageId)
            {
                case ProductLanguage_Chinese:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("SimplifiedChinese"),
                                        out ret);
                    break;
                case ProductLanguage_English:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("English"),
                                        out ret);
                    break;
                case ProductLanguage_Common:
                    ret = gameOptionsFile.gameLanguage;
                    break;
                default:
                    GameRoot.gameResource.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId("English"),
                                    out ret);
                    break;
            }
            return ret;
        }
        set
        {
            gameOptionsFile.gameLanguage = value;
        }
    }
    //游戏声音设定
    public static float gameVolume
    {
        get { return gameOptionsFile.gameVolume; }
        set
        {
            gameOptionsFile.gameVolume = value;
        }
    }
    //待机背景音乐音量
    public static float StandByMusicVolume
    {
        get { return gameOptionsFile.StandByMusicVolume; }
        set
        {
            gameOptionsFile.StandByMusicVolume = value;
        }
    }
    //游戏显示分辨率设定
    public static UniGameOptionsFile.GameResolution gameResolution
    {
        get { return gameOptionsFile.gameResolution; }
        set
        {
            gameOptionsFile.gameResolution = value;
        }
    }
    public static UnityEngine.Resolution gameResolutionUnity
    {
        get
        {
            return gameOptionsFile.gameResolutionUnity;
        }
    }
    //游戏收费模式
    public static UniInsertCoinsOptionsFile.GameChargeMode chargeMode
    { get { return insertCoinsOptionsFile.chargeMode; } set { insertCoinsOptionsFile.chargeMode = value; insertCoinsOptionsFile.SaveOptions(); } }
    //开始游戏需要几币
    public static int gameCoins
    { get { return insertCoinsOptionsFile.coins; } set { insertCoinsOptionsFile.coins = value; insertCoinsOptionsFile.SaveOptions(); } }

    public static float gameTimes
    { get { return insertCoinsOptionsFile.times; } set { insertCoinsOptionsFile.times = value; insertCoinsOptionsFile.SaveOptions(); } }

    public static UniInsertCoinsOptionsFile.GameAwardMode awardMode
    { get { return insertCoinsOptionsFile.awardMode; } set { insertCoinsOptionsFile.awardMode = value; insertCoinsOptionsFile.SaveOptions(); } }

    public static int gameAwardCount
    { get { return insertCoinsOptionsFile.awardCount; } set { insertCoinsOptionsFile.awardCount = value; insertCoinsOptionsFile.SaveOptions(); } }

    public static int gameAwardNeedScore
    { get { return insertCoinsOptionsFile.awardNeedScore; } set { insertCoinsOptionsFile.awardNeedScore = value; insertCoinsOptionsFile.SaveOptions(); } }

    public static int gameLossPerCent
    { get { return insertCoinsOptionsFile.lossPerCent; } set { insertCoinsOptionsFile.lossPerCent = value; insertCoinsOptionsFile.SaveOptions(); } }
}
