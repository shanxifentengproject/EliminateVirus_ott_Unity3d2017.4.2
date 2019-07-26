using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTLibrary.XML;

class GameBaseDefine
{
#if _GameType_BaoYue
    private const string fileName = "gamebasedefine_by.xml";
#else
    private const string fileName = "gamebasedefine.xml";
#endif
    public struct GameJewelData
    {
        public struct BuyJewel
        {
            public IGameCenterEviroment.PlayerPayMoneyItem itemId;
            public int rmb;
            public int jewel;
        }
        //一次付费购买多少钻，付多少钱是在收费接口底层配置的
        public BuyJewel[] buyJewelList;
        public BuyJewel[] ShowbuyJewelList;

        public IGameCenterEviroment.PlayerPayMoneyItem defItemId;
        //钻石到金币的兑换比率
        public int jeweltomoney;
        //开发模式模拟单次最高充值限定额度
        public int developmaxbuyid;
    }
    public GameJewelData jewelData = new GameJewelData();



    public struct PlatformChargeIntensityData
    {
        //当前定义的付费强度
        public GameCenterEviroment.PlatformChargeIntensity defPlatformChargeIntensity;
        //<!--登陆大礼包 effectlevel 从第几关开始有效可见  -1 不限制，首关有效 很大的数值，所有时间都不现实-->
        //    opentimes 一次游戏可以显示几次该界面，-1不限制，
        //    弹出界面默认按钮位置 btnindex  0确定 1取消 （对应游戏中的定义） -->
        public int closeLevel_LoginAward;
        public int closeLevel_LoginAward_OpenTimes;
        public int closeLevel_LoginAward_BtnIndex;
        //<!--第几关之前默认都选择第一个免费角色-->
        public int closeLevel_SelectCharacter;
        //<!--提示角色需要升级-->
        public int closeLevel_CharacterLevelSale;
        public int closeLevel_CharacterLevelSale_OpenTimes;
        public int closeLevel_CharacterLevelSale_BtnIndex;
        //<!--游戏过程收费 多少关前都免费-->
        public int closeLevel_GameCharge;
        public int closeLevel_Foreverfree;
        //<!--打开宝箱，多少关前免费 btnindex在这里0代表放弃  1代表单开，2代表全开-->
        public int closeLevel_Treasure;
        public int closeLevel_Treasure_BtnIndex;
        //<!--首页退出游戏界面 默认按钮位置-->
        public int closeLevel_ExitGameDlg_BtnIndex;
        //<!--资金不足界面 默认按钮位置-->
        public int closeLevel_RechargeAsk_BtnIndex;
        //<!--复活界面 默认按钮位置 ，btnindex在这里0代表复活  1代表放弃-->
        public int closeLevel_Revive_BtnIndex;

    }

    public PlatformChargeIntensityData platformChargeIntensityData = new PlatformChargeIntensityData();

    public struct GameParameter
    {
        public struct NewPlayer
        {
            public int playermoney;
        }
        public NewPlayer newPlayer;
        public struct ButtleBuy
        {
            public int onebuttlemoney;
            public int maxbuybuttle;
        }
        public ButtleBuy buttleBuy;
        public struct Treasure
        {
            public int openonce;
            public int openall;
            public enum AwardType
            {
                AwardType_Equip = 0,
                AwardType_Buttle = 1,
                AwardType_Money = 2,
            }
            public struct AwardData
            {
                public AwardType type;
                public int minvalue;
                public int maxvalue;
                public int multiple;
                public int rate;
            }
            public AwardData[] awardData;
            public struct AwardValueData
            {
                public AwardType type;
                public int value;
            }
            public AwardValueData RandomAwardValueData()
            {
                AwardData selData = awardData[awardData.Length - 1];
                int value = FTLibrary.Command.FTRandom.Next(100);
                int total = 0;
                for (int i = 0; i < awardData.Length; i++)
                {
                    total += awardData[i].rate;
                    if (value < total)
                    {
                        selData = awardData[i];
                        break;
                    }
                }
                AwardValueData ret = new AwardValueData();
                ret.type = selData.type;
                ret.value = (selData.minvalue + FTLibrary.Command.FTRandom.Next(selData.maxvalue - selData.minvalue + 1)) * selData.multiple;
                return ret;
            }
        }
        public Treasure treasure;

        public struct LoginAward
        {
            public int paymoney;
            public int[] skill;
            public int[] equip;
        }
        public LoginAward loginAward;
    }
    public GameParameter gameParameter = new GameParameter();

    public void Load()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("GameBaseDefine");
        XmlNode node = root.SelectSingleNode("jewel");
        XmlNode n;

        jewelData.defItemId = (IGameCenterEviroment.PlayerPayMoneyItem)Convert.ToInt32(node.Attribute("defbuyjewel"));
        jewelData.jeweltomoney = Convert.ToInt32(node.Attribute("jeweltomoney"));
        jewelData.developmaxbuyid = Convert.ToInt32(node.Attribute("developmaxbuyid"));

        XmlNodeList nodelist = node.SelectSingleNode("BuyJewelList").SelectNodes("buyjewel");
        jewelData.buyJewelList = new GameJewelData.BuyJewel[nodelist.Count];
        for (int i = 0; i < jewelData.buyJewelList.Length; i++)
        {
            n = nodelist[i];
            jewelData.buyJewelList[i].itemId = (IGameCenterEviroment.PlayerPayMoneyItem)Convert.ToInt32(n.Attribute("id"));
            jewelData.buyJewelList[i].rmb = Convert.ToInt32(n.Attribute("rmb"));
            jewelData.buyJewelList[i].jewel = Convert.ToInt32(n.Attribute("jewel"));
        }

        nodelist = node.SelectSingleNode("ShowBuyJewelList").SelectNodes("buyjewel");
        jewelData.ShowbuyJewelList = new GameJewelData.BuyJewel[nodelist.Count];
        for (int i = 0; i < jewelData.ShowbuyJewelList.Length; i++)
        {
            n = nodelist[i];
            jewelData.ShowbuyJewelList[i] = jewelData.buyJewelList[Convert.ToInt32(n.Attribute("id"))];
        }

        node = root.SelectSingleNode("PlatformChargeIntensity");
        platformChargeIntensityData.defPlatformChargeIntensity = (GameCenterEviroment.PlatformChargeIntensity)Convert.ToInt32(node.Attribute("value"));
        //<!--登陆大礼包 effectlevel 从第几关开始有效可见  -1 不限制，首关有效 很大的数值，所有时间都不现实-->
        platformChargeIntensityData.closeLevel_LoginAward = Convert.ToInt32 ( node.SelectSingleNode ( "LoginAward" ).Attribute ( "closelevel" ) );
        platformChargeIntensityData.closeLevel_LoginAward_OpenTimes = Convert.ToInt32 ( node.SelectSingleNode ( "LoginAward" ).Attribute ( "opentimes" ) );
        platformChargeIntensityData.closeLevel_LoginAward_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "LoginAward" ).Attribute ( "btnindex" ) );
        //<!--第几关之前默认都选择第一个免费角色-->
        platformChargeIntensityData.closeLevel_SelectCharacter = Convert.ToInt32 ( node.SelectSingleNode ( "SelectCharacter" ).Attribute ( "closelevel" ) );
        //<!--提示角色需要升级-->
        platformChargeIntensityData.closeLevel_CharacterLevelSale = Convert.ToInt32 ( node.SelectSingleNode ( "CharacterLevelSale" ).Attribute ( "closelevel" ) );
        platformChargeIntensityData.closeLevel_CharacterLevelSale_OpenTimes = Convert.ToInt32 ( node.SelectSingleNode ( "CharacterLevelSale" ).Attribute ( "opentimes" ) );
        platformChargeIntensityData.closeLevel_CharacterLevelSale_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "CharacterLevelSale" ).Attribute ( "btnindex" ) );
        //<!--游戏过程收费 多少关前都免费-->
        platformChargeIntensityData.closeLevel_GameCharge = Convert.ToInt32 ( node.SelectSingleNode ( "GameCharge" ).Attribute ( "closelevel" ) );
        platformChargeIntensityData.closeLevel_Foreverfree = Convert.ToInt32 ( node.SelectSingleNode ( "GameCharge" ).Attribute ( "foreverfree" ) );
        //<!--打开宝箱，多少关前免费  btnindex在这里0代表放弃  1代表单开，2代表全开-->
        platformChargeIntensityData.closeLevel_Treasure = Convert.ToInt32 ( node.SelectSingleNode ( "Treasure" ).Attribute ( "closelevel" ) );
        platformChargeIntensityData.closeLevel_Treasure_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "Treasure" ).Attribute ( "btnindex" ) );
        //<!--首页退出游戏界面 默认按钮位置-->
        platformChargeIntensityData.closeLevel_ExitGameDlg_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "ExitGameDlg" ).Attribute ( "btnindex" ) );
        //<!--资金不足界面 默认按钮位置-->
        platformChargeIntensityData.closeLevel_RechargeAsk_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "RechargeAsk" ).Attribute ( "btnindex" ) );
        //<!--复活界面 默认按钮位置 ，btnindex在这里0代表复活  1代表放弃-->
        platformChargeIntensityData.closeLevel_Revive_BtnIndex = Convert.ToInt32 ( node.SelectSingleNode ( "ReviveDlg" ).Attribute ( "btnindex" ) );


        node = root.SelectSingleNode("GameParameter");
        n = node.SelectSingleNode("NewPlayer");
        gameParameter.newPlayer.playermoney = Convert.ToInt32(n.Attribute("playermoney"));
        n = node.SelectSingleNode("ButtleBuy");
        gameParameter.buttleBuy.onebuttlemoney = Convert.ToInt32(n.Attribute("onebuttlemoney"));
        gameParameter.buttleBuy.maxbuybuttle = Convert.ToInt32(n.Attribute("maxbuybuttle"));
        n = node.SelectSingleNode("Treasure");
        gameParameter.treasure.openonce = Convert.ToInt32(n.Attribute("openonce"));
        gameParameter.treasure.openall = Convert.ToInt32(n.Attribute("openall"));
        nodelist = n.SelectNodes("Award");
        gameParameter.treasure.awardData = new GameParameter.Treasure.AwardData[nodelist.Count];
        for (int i = 0; i < nodelist.Count; i++)
        {
            n = nodelist[i];
            gameParameter.treasure.awardData[i].type = (GameParameter.Treasure.AwardType)Convert.ToInt32(n.Attribute("id"));
            gameParameter.treasure.awardData[i].minvalue = Convert.ToInt32(n.Attribute("minvalue"));
            gameParameter.treasure.awardData[i].maxvalue = Convert.ToInt32(n.Attribute("maxvalue"));
            gameParameter.treasure.awardData[i].multiple = Convert.ToInt32(n.Attribute("multiple"));
            gameParameter.treasure.awardData[i].rate = Convert.ToInt32(n.Attribute("rate"));
        }

        n = node.SelectSingleNode("LoginAward");
        gameParameter.loginAward.paymoney = Convert.ToInt32(n.Attribute("paymoney"));
        gameParameter.loginAward.skill = new int[3];
        gameParameter.loginAward.skill[0] = Convert.ToInt32(n.Attribute("skill1"));
        gameParameter.loginAward.skill[1] = Convert.ToInt32(n.Attribute("skill2"));
        gameParameter.loginAward.skill[2] = Convert.ToInt32(n.Attribute("skill3"));
        gameParameter.loginAward.equip = new int[3];
        gameParameter.loginAward.equip[0] = Convert.ToInt32(n.Attribute("equip1"));
        gameParameter.loginAward.equip[1] = Convert.ToInt32(n.Attribute("equip2"));
        gameParameter.loginAward.equip[2] = Convert.ToInt32(n.Attribute("equip3"));
    }
}
