//#define PLATFORM_COMMON   //通用平台
//#define PLATFORM_AYX      //爱游戏
//#define PLATFORM_LT       //联通
//#define PLATFORM_YDMG     //移动咪咕
//#define PLATFORM_AL       //阿里
//#define PLATFORM_DB       //当贝
//#define PLATFORM_CYBER    // 视博云

//网络存档  视博云
#if PLATFORM_CYBER
#elif _GameType_BaoYue
//使用本地存储档案
#define LOCALARCHIVES
#endif

#if PLATFORM_COMMON || PLATFORM_AYX || PLATFORM_LT || PLATFORM_YDMG || PLATFORM_AL || PLATFORM_DB
//使用本地存储档案
#define LOCALARCHIVES
#endif


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FTLibrary.XML;
using FTLibrary.Command;
using FtGameInput;


abstract class GameCenterEviroment : MonoBehaviourIgnoreGui
{
    public enum PlatformChargeIntensity
    {
        Intensity_Lower = 0,
        Intensity_Normal = 10,
        Intensity_High = 20,
        Intensity_VeryHigh = 30,
        Intensity_VeryVeryHigh = 40,
    }
    public enum GameCenterLinkStatus
    {
        Status_Init,
        Status_Succeed,
        Status_Failed,
    }

    public static GameCenterEviroment currentGameCenterEviroment = null;
    public static GamerProfile currentGamerProfile = null;
    //当前平台收费强度
    public static PlatformChargeIntensity platformChargeIntensity = PlatformChargeIntensity.Intensity_VeryHigh;
    

    public static GameCenterLinkStatus gameCenterLinkStatus = GameCenterLinkStatus.Status_Init;
    //整个平台关联是否准备好了
    public static bool IsAllReady
    {
        get
        {
            if (GameCenterEviroment.gameCenterLinkStatus != GameCenterEviroment.GameCenterLinkStatus.Status_Succeed)
                return false;
            if (currentGamerProfile == null)
                return false;
            if (!currentGamerProfile.IsPlayerDataLoadSucceed)
                return false;
            return true;
        }
    }



    //计算玩家的循环关卡数，实际玩家档案的关卡是无限累计的，会溢出总关卡数
    //使用这个函数可以计算出玩家的循环关卡
    public static int AccountRoundLevel(int level, int maxLevel)
    {
        return level % maxLevel;
    }
    //计算出玩家的真实关卡数，会被限制在设定的最大关卡内
    public static int AccountRealLevel(int level, int maxLevels)
    {
        return (level < maxLevels) ? level : maxLevels;
    }
    //计算出总章节
    public static int AccountMaxSections(int maxLevels, int perSectionLevels)
    {
        return maxLevels / perSectionLevels;
    }
    //根据玩家当前关卡计算出应该显示几个章节
    public static int AccountLevelSection(int level, int maxLevel, int perSectionLevels)
    {
        //计算出实际关卡数
        int ret = AccountRealLevel(level, maxLevel);
        //计算出这个关卡应该在第几章
        ret = ret / perSectionLevels;
        //转换为数量
        ret += 1;
        //计算出最大章节数
        int maxSections = AccountMaxSections(maxLevel, perSectionLevels);
        //计算出的关卡数不可以超过最大关卡数
        return (ret < maxSections) ? ret : maxSections;
    }
    //计算玩家的关卡数应该定位在那一个章节索引上，索引是从0开始的
    public static int AccountLevelSectionIndex(int level, int maxLevel, int perSectionLevels)
    {
        //计算出实际关卡数
        int ret = AccountRealLevel(level, maxLevel);
        //计算出这个关卡应该在第几章
        ret = ret / perSectionLevels;
        //计算出最大章节数
        int maxSections = AccountMaxSections(maxLevel, perSectionLevels);
        return (ret < maxSections) ? ret : maxSections - 1;

    }
    //判断当前关卡索引是否是我档案关卡的那一页
    public static bool IsMyLevelSectionIndex(int level, int maxLevel, int perSectionLevels, int currentSectionIndex)
    {
        //计算出玩家的关卡数在第几个章节索引上
        int levelSectionIndex = AccountLevelSectionIndex(level, maxLevel, perSectionLevels);
        return levelSectionIndex == currentSectionIndex;
    }
    //计算玩家在当前章节的索引值，如果玩家档案不在当前章节页面则索引到位到0
    public static int AccountLevelSectionLevelIndex(int level, int maxLevel, int perSectionLevels, int currentSectionIndex)
    {
        //如果不是我档案的这一页就定位在0上
        if (!IsMyLevelSectionIndex(level, maxLevel, perSectionLevels, currentSectionIndex))
            return 0;
        //计算真实关卡数
        int realLevel = AccountRealLevel(level, maxLevel);
        //计算出章节数
        int realSections = realLevel / perSectionLevels;
        realSections += 1;
        //计算出最大章节数
        int maxSections = AccountMaxSections(maxLevel, perSectionLevels);
        //如果计算出来的章节数已经超过了最大章节数，说明关卡溢出了
        //定位在本页的最后一关
        if (realSections > maxSections)
            return perSectionLevels - 1;
        //否则求余
        return realLevel % perSectionLevels;
    }
    //计算出当前章节的开始关卡
    public static int AccountSectionStartLevel(int currentSectionIndex, int perSectionLevels)
    {
        return perSectionLevels * currentSectionIndex;
    }
    //计算当前章节的结束关卡
    public static int AccountSectionEndLevel(int currentSectionIndex, int perSectionLevels)
    {
        return (currentSectionIndex + 1) * perSectionLevels - 1;
    }


    







    protected override void Awake()
    {
        base.Awake();
        GameCenterEviroment.currentGameCenterEviroment = this;
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
    }

    //初始化游戏中心
    public void InitializationGameCenter()
    {
#if !UNITY_EDITOR && !PLATFORM_COMMON && UNITY_ANDROID
    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
        {
            jo.Call("InitializationGameCenter");
        }

    }
#else
        InitializationGameCenterCallBack("1");
#endif //UNITY_ANDROID
    }
    //游戏中心连接回调
    public void InitializationGameCenterCallBack(string issucceed)
    {
        if (GameCenterEviroment.gameCenterLinkStatus != GameCenterLinkStatus.Status_Init)
            return;
        GameCenterEviroment.gameCenterLinkStatus = (issucceed == "1") ? GameCenterLinkStatus.Status_Succeed : GameCenterLinkStatus.Status_Failed;
        if (GameCenterEviroment.gameCenterLinkStatus == GameCenterLinkStatus.Status_Succeed)
        {
            OnGameCenterLinkSucceed();
        }
        else
        {
            OnGameCenterLinkFailed();
        }
    }


    /** 在与游戏中心连接完成并登陆成功后进行下面函数的访问*/
    /** 获取用户的积分*/
    public int getPlayerScore()
    {
        try
        {
#if !UNITY_EDITOR && !LOCALARCHIVES && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<int>("getPlayerScore");
            }

        }
#else
            //由于取消加密系统，为了适应之前的旧系统
            //首先，从新文件读取内容，如果未读取到，则从旧文件读取内容
            //首先以加密的方式从旧文件读取内容，如果未读取到，则以未加密的
            //方式从旧文件读取，如果都读取失败，则为失败
            
            using (UniDataFileReader reader = new UniDataFileReader())
            {
                if (!reader.Open("new_gamecenter_playerscore.txt", false))
                {
                    if (!reader.Open("gamecenter_playerscore.txt", true))
                    {
                        if (!reader.Open("gamecenter_playerscore.txt",false))
                            return 0;
                    }
                }
                int ret = reader.reader.ReadInt32();
                reader.Close();
                return ret;

            }
#endif //UNITY_ANDROID
        }
        catch (System.Exception ex)
        {
            return 0;
        }
    }
    public void setPlayerScore(int score)
    {
#if !UNITY_EDITOR && !LOCALARCHIVES && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                jo.Call("setPlayerScore",score);
            }

        }
#else
        //为了适应之前的系统，这里，非加密档案，更换新的存储文件
        //using (UniDataFileWriter writer = new UniDataFileWriter())
        //{
        //    writer.Open("gamecenter_playerscore.txt");
        //    writer.writer.Write(score);
        //    writer.Close();
        //}
        using (UniDataFileWriter writer = new UniDataFileWriter())
        {
            writer.Open("new_gamecenter_playerscore.txt",false);
            writer.writer.Write(score);
            writer.Close();
        }
#endif
    }
    /** 获取用户的档案*/
    public string getPlayerParam(string paramName)
    {
        try
        {
#if !UNITY_EDITOR && !LOCALARCHIVES && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return jo.Call<string>("getPlayerParam", paramName);
            }

        }
#else
            //由于取消加密系统，为了适应之前的旧系统
            //首先，从新文件读取内容，如果未读取到，则从旧文件读取内容
            //首先以加密的方式从旧文件读取内容，如果未读取到，则以未加密的
            //方式从旧文件读取，如果都读取失败，则为失败
            using (UniDataFileReader reader = new UniDataFileReader())
            {
                if (!reader.Open(UniGameResources.ConnectPath("new_gamecenter_",
                paramName, ".txt"),false))
                {
                    if (!reader.Open(UniGameResources.ConnectPath("gamecenter_",
                                paramName, ".txt"), true))
                    {
                        if (!reader.Open(UniGameResources.ConnectPath("gamecenter_",
                                paramName, ".txt"), false))
                            return "";
                    }
                }
                string ret = reader.reader.ReadString();
                reader.Close();
                return ret;
            }
#endif //UNITY_ANDROID
        }
        catch (System.Exception ex)
        {
            return "";
        }
    }
    public void setPlayerParam(string paramName, string sParam)
    {
#if !UNITY_EDITOR && !LOCALARCHIVES && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                jo.Call("setPlayerParam", paramName, sParam);
            }

        }
#else
        //为了适应之前的系统，这里，非加密档案，更换新的存储文件
        //using (UniDataFileWriter writer = new UniDataFileWriter())
        //{
        //    writer.Open(UniGameResources.ConnectPath("gamecenter_",
        //    paramName, ".txt"));
        //    writer.writer.Write(sParam);
        //    writer.Close();
        //}

        using (UniDataFileWriter writer = new UniDataFileWriter())
        {
            writer.Open(UniGameResources.ConnectPath("new_gamecenter_",
            paramName, ".txt"),false);
            writer.writer.Write(sParam);
            writer.Close();
        }
#endif //UNITY_ANDROID
    }



    //付费成功回调
    public delegate void RechargePayCallback(int payid,bool issucceed);
    //当前付费回调函数
    private static GameCenterEviroment.RechargePayCallback currentPayBackFuntion = null;
    private static int LastPayId;
    /** 激活收费过程 */
    public void OpenPlayerPayMoney(int payid, GameCenterEviroment.RechargePayCallback backFuntion)
    {
        LastPayId = payid;
        currentPayBackFuntion = backFuntion;
#if !UNITY_EDITOR && !PLATFORM_COMMON && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //标记为合理挂起，这样，网络不会被断开        
                NetInputDefine.Rational_Hang_Up = true;

                jo.Call("OpenPlayerPayMoney", payid);
            }

        }
#else
        PlayerPayMoneyCallBack("");
#endif //UNITY_ANDROID
    }
    /** 收费返回 */
    public void PlayerPayMoneyCallBack(string s)
    {
        int payid;
        bool isPaySucceed = false;

#if !UNITY_EDITOR && !PLATFORM_COMMON && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                payid = jo.GetStatic<int>("LastPayId");
                isPaySucceed = jo.GetStatic<bool>("IsPaySucceed");

                //关闭合理挂起
                NetInputDefine.Rational_Hang_Up = false;
            }

        }
#else
        payid = LastPayId;
        isPaySucceed = CheckPlayerPayMoneyIsSucceedDevelop(payid);
#endif //UNITY_ANDROID
//忽略默认付费提升
#if __AbsentUpDefBuyItemId
#else
        if (isPaySucceed == true && payid > ((IGamerProfile)(IGameCenterEviroment.currentGamerProfile)).playerdata.defBuyItemId)
        {
            ((IGamerProfile)(IGameCenterEviroment.currentGamerProfile)).playerdata.defBuyItemId = payid;
            ((IGamerProfile)(IGameCenterEviroment.currentGamerProfile)).SaveGamerProfileToServer();
        }
#endif // __AbsentUpDefBuyItemId
        if (currentPayBackFuntion != null)
        {
            currentPayBackFuntion(payid, isPaySucceed);
            currentPayBackFuntion = null;
        }
    }
    protected virtual bool CheckPlayerPayMoneyIsSucceedDevelop(int payid)
    {
        return true;
    }
     /** 获取收费强度*/
    private PlatformChargeIntensity getPlatformChargeIntensity()
    {
#if _GameType_BaoYue
        return PlatformChargeIntensity.Intensity_Normal;
#else
#if !UNITY_EDITOR && !PLATFORM_COMMON && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return (PlatformChargeIntensity)jo.Call<int>("getPlatformChargeIntensity");
            }

        }
#else
        return GameCenterEviroment.currentGamerProfile.getPlatformChargeIntensity();
#endif //UNITY_ANDROID
#endif //_GameType_BaoYue
    }


    //游戏中心连接成功
    protected virtual void OnGameCenterLinkSucceed()
    {
        //游戏中心连接成功
        //构造档案对象
        currentGamerProfile = AllocGamerProfile();
        if (!currentGamerProfile.Initialization())
        {
            gameCenterLinkStatus = GameCenterLinkStatus.Status_Failed;
            OnGameCenterLinkFailed();
        }
        //获取定义的收费强度
        platformChargeIntensity = getPlatformChargeIntensity();
    }
    //游戏中心连接失败
    protected virtual void OnGameCenterLinkFailed()
    {

    }


    //申请档案对象
    protected abstract GamerProfile AllocGamerProfile();

}
