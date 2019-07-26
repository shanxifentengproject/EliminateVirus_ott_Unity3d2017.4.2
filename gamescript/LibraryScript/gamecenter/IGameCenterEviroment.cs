using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("GameCenter/IGameCenterEviroment")]
class IGameCenterEviroment : GameCenterEviroment
{

    public enum PlayerPayMoneyItem
    {
        //购买钻石收费
        PlayerPayMoney_BuyJewel_1 = 0,
        PlayerPayMoney_BuyJewel_2 = 1,
        PlayerPayMoney_BuyJewel_5 = 2,
        PlayerPayMoney_BuyJewel_10 = 3,
        PlayerPayMoney_BuyJewel_20 = 4,
        PlayerPayMoney_BuyJewel_30 = 5,
        PlayerPayMoney_BuyJewel_50 = 6,
        PlayerPayMoney_BuyJewel_60 = 7,
        PlayerPayMoney_BuyJewel_80 = 8,
        PlayerPayMoney_BuyJewel_100 = 9,
    }
    
    //申请档案对象
    protected override GamerProfile AllocGamerProfile() { return new IGamerProfile(); }
    //游戏中心连接成功
    protected override void OnGameCenterLinkSucceed()
    {
        base.OnGameCenterLinkSucceed();
    }
    //游戏中心连接失败
    protected override void OnGameCenterLinkFailed()
    {
        //这里需要直接退出游戏
        Application.Quit();
    }
    //模拟最高限额
    protected override bool CheckPlayerPayMoneyIsSucceedDevelop(int payid)
    {
        return payid <= IGamerProfile.gameBaseDefine.jewelData.developmaxbuyid;
    }

    //<!--登陆大礼包 effectlevel 从第几关开始有效可见  -1 不限制，首关有效 很大的数值，所有时间都不现实-->
    public static int LoginAwardTimes = 0;      // 登陆大礼包提示次数
    public static bool effectLoginAward
    {
        get
        {
            LoginAwardTimes++;
            //计算当前关卡数
            int level = IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.gameLevel.mapData.Length - 1);

            if ( ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_LoginAward == -1 ) ||
                ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_LoginAward < level ) )
            {
                if ( ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_LoginAward_OpenTimes == -1 ) ||
                    ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_LoginAward_OpenTimes >= LoginAwardTimes ) )
                {
                    return true;
                }
            }
            return false;
        }
    }
    //<!--第几关之前默认都选择第一个免费角色-->
    public static bool effectSelectCharacter
    {
        get
        {
            if ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_SelectCharacter == -1 )
                return true;
            //计算当前关卡数
            int level = IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.gameLevel.mapData.Length - 1);
            return ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_SelectCharacter < level );
        }
    }
    //<!--提示角色需要升级-->
    public static int CharacterLevelSaleTimes = 0;      // 角色升级提示次数
    public static bool effectCharacterLevelSale
    {
        get
        {
            CharacterLevelSaleTimes++;
            //计算当前关卡数
            int level = IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.gameLevel.mapData.Length - 1);

            if ( ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_CharacterLevelSale == -1 ) ||
                ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_CharacterLevelSale < level ) )
            {
                if ( ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_CharacterLevelSale_OpenTimes == -1 ) ||
                    ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_CharacterLevelSale_OpenTimes >= CharacterLevelSaleTimes ) )
                {
                    return true;
                }
            }
            return false;
        }
    }
    //<!--游戏过程收费 多少关前都免费-->
    public static bool effectGameCharge
    {
        get
        {

            if ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_GameCharge == -1 )
                return true;

            //计算当前关卡数
            int level = 0;
            //foreverfree= 0过了阈值之前的关卡也不免费了
            if ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_Foreverfree == 0 )
            {
                level = IGamerProfile.Instance.playerdata.AccountLevelTotal ( IGamerProfile.gameLevel.mapData.Length - 1 );
            }
            //foreverfree= 1过了阈值之前的关卡还免费
            else if ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_Foreverfree == 1 )
            {
                level = IGamerProfile.Instance.playerdata.AccountLevelTotal ( IGamerProfile.Instance.gameEviroment.mapIndex );
            }
            return ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_GameCharge < level );

        }

    }
    //<!--打开宝箱，多少关前免费-->
    public static bool effectTreasure
    {
        get
        {
            if ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_Treasure == -1 )
                return true;
            //计算当前关卡数
            int level = IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.gameLevel.mapData.Length - 1);
            return ( IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_Treasure < level );
        }
    }

}
