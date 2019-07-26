using System;
using System.IO;
using FTLibrary.XML;
using UnityEngine;
class UniInsertCoinsOptionsFile : UniOptionsFileBase
{
    public UniInsertCoinsOptionsFile(string filePath, UniGameResources gameresources)
        : base(filePath, gameresources)
    {
        
    }
    public enum GameChargeMode
    {
        Mode_Free = 0,     //免费模式
        Mode_Charge = 1     //收费模式
    }
    public enum GameAwardMode
    {
        Mode_Games = 0,     //每局游戏奖励
        Mode_Scores = 1,    //达到一定分数进行奖励
        Mode_LossPerCent = 2,//赔率奖励
    }
    //游戏收费模式
    protected static GameChargeMode defaultChargeMode;
    //需要游戏投币数
    protected static int defaultCoins;
    //可以使用的游戏时间
    protected static float defaultTimes;
    //奖励模式
    protected static GameAwardMode defaultAwardMode;
    //奖励的数量
    protected static int defaultAwardCount;
    //奖励的分数
    protected static int defaultAwardNeedScore;
    //赔率值
    protected static int defaultLossPerCent;
    
   
    public static void LoadInsertCoinsOptionsDefaultInfo(string fileName, UniGameResources gameResources)
    {
        XmlDocument doc = gameResources.LoadResource_PublicXmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("InsertCoinsOptions");

        XmlNode node = root.SelectSingleNode("ChargeMode");
        switch (node.Attribute("mode"))
        {
            case "Free":
                defaultChargeMode = GameChargeMode.Mode_Free;
                break;
            case "Charge":
                defaultChargeMode = GameChargeMode.Mode_Charge;
                break;
        }

        node = root.SelectSingleNode("ChargeData");
        defaultCoins = Convert.ToInt32(node.Attribute("coins"));
        defaultTimes = Convert.ToSingle(node.Attribute("times"));

        node = root.SelectSingleNode("AwardMode");
        switch (node.Attribute("mode"))
        {
            case "Games":
                defaultAwardMode = GameAwardMode.Mode_Games;
                break;
            case "Scores":
                defaultAwardMode = GameAwardMode.Mode_Scores;
                break;
            case "LossPerCent":
                defaultAwardMode = GameAwardMode.Mode_LossPerCent;
                break;
        }
        node = root.SelectSingleNode("AwardData");
        defaultAwardCount = Convert.ToInt32(node.Attribute("count"));
        defaultAwardNeedScore = Convert.ToInt32(node.Attribute("score"));
        defaultLossPerCent = Convert.ToInt32(node.Attribute("losspercent"));
    }


    public override uint OptionsType
    {
        get
        {
            //case 0xF171520D://UniInsertCoinsOptionsFile CRC32HashCode
            return 0xF171520D;
        }
    }
    //当前配置文件的版本号
    //直接版本号一致才可以读取
    protected override string OptionsVersion { get { return "1.1"; } }
    protected override void FillDefaultOptions()
    {
        //游戏收费模式
        chargeMode = defaultChargeMode;
        //开始游戏需要几币
        coins = defaultCoins;
        times = defaultTimes;
        awardMode = defaultAwardMode;
        awardCount = defaultAwardCount;
        awardNeedScore = defaultAwardNeedScore;
        lossPerCent = defaultLossPerCent;
        //SaveOptions();
    }
    protected override void LoadOptions(BinaryReader reader)
    {
        base.LoadOptions(reader);
        //游戏收费模式
        chargeMode = (GameChargeMode)reader.ReadInt32();
        //开始游戏需要几币
        coins = reader.ReadInt32();
        times = reader.ReadSingle();
        awardMode = (GameAwardMode)reader.ReadInt32();
        awardCount = reader.ReadInt32();
        awardNeedScore = reader.ReadInt32();
        lossPerCent = reader.ReadInt32();
    }
    protected override void SaveOptions(BinaryWriter writer)
    {
        base.SaveOptions(writer);
        writer.Write((int)chargeMode);
        writer.Write(coins);
        writer.Write(times);
        writer.Write((int)awardMode);
        writer.Write(awardCount);
        writer.Write(awardNeedScore);
        writer.Write(lossPerCent);
    }

    //游戏收费模式
    public GameChargeMode chargeMode;
    //需要游戏投币数
    public int coins;
    //可以使用的游戏时间
    public float times;
    //奖励模式
    public GameAwardMode awardMode;
    //奖励的数量
    public int awardCount;
    //奖励的分数
    public int awardNeedScore;
    //赔率值
    public int lossPerCent;
}
