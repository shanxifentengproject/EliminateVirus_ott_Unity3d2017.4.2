using System;
using System.Collections.Generic;
using UnityEngine;
class UiSceneSelectGameCharacter : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameCharacter; } }

    public enum SelectCharacterMode
    {
        Mode_IntoGame,
        Mode_RebackGame,
        Mode_NextGame,
    }
    public static SelectCharacterMode selectCharacterMode = SelectCharacterMode.Mode_IntoGame;


    public GuiPlaneAnimationTextRoll playerMoney = null;

    public GameObject processObject;
    public GameObject lockObject;
    public GuiPlaneAnimationText curLevelNumber;
    public GuiPlaneAnimationText maxLevelNumber;
    public GuiPlaneAnimationProgressBar levelProgressBar;

    public GuiPlaneAnimationTextRoll characterAttack = null;
    public GuiPlaneAnimationTextRoll playerExact = null;


    public GameObject[] selectPosition;


    public GuiPlaneAnimationPlayer moveAni;
    public Transform[] gunlist;


    public GameObject btn_intogame;
    public GameObject btn_levelcharacter;
    public GameObject btn_activecharacter;

    public GuiPlaneAnimationText levelcharactermoney;
    public GuiPlaneAnimationText activecharactermoney;

    public GameObject arrowsright;
    public GameObject arrowsleft;

    private int currentSelectGunIndex = -1;
    
    private enum ButtonId_NotActiveCharacter
    {
        Id_ActiveCharacter = 0,
        Id_IntoGame = 1,
        ButtonCount = 2,
    }
    private enum ButtonId_ActiveCharacter
    {
        Id_LevelCharacter = 0,
        Id_IntoGame = 1,
        ButtonCount = 2,
    }
    private enum ButtonId_MaxLevel
    {
        Id_IntoGame = 0,
        ButtonCount = 1,
    }

    private GuiExtendButtonGroup buttonGroup = null;

    //是否没有操作
    private bool isNotHandle = true;


    protected override void OnInitializationUI()
    {
        playerMoney.SetIntegerRollValue(0, true);
        playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
        characterAttack.SetIntegerRollValue(0, true);
        playerExact.SetIntegerRollValue(0, true);
        
        buttonGroup = GetComponent<GuiExtendButtonGroup>();
        buttonGroup.selectFuntion += OnButtonSelectOk;
        //先停止工作
        buttonGroup.IsWorkDo = false;
        //进入游戏，定位到已经解锁的角色上
        if (selectCharacterMode == SelectCharacterMode.Mode_IntoGame)
        {
            if ((GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_High) &&
                    IGameCenterEviroment.effectSelectCharacter)
            {
                //定位到未解锁的角色上
                int index = IGamerProfile.Instance.getFirstUnActiveCharacter;
                if (index != -1)
                {
                    SetCurrentSelectGunIndex(index);
                }
                else
                {
                    SetCurrentSelectGunIndex(IGamerProfile.Instance.getSelectCharacter);
                }
            }
            else
            {
                //定位到最后解锁的地图上
                SetCurrentSelectGunIndex(IGamerProfile.Instance.getSelectCharacter);
            } 
        }
        else if (selectCharacterMode == SelectCharacterMode.Mode_NextGame ||
                selectCharacterMode == SelectCharacterMode.Mode_RebackGame)
        {
            if (IGameCenterEviroment.effectSelectCharacter)
            {
                int index = IGamerProfile.Instance.getFirstUnActiveCharacter;
                if (index != -1)
                {
                    SetCurrentSelectGunIndex(index);
                }
                else
                {
                    SetCurrentSelectGunIndex(IGamerProfile.Instance.getSelectCharacter);
                }
            }
            else
            {
                //定位到最后解锁的地图上
                SetCurrentSelectGunIndex(IGamerProfile.Instance.getSelectCharacter);
            }
        }

    }
    private GuiPlaneAnimationPlayer tempMoveAni = null;
    private void SetCurrentSelectGunIndex(int index)
    {
        SetCurrentSelectGunIndex(index, null);
    }

    private void SetCurrentSelectGunIndex(int index, GuiPlaneAnimationPlayer.OnPlayDelegateEvent callfun)
    {
        if (currentSelectGunIndex == index)
            return;
        buttonGroup.IsWorkDo = false;

        tempMoveAni = ((GameObject)UnityEngine.Object.Instantiate(moveAni.gameObject)).GetComponent<GuiPlaneAnimationPlayer>();
        tempMoveAni.transform.parent = this.transform;
        tempMoveAni.IsAutoDel = false;
        tempMoveAni.playMode = GuiPlaneAnimationPlayer.PlayMode.Mode_PlayOnec;
        tempMoveAni.DelegateOnPlayEndEvent += OnMoveAniPlayEnd;
        if (callfun != null)
        {
            tempMoveAni.DelegateOnPlayEndEvent += callfun;
        }
        //增加关键帧
        GuiPlaneAnimationCurvePosition curvePosition = tempMoveAni.gameObject.GetComponentInChildren<GuiPlaneAnimationCurvePosition>();
        curvePosition.xCurve = UnityEngine.AnimationCurve.EaseInOut(0.0f, gunlist[0].position.x,
                                1.0f, gunlist[0].position.x - gunlist[index].position.x);
        //将对象定位到开始坐标对象
        curvePosition.gameObject.transform.position = gunlist[0].position;
        gunlist[0].transform.parent.parent = curvePosition.gameObject.transform;
        //重新标记索引
        currentSelectGunIndex = index;
        //开始播放
        tempMoveAni.Play();
    }
    private void OnMoveAniPlayEnd()
    {
        //将地图对象移出来
        gunlist[0].transform.parent.parent = tempMoveAni.transform.parent;
        UnityEngine.Object.DestroyObject(tempMoveAni.gameObject);
        tempMoveAni = null;
        //刷新当前选择属性
        UpdateCurrentSelectGunData(currentSelectGunIndex);
    }
    private void UpdateCurrentSelectGunData(int index)
    {
        //角色没有解锁
        if (!IGamerProfile.Instance.playerdata.characterData[index].isactive)
        {
            lockObject.SetActive(true);
            btn_levelcharacter.transform.parent.gameObject.SetActive(false);
            btn_activecharacter.transform.parent.gameObject.SetActive(true);
            //刷新激活角色的钱
            activecharactermoney.Text = IGamerProfile.gameCharacter.characterDataList[index].activemoney.ToString();
            //填充按钮
            buttonGroup.buttonList = new GameObject[(int)ButtonId_NotActiveCharacter.ButtonCount];
            buttonGroup.selectAnchorList = new GuiAnchorObject[(int)ButtonId_NotActiveCharacter.ButtonCount];
            buttonGroup.buttonList[(int)ButtonId_NotActiveCharacter.Id_ActiveCharacter] = btn_activecharacter;
            buttonGroup.selectAnchorList[(int)ButtonId_NotActiveCharacter.Id_ActiveCharacter] = btn_activecharacter.GetComponent<GuiAnchorObject>();
            buttonGroup.buttonList[(int)ButtonId_NotActiveCharacter.Id_IntoGame] = btn_intogame;
            buttonGroup.selectAnchorList[(int)ButtonId_NotActiveCharacter.Id_IntoGame] = btn_intogame.GetComponent<GuiAnchorObject>();

            if ((selectCharacterMode == SelectCharacterMode.Mode_IntoGame) &&
                (GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_High) &&
                        isNotHandle)
            {
                //高收费，用户是进入游戏，而且用户没有自主动过操作
                buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_NotActiveCharacter.Id_IntoGame;
            }
            else if ((selectCharacterMode == SelectCharacterMode.Mode_NextGame) &&
                (GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_Normal) &&
                        isNotHandle)
            {
                //高收费，用户是进入游戏，而且用户没有自主动过操作
                buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_NotActiveCharacter.Id_IntoGame;
            }
            else
            {
                //任何时候都选择需要解锁角色
                buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_NotActiveCharacter.Id_ActiveCharacter;
            }
           
        }
        else if (IGamerProfile.Instance.playerdata.characterData[index].level >= IGamerProfile.gameCharacter.characterDataList[index].maxlevel)
        {
            //级别升满了
            lockObject.SetActive(false);
            btn_levelcharacter.transform.parent.gameObject.SetActive(false);
            btn_activecharacter.transform.parent.gameObject.SetActive(false);

            //填充按钮
            buttonGroup.buttonList = new GameObject[(int)ButtonId_MaxLevel.ButtonCount];
            buttonGroup.selectAnchorList = new GuiAnchorObject[(int)ButtonId_MaxLevel.ButtonCount];
            buttonGroup.buttonList[(int)ButtonId_MaxLevel.Id_IntoGame] = btn_intogame;
            buttonGroup.selectAnchorList[(int)ButtonId_MaxLevel.Id_IntoGame] = btn_intogame.GetComponent<GuiAnchorObject>();
            //只需要选择进入游戏
            buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_MaxLevel.Id_IntoGame;

        }
        else
        {
            lockObject.SetActive(false);
            btn_levelcharacter.transform.parent.gameObject.SetActive(true);
            btn_activecharacter.transform.parent.gameObject.SetActive(false);
            //刷新升级角色的钱
            levelcharactermoney.Text = IGamerProfile.gameCharacter.characterDataList[index].LevelToMoney.GetValue(
                                    IGamerProfile.Instance.playerdata.characterData[index].level).ToString();

            //填充按钮
            buttonGroup.buttonList = new GameObject[(int)ButtonId_ActiveCharacter.ButtonCount];
            buttonGroup.selectAnchorList = new GuiAnchorObject[(int)ButtonId_ActiveCharacter.ButtonCount];
            buttonGroup.buttonList[(int)ButtonId_ActiveCharacter.Id_LevelCharacter] = btn_levelcharacter;
            buttonGroup.selectAnchorList[(int)ButtonId_ActiveCharacter.Id_LevelCharacter] = btn_levelcharacter.GetComponent<GuiAnchorObject>();
            buttonGroup.buttonList[(int)ButtonId_ActiveCharacter.Id_IntoGame] = btn_intogame;
            buttonGroup.selectAnchorList[(int)ButtonId_ActiveCharacter.Id_IntoGame] = btn_intogame.GetComponent<GuiAnchorObject>();

            //设置选择哪个按钮
            if (selectCharacterMode == SelectCharacterMode.Mode_IntoGame)
            {
                //保持原来的选择
                buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_IntoGame;
            }
            else if (selectCharacterMode == SelectCharacterMode.Mode_NextGame ||
                    selectCharacterMode == SelectCharacterMode.Mode_RebackGame)
            {
                if (IGameCenterEviroment.effectSelectCharacter)
                {
                    buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_LevelCharacter;
                }
                else if (!isNotHandle)
                {
                    buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_LevelCharacter;
                }
                else
                {
                    buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_IntoGame;
                }
                
            }
        }
        //刷新角色当前级别
        curLevelNumber.Text = IGamerProfile.Instance.playerdata.characterData[index].level.ToString();
        maxLevelNumber.Text = IGamerProfile.gameCharacter.characterDataList[index].maxlevel.ToString();
        levelProgressBar.SetProgressBar((float)IGamerProfile.Instance.playerdata.characterData[index].level /
                        (float)IGamerProfile.gameCharacter.characterDataList[index].maxlevel);

        //刷新角色当前级别的攻击力和命中率
        characterAttack.SetIntegerRollValue(IGamerProfile.gameCharacter.characterDataList[index].LevelToAttack.GetValue(
                                        IGamerProfile.Instance.playerdata.characterData[index].level));
        playerExact.SetIntegerRollValue(IGamerProfile.gameCharacter.characterDataList[index].LevelToExact.GetValue(
                                        IGamerProfile.Instance.playerdata.characterData[index].level));
        for (int i = 0; i < selectPosition.Length; i++)
        {
            selectPosition[i].SetActive(false);
        }
        selectPosition[index].SetActive(true);

        if (index == 0)
        {
            arrowsright.SetActive(true);
            arrowsleft.SetActive(false);
        }
        else if (index == IGamerProfile.gameCharacter.characterDataList.Length - 1)
        {
            arrowsright.SetActive(false);
            arrowsleft.SetActive(true);
        }
        else
        {
            arrowsright.SetActive(true);
            arrowsleft.SetActive(true);
        }

        buttonGroup.IsWorkDo = true;
    }

    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate()
    {
        if (InputDevice.ButtonLeft && buttonGroup.IsWorkDo)
        {
            SoundEffectPlayer.Play("scroll.wav");
            if (currentSelectGunIndex > 0)
            {
                isNotHandle = false;
                SetCurrentSelectGunIndex(currentSelectGunIndex - 1);
            }
            return false;
        }
        else if (InputDevice.ButtonRight && buttonGroup.IsWorkDo)
        {
            SoundEffectPlayer.Play("scroll.wav");
            if (currentSelectGunIndex < gunlist.Length - 1)
            {
                isNotHandle = false;
                SetCurrentSelectGunIndex(currentSelectGunIndex + 1);
            }
            return false;
        }
        else if (InputDevice.ButtonBack)
        {
            if (selectCharacterMode == SelectCharacterMode.Mode_IntoGame ||
                    selectCharacterMode == SelectCharacterMode.Mode_RebackGame)
            {
                SoundEffectPlayer.Play("buttonok.wav");
                //闪白
                ((UiSceneUICamera)UIManager).FadeScreen();
                //进入角色选择界面
                ((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameStart);
                return false;
            }

        }
        return true;
    }


    private void OnButtonSelectOk(int index)
    {
        SoundEffectPlayer.Play("buttonok.wav");
        //当前角色未解锁
        if (!IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].isactive)
        {
            if (index == (int)ButtonId_NotActiveCharacter.Id_ActiveCharacter)
            {
                //激活角色需要检测这个角色之前是否有未激活的角色，如果有就需要移动到这个角色
                int lastNeedActiveCharacter = IGamerProfile.Instance.getLastActiveCharacter + 1;
                if (lastNeedActiveCharacter < currentSelectGunIndex)
                {
                    isNotHandle = true;
                    SetCurrentSelectGunIndex(lastNeedActiveCharacter);
                    return;
                }
                else
                {
                    //请求解锁角色
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_ActiveCharacter,
                                                IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].activemoney,
                                                0,
                                                PayMoneyCallback), this);
                }
            }
            else if (index == (int)ButtonId_NotActiveCharacter.Id_IntoGame)
            {
                //激活角色需要检测这个角色之前是否有未激活的角色，如果有就需要移动到这个角色
                int lastNeedActiveCharacter = IGamerProfile.Instance.getLastActiveCharacter + 1;
                if (lastNeedActiveCharacter < currentSelectGunIndex)
                {
                    isNotHandle = true;
                    SetCurrentSelectGunIndex(lastNeedActiveCharacter);
                    return;
                }
                else
                {

                    //请求解锁角色
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_ActiveCharacter,
                                                IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].activemoney,
                                                0,
                                                PayMoneyCallback), this);
                }
            }
        }
        else if (IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level >= IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].maxlevel)
        {
            if (index == (int)ButtonId_MaxLevel.Id_IntoGame)
            {
                IntoGame();
            }
        }
        else
        {
            if (index == (int)ButtonId_ActiveCharacter.Id_LevelCharacter)
            {
                //请求升级角色
                IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_LevelCharacter,
                                            IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].LevelToMoney.GetValue(
                                                    IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level),
                                            0,
                                            PayMoneyCallback), this);
            }
            else if (index == (int)ButtonId_ActiveCharacter.Id_IntoGame)
            {
                IntoGame();
            }
        }
    }
    private void PayMoneyCallback(IGamerProfile.PayMoneyData paydata, bool isSucceed)
    {
        switch (paydata.item)
        {
            case IGamerProfile.PayMoneyItem.PayMoneyItem_ActiveCharacter:
                {
                    //刷新用户钱
                    playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
                    //如果失败就什么也做
                    if (!isSucceed)
                    {
                        //解锁角色收费失败则恢复到之前选择角色
                        if (selectCharacterMode == SelectCharacterMode.Mode_NextGame)
                        {
                            SetCurrentSelectGunIndex(IGamerProfile.Instance.getLastActiveCharacter, NextGameActiveCharacterFaile);

                        }
                        return;
                    }
                    //标记当前角色解锁
                    IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].isactive = true;
                    //存储档案
                    IGamerProfile.Instance.SaveGamerProfileToServer();

                    //如果选择的按钮是开始游戏则说明用户在进入游戏的时候请求解锁了，直接进入游戏
                    if (buttonGroup.CurrentSelectButtonIndex == (int)ButtonId_NotActiveCharacter.Id_IntoGame)
                    {
                        IntoGame();
                        return;
                    }
                    //刷新一次当前角色信息
                    UpdateCurrentSelectGunData(currentSelectGunIndex);
                    //播放一个触发光效
                    GameObject obj = LoadResource_UIPrefabs("SelectChracterEffect.prefab");
                    obj.transform.parent = this.transform;
                    //这里需要补充按钮选择功能
                    if (selectCharacterMode == SelectCharacterMode.Mode_IntoGame ||
                                selectCharacterMode == SelectCharacterMode.Mode_RebackGame)
                    {
                        buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_LevelCharacter;
                    }
                    else if (selectCharacterMode == SelectCharacterMode.Mode_NextGame)
                    {
                        buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_IntoGame;
                    }

                }
                break;
            case IGamerProfile.PayMoneyItem.PayMoneyItem_LevelCharacter:
                {
                    //刷新用户钱
                    playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
                    //如果失败就什么也做
                    if (!isSucceed)
                        return;
                    //增加角色级别
                    IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level += 1;
                    if (IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level > IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].maxlevel)
                    {
                        IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level = IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].maxlevel;
                    }
                    //存储档案
                    IGamerProfile.Instance.SaveGamerProfileToServer();
                    //刷新一次当前角色信息
                    UpdateCurrentSelectGunData(currentSelectGunIndex);
                    //播放一个触发光效
                    GameObject obj = LoadResource_UIPrefabs("SelectChracterEffect.prefab");
                    obj.transform.parent = this.transform;
                    //这里需要补充按钮选择功能
                    if (IGamerProfile.Instance.playerdata.characterData[currentSelectGunIndex].level <
                            IGamerProfile.gameCharacter.characterDataList[currentSelectGunIndex].maxlevel)
                    {
                        if (selectCharacterMode == SelectCharacterMode.Mode_IntoGame ||
                                selectCharacterMode == SelectCharacterMode.Mode_RebackGame)
                        {
                            buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_LevelCharacter;
                        }
                        else if (selectCharacterMode == SelectCharacterMode.Mode_NextGame)
                        {
                            buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_IntoGame;
                        }
                    }
                }
                break;
        }
    }
    private void NextGameActiveCharacterFaile()
    {
        buttonGroup.CurrentSelectButtonIndex = (int)ButtonId_ActiveCharacter.Id_IntoGame;
    }
    private void IntoGame()
    {

        //保持当前选择的角色索引
        IGamerProfile.Instance.playerdata.playerLastChacterIndex = currentSelectGunIndex;
        IGamerProfile.Instance.SaveGamerProfileToServer();

        //设置当前地图
        IGamerProfile.Instance.gameEviroment.characterIndex = currentSelectGunIndex;
        if (selectCharacterMode == SelectCharacterMode.Mode_NextGame)
        {
            //进入加载界面
            ((UiSceneUICamera)UIManager).CreatePoolingScene(UiSceneUICamera.UISceneId.Id_UIGameLoading, UiSceneGameLoading.LoadingType.Type_LoadingGameNew);
            //删除主UI界面
            ((UiSceneUICamera)UIManager).ReleaseAloneScene();

        }
        else
        {
            //闪白
            ((UiSceneUICamera)UIManager).FadeScreen();
            //进入角色选择界面
            ((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameMap);
        }
    }

    //一个子UI删除
    public override void OnChildUIRelease(GuiUiSceneBase ui)
    {
        if (ui is UiSceneRechargeAsk)
        {
            //重新设置会光标
            buttonGroup.IsWorkDo = true;
        }
    }
}






