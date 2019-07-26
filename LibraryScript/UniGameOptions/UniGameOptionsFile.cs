using System;
using System.IO;
using FTLibrary.XML;
using UnityEngine;
partial class UniGameOptionsFile : UniOptionsFileBase
{
    //游戏难度设定
    public enum GameDifficulty
    {
        Difficulty_Simpleness = 0,
        Difficulty_Normal = 1,
        Difficulty_Difficulty = 2
    }
    public enum GameResolution
    {
        Resolution_Default,
        Resolution_16_9_1920_1080,
        Resolution_16_9_1600_900,
        Resolution_16_9_1280_720,
    }
    //默认的游戏难度
    protected static GameDifficulty defaultGameDifficulty = GameDifficulty.Difficulty_Normal;
    //系统默认的语言定义ID
    protected static UniGameResources.LanguageDefine defaultGameLanguage;
    //默认的游戏声音定义
    protected static float defaultGameVolume;
    //默认的游戏待机声音定义
    protected static float defaultStandByMusicVolume;
    //游戏默认的分辨率
    protected static GameResolution defaultGameResolution;
    
    public static void LoadGameOptionsDefaultInfo(string fileName,UniGameResources gameResources)
    {
        XmlDocument doc = gameResources.LoadResource_PublicXmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("GameOptions");

        XmlNode node = root.SelectSingleNode("GameDifficulty");
        switch (node.Attribute("difficulty"))
        {
            case "Simpleness":
                defaultGameDifficulty = GameDifficulty.Difficulty_Simpleness;
            break;
            case "Normal":
                defaultGameDifficulty = GameDifficulty.Difficulty_Normal;
            break;
            case "Difficulty":
                defaultGameDifficulty = GameDifficulty.Difficulty_Difficulty;
            break;
            default:
                defaultGameDifficulty = GameDifficulty.Difficulty_Normal;
            break;
        }

        node=root.SelectSingleNode("Language");
        if (!gameResources.FindLanguageDefine(UniGameResources.LanguageDefine.LanguageNameToLanguageId(node.Attribute("name")),out defaultGameLanguage))
        {
            defaultGameLanguage = gameResources.GetDefineLanguage();
        }

        node = root.SelectSingleNode("GameVolume");
        defaultGameVolume = Convert.ToSingle(node.Attribute("volume"));
        defaultStandByMusicVolume = Convert.ToSingle(node.Attribute("standbymusicvolume"));


        node = root.SelectSingleNode("GameResolution");
        if (node.Attribute("resolution") == GameResolution.Resolution_Default.ToString())
        {
            defaultGameResolution = GameResolution.Resolution_Default;
        }
        else if (node.Attribute("resolution") == GameResolution.Resolution_16_9_1920_1080.ToString())
        {
            defaultGameResolution = GameResolution.Resolution_16_9_1920_1080;
        }
        else if (node.Attribute("resolution") == GameResolution.Resolution_16_9_1600_900.ToString())
        {
            defaultGameResolution = GameResolution.Resolution_16_9_1600_900;
        }
        else if (node.Attribute("resolution") == GameResolution.Resolution_16_9_1280_720.ToString())
        {
            defaultGameResolution = GameResolution.Resolution_16_9_1280_720;
        }
        else
        {
            defaultGameResolution = GameResolution.Resolution_Default;
        }

    }

    
    public UniGameOptionsFile(string filePath,UniGameResources gameresources)
        : base(filePath, gameresources)
    {
        
    }

    public override uint OptionsType 
    { 
        get 
        { 
            //case 0x55BBDAE0://UniGameOptionsFile CRC32HashCode
            return 0x55BBDAE0; 
        } 
    }
    //当前配置文件的版本号
    //直接版本号一致才可以读取
    protected override string OptionsVersion { get { return "1.1"; } }
    protected override void FillDefaultOptions()
    {
        m_GameDifficulty = defaultGameDifficulty;
        m_GameLanguage = defaultGameLanguage;
        m_GameVolume = defaultGameVolume;
        m_StandByMusicVolume = defaultStandByMusicVolume;
        m_GameResolution = defaultGameResolution;
        
        //SaveOptions();
    }
    protected override void LoadOptions(BinaryReader reader)
    {
        base.LoadOptions(reader);
        m_GameDifficulty=(GameDifficulty)reader.ReadInt32();
        if (!gameResources.FindLanguageDefine(reader.ReadUInt32(),out m_GameLanguage))
        {
            m_GameLanguage = gameResources.GetDefineLanguage();
        }
        m_GameVolume=reader.ReadSingle();
        m_StandByMusicVolume = reader.ReadSingle();
        m_GameResolution = (GameResolution)reader.ReadInt32();
        
    }
    protected override void SaveOptions(BinaryWriter writer)
    {
        base.SaveOptions(writer);
        writer.Write((int)m_GameDifficulty);
        writer.Write(m_GameLanguage.languageId);
        writer.Write(m_GameVolume);
        writer.Write(m_StandByMusicVolume);
        writer.Write((int)m_GameResolution);
    }
    //游戏难度设定
    protected GameDifficulty m_GameDifficulty;
    public GameDifficulty gameDifficulty
    {
        get
        {
            return m_GameDifficulty;
        }
        set
        {
            m_GameDifficulty=value;
            SaveOptions();
        }
    }
    //游戏语言设定
    protected UniGameResources.LanguageDefine m_GameLanguage;
    public UniGameResources.LanguageDefine gameLanguage
    {
        get { return m_GameLanguage; }
        set
        {
            m_GameLanguage = value;
            SaveOptions();
        }
    }
    //游戏声音设定
    protected float m_GameVolume;
    public float gameVolume
    {
        get { return m_GameVolume; }
        set
        {
            m_GameVolume = value;
            SaveOptions();
        }
    }
    //待机背景音乐音量
    protected float m_StandByMusicVolume;
    public float StandByMusicVolume
    {
        get { return m_StandByMusicVolume; }
        set
        {
            m_StandByMusicVolume = value;
            SaveOptions();
        }
    }
    //游戏显示分辨率设定
    protected GameResolution m_GameResolution;
    public GameResolution gameResolution
    {
        get { return m_GameResolution; }
        set
        {
            m_GameResolution = value;
            SaveOptions();
        }
    }
    public UnityEngine.Resolution gameResolutionUnity
    {
        get
        {
            switch(gameResolution)
            {
                case GameResolution.Resolution_Default:
                    return UnityEngine.Screen.currentResolution;
                case GameResolution.Resolution_16_9_1920_1080:
                    {
                        UnityEngine.Resolution ret = new UnityEngine.Resolution();
                        ret.width = 1920;
                        ret.height = 1080;
                        return ret;
                    }
                case GameResolution.Resolution_16_9_1600_900:
                    {
                        UnityEngine.Resolution ret = new UnityEngine.Resolution();
                        ret.width = 1600;
                        ret.height = 900;
                        return ret;
                    }
                case GameResolution.Resolution_16_9_1280_720:
                    {
                        UnityEngine.Resolution ret = new UnityEngine.Resolution();
                        ret.width = 1280;
                        ret.height = 720;
                        return ret;
                    }
                default:
                    return UnityEngine.Screen.currentResolution;
            }
        }
    }
}
