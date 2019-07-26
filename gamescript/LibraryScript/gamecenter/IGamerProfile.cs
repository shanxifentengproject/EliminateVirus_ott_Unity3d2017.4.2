using System;
using System.Collections.Generic;
using System.Text;

class IGamerProfile : GamerProfile
{
    public static GameBaseDefine gameBaseDefine = new GameBaseDefine();
    public static GameCharacter gameCharacter = new GameCharacter();
    public static GameEquip gameEquip = new GameEquip();
    public static FtGameLevel gameLevel = new FtGameLevel();
    public static GameProp gameProp = new GameProp();
    public static GameSkill gameSkill = new GameSkill();
    public static void LoadGameConfigData()
    {
        gameBaseDefine.Load();
        gameCharacter.Load();
        gameEquip.Load();
        gameLevel.Load();
        gameProp.Load();
        gameSkill.Load();
    }



    private static IGamerProfile myInstance = null;
    public static IGamerProfile Instance
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
    public IGamerProfile()
    {
        IGamerProfile.myInstance = this;
    }


    //用户档案版本
    public const byte GamerProfileVersion_1 = 1;

    //用户最终存档使用的版本
    public const byte GamerProfileSaveVersion = GamerProfileVersion_1;

    //档案拆分字符
    public const char GamerProfileSplitChar = '#';

    //档案名称
    public const string GamerProfileName = "PlayerProfile";

    public class PlayerData
    {
        //档案是否载入成功
        public bool IsPayerDataLoadSucceed = false;

        //玩家当前的金币数
        public int playerMoney;
        //用户最后使用的角色
        public int playerLastChacterIndex;
        //用户默认的收费项目
        public int defBuyItemId;


        //玩家关卡进度
        public int[] levelProcess = null;
        //获取指定地图当前玩家累计关卡数
        public int AccountLevelTotal(int mapIndex)
        {
            int ret = 0;
            for (int i = 0; i <= mapIndex; i++)
            {
                ret += levelProcess[i];
            }
            if (levelProcess[mapIndex] >=
                IGamerProfile.gameLevel.mapMaxLevel[mapIndex])
            {
                return ret;
            }
            else
            {
                return ret + 1;
            }
            
        }
        //每个角色的存储数据
        public struct PlayerChacterData
        {
            //是否激活
            public bool isactive;
            //当前级别，每过一关，当前使用的龙就增加一级
            public int level;
        }
        public PlayerChacterData[] characterData = null;
        //当前用户的装备数
        public int[] equipCount = new int[GameEquip.EquipMaxCount];
        //当前用户的道具数量
        public int[] skillCount = new int[(int)GameSkill.SkillId.Id_SkillCount];


        public void CreateNewPlayerData()
        {
            playerMoney = IGamerProfile.gameBaseDefine.gameParameter.newPlayer.playermoney;

            playerLastChacterIndex = 0;

            defBuyItemId = (int)IGamerProfile.gameBaseDefine.jewelData.defItemId;


            levelProcess = new int[IGamerProfile.gameLevel.mapData.Length];
            for (int i = 0; i < levelProcess.Length; i++)
            {
                levelProcess[i] = 0;
            }

            characterData = new PlayerChacterData[IGamerProfile.gameCharacter.characterDataList.Length];
            for (int i = 0; i < characterData.Length; i++)
            {
                characterData[i].isactive = false;
                characterData[i].level = 0;
            }
            //默认第一个角色是解锁的
            characterData[0].isactive = true;
            for (int i = 0; i < equipCount.Length; i++)
            {
                equipCount[i] = 0;
            }
            for (int i = 0; i < skillCount.Length; i++)
            {
                skillCount[i] = 0;
            }
            IsPayerDataLoadSucceed = true;
        }
        private bool LoadGamerProfile_Version_1(string[] profileList)
        {
            int index = 1;
            playerMoney = Convert.ToInt32(profileList[index]); index += 1;
            playerLastChacterIndex = Convert.ToInt32(profileList[index]); index += 1;
            defBuyItemId = Convert.ToInt32(profileList[index]); index += 1;

            characterData = new PlayerChacterData[IGamerProfile.gameCharacter.characterDataList.Length];
            for (int i = 0; i < characterData.Length; i++)
            {
                characterData[i] = new PlayerChacterData();
                characterData[i].isactive = Convert.ToInt32(profileList[index]) == 1; index += 1;
                characterData[i].level = Convert.ToInt32(profileList[index]); index += 1;
            }

            levelProcess = new int[IGamerProfile.gameLevel.mapData.Length];
            for (int i = 0; i < levelProcess.Length; i++)
            {
                levelProcess[i] = Convert.ToInt32(profileList[index]); index += 1;
            }
            for (int i = 0; i < equipCount.Length; i++)
            {
                equipCount[i] = Convert.ToInt32(profileList[index]); index += 1;
            }
            for (int i = 0; i < skillCount.Length; i++)
            {
                skillCount[i] = Convert.ToInt32(profileList[index]); index += 1;
            }
            return true;
        }
        public bool LoadGamerProfile(string data)
        {
            try
            {
                //对档案进行拆分
                string[] profileList = data.Split(GamerProfileSplitChar);
                if (profileList.Length <= 1)
                {
                    //提示错误退出，不能初始化为新档案，会导致丢档
                    return false;
                }
                int version = Convert.ToInt32(profileList[0]);
                switch (version)
                {
                    case GamerProfileVersion_1:
                        if (!LoadGamerProfile_Version_1(profileList))
                            return false;
                        break;
                    default:
                        return false;
                }
                IsPayerDataLoadSucceed = true;
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }
        public string SaveGamerProfile()
        {
            StringBuilder buf = new StringBuilder(256);
            buf.Append(GamerProfileSaveVersion); buf.Append(GamerProfileSplitChar);

            buf.Append(playerMoney); buf.Append(GamerProfileSplitChar);
            buf.Append(playerLastChacterIndex); buf.Append(GamerProfileSplitChar);
            buf.Append(defBuyItemId); buf.Append(GamerProfileSplitChar);

            for (int i = 0; i < characterData.Length; i++)
            {
                buf.Append(characterData[i].isactive ? 1 : 0); buf.Append(GamerProfileSplitChar);
                buf.Append(characterData[i].level); buf.Append(GamerProfileSplitChar);
            }
            for (int i = 0; i < levelProcess.Length; i++)
            {
                buf.Append(levelProcess[i]); buf.Append(GamerProfileSplitChar);
            }
            for (int i = 0; i < equipCount.Length; i++)
            {
                buf.Append(equipCount[i]); buf.Append(GamerProfileSplitChar);
            }
            for (int i = 0; i < skillCount.Length; i++)
            {
                buf.Append(skillCount[i]); buf.Append(GamerProfileSplitChar);
            }
            return buf.ToString();
        }
    }

    public PlayerData playerdata = new PlayerData();

    //用户档案是否载入成功
    public override bool IsPlayerDataLoadSucceed
    {
        get
        {
            return playerdata.IsPayerDataLoadSucceed;
        }
    }

    //请求存储一次档案
    public override void SaveGamerProfileToServer()
    {
        //将当前档案数据写入到底层
        string playerProfile = playerdata.SaveGamerProfile();
        GameCenterEviroment.currentGameCenterEviroment.setPlayerParam(GamerProfileName, playerProfile);
    }
    //载入用户档案数据
    public override bool Initialization()
    {
        //对新的档案存储方式进行初始化
        IUniPlayerPrefs.Initialization(typeof(IUniPlayerPrefs));
        
        //读取档案
        string playerProfile = GameCenterEviroment.currentGameCenterEviroment.getPlayerParam(GamerProfileName);
        //没有用户档案，需要对档案初始化
        if (playerProfile.Equals(""))
        {
            playerdata.CreateNewPlayerData();
            //请求存储一次新档案
            SaveGamerProfileToServer();
            return true;
        }
        else if (!playerdata.LoadGamerProfile(playerProfile))
        {
            //档案出错后，直接初始化为新用户好了。这样至少保证用户还可以完嘛
            playerdata.CreateNewPlayerData();
            //请求存储一次新档案
            SaveGamerProfileToServer();
            return true;
        }
        //载入用户档案数据
        return true;
    }

    //当前游戏环境
    public GameEviroment gameEviroment = new GameEviroment();

    //获取最后解锁的地图
    public int getLastLockedMap
    {
        get
        {
            for (int i = 0; i < playerdata.levelProcess.Length; i++)
            {
                if (playerdata.levelProcess[i] < IGamerProfile.gameLevel.mapMaxLevel[i])
                {
                    return i;
                }
            }
            return playerdata.levelProcess.Length - 1;
        }
    }
    //获取最后一个已经解锁的角色
    public int getLastActiveCharacter
    {
        get
        {
            for (int i = playerdata.characterData.Length - 1; i >= 0; i--)
            {
                if (playerdata.characterData[i].isactive)
                    return i;
            }
            return -1;
        }
    }
    //获取第一个未解锁的角色
    public int getFirstUnActiveCharacter
    {
        get
        {
            for (int i = 0; i < playerdata.characterData.Length; i++)
            {
                if (!playerdata.characterData[i].isactive)
                    return i;
            }
            return -1;
        }

    }

    //获取最后选择的角色
    public int getSelectCharacter
    {
        get
        {
            if (playerdata.playerLastChacterIndex == -1)
                return 0;
            return playerdata.playerLastChacterIndex;
        }
    }


    public enum PayMoneyItem
    {
        //解锁角色
        PayMoneyItem_ActiveCharacter,
        //升级角色
        PayMoneyItem_LevelCharacter,
        //宝箱单开
        PayMoneyItem_OneTreasure,
        //宝箱全开
        PayMoneyItem_AllTreasure,
        //释放技能
        PayMoneyItem_Skill,
        //登陆收费
        PayMoneyItem_LoginAward,
    }
    public struct PayMoneyData
    {
        public PayMoneyItem item;
        public int money;
        public int value;
        public delegate void PayMoneyCallback(PayMoneyData paydata, bool isSucceed);
        public PayMoneyCallback callbackFun;
        public PayMoneyData(PayMoneyItem i, int m, int v, PayMoneyCallback fun)
        {
            item = i;
            money = m;
            value = v;
            callbackFun = fun;
        }
    }

    //进行一次收费
    public void PayMoney(PayMoneyData paydata, GuiUiSceneBase callui)
    {
        //检测钱够不够
        if (playerdata.playerMoney < paydata.money)
        {
            //激活收费框
            GuiExtendButtonGroup buttonGroup = callui.GetComponent<GuiExtendButtonGroup>();
            if (buttonGroup != null)
            {
                buttonGroup.IsWorkDo = false;
            }
#if _GameType_BaoYue
            UiSceneNotEnoughMoney em = callui.LoadResource_UIPrefabs("notenougmoney.prefab").GetComponent<UiSceneNotEnoughMoney>();
            em.Initialization(paydata, callui);
#else
            UiSceneRechargeAsk recharge = callui.LoadResource_UIPrefabs("rechargeask.prefab").GetComponent<UiSceneRechargeAsk>();
            recharge.Initialization(paydata, callui);
#endif // _GameType_BaoYue
            return;
        }
        //扣除钱
        playerdata.playerMoney -= paydata.money;
        //回调扣钱成功的函数
        paydata.callbackFun(paydata, true);
    }

    //决策使用那一个默认收费项目
    public int CheckDefBuyItemId(PayMoneyData data)
    {
        int idIndex = playerdata.defBuyItemId;
        //循环到能找到的最大收费条目
        do
        {
            if ((IGamerProfile.gameBaseDefine.jewelData.buyJewelList[idIndex].jewel + playerdata.playerMoney) >= data.money)
            {
                return idIndex;
            }
            idIndex += 1;
        } while (idIndex < IGamerProfile.gameBaseDefine.jewelData.buyJewelList.Length);
        //按照最高收费走
        return (int)IGamerProfile.gameBaseDefine.jewelData.buyJewelList[IGamerProfile.gameBaseDefine.jewelData.buyJewelList.Length - 1].itemId;
    }
    //收费失败了重新调整一次默认收费项目
    public void AjustDefBuyItemId(int currentCheckDefBuyItemId)
    {
        if (currentCheckDefBuyItemId != playerdata.defBuyItemId ||
                playerdata.defBuyItemId == 0)
            return;
        playerdata.defBuyItemId -= 1;
        SaveGamerProfileToServer();
    }
    public override GameCenterEviroment.PlatformChargeIntensity getPlatformChargeIntensity()
    {
        return IGamerProfile.gameBaseDefine.platformChargeIntensityData.defPlatformChargeIntensity;
    }
}
