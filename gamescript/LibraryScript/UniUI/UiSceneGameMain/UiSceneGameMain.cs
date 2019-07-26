using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneGameMain : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameMain; } }
    public GuiPlaneAnimationText levelNumber;
    public GuiPlaneAnimationText missionCurNumber;
    public GuiPlaneAnimationText missionCompleteNumber;
    public GuiPlaneAnimationText minuteNumber;
    public GuiPlaneAnimationText secondNumber;

    public GuiPlaneAnimationTextRoll playerMoney = null;

    public GameObject[] skillPosition = null;
    private bool[] skillEffect = null;

    public float time_startgameprompt = 2.0f;
    public float time_skillpromptcheck = 5.0f;

    private GameSkill.SkillId currentSkillpromptId;
    private GuiPlaneAnimationPlayer currentSkillpromptmove = null;
    private GameObject currentSkillprompt = null;
    private FTLibrary.Time.TimeLocker skillCheckTimerLock = new FTLibrary.Time.TimeLocker(1000);

    public GuiPlaneAnimationText[] equipNumber = null;
    public GuiPlaneAnimationText[] skillNumber = null;

    public bool IsStartWork
    {
        get
        {
            return ((SceneControlGame)GameRoot.CurrentSceneControl).IsStartWork;
        }
        set
        {
            ((SceneControlGame)GameRoot.CurrentSceneControl).IsStartWork = value;
        }
    }
    protected override void OnInitializationUI()
    {
        levelNumber.Text = string.Format("{0}",IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.Instance.gameEviroment.mapIndex));
        missionCompleteNumber.Text = IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                            levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].missiontarget.ToString();

        playerMoney.SetIntegerRollValue(0, true);
        playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
        Invoke("ShowStartGamePrompt", time_startgameprompt);

        //skillEffect = new bool[skillPosition.Length];
        skillEffect = new bool[3];

        skillCheckTimerLock = new FTLibrary.Time.TimeLocker((int)(time_skillpromptcheck * 1000.0f));
        skillCheckTimerLock.IsLocked = true;


        UpdateMissionNumber();
        UpdateEquipCount();
        UpdateSkillCount();
        UpdateTime();
    }
    private void UpdateMissionNumber()
    {
        missionCurNumber.Text = IGamerProfile.Instance.gameEviroment.killCount.ToString();
       
    }
    public void UpdateEquipCount()
    {
        for (int i = 0; i < equipNumber.Length; i++)
        {
            equipNumber[i].Text = IGamerProfile.Instance.playerdata.equipCount[i].ToString();
        }
    }
    public void UpdateSkillCount()
    {
        for (int i = 0; i < skillNumber.Length; i++)
        {
            skillNumber[i].Text = IGamerProfile.Instance.playerdata.skillCount[i].ToString();
        }
    }
    private bool IsMissionComplete()
    {
        return (IGamerProfile.Instance.gameEviroment.killCount >= IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                            levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].missiontarget);
    }

    private int prveminute = -1;
    private int prveremainsecond = -1;
    private void UpdateTime()
    {
        //计算有多少分钟
        int minute = (int)IGamerProfile.Instance.gameEviroment.useTime / 60;
        //剩余多少秒
        float remainsecond = IGamerProfile.Instance.gameEviroment.useTime - (float)(minute * 60);
        if (prveminute != minute)
        {
            prveminute = minute;
            minuteNumber.Text = prveminute.ToString();
        }
        if (prveremainsecond != (int)(remainsecond * 1000.0f))
        {
            prveremainsecond = (int)(remainsecond * 1000.0f);
            secondNumber.Text = string.Format("{0:00.000}", remainsecond);
        }
    }
    private void ShowStartGamePrompt()
    {
        //播放游戏开始对象
        GuiPlaneAnimationPlayer player = LoadResource_UIPrefabs("startgameprompt.prefab").GetComponent<GuiPlaneAnimationPlayer>();
        player.DelegateOnPlayEndEvent += StartGamePromptPlayEnd;
    }
    private void StartGamePromptPlayEnd()
    {
        //((SceneControlGame)GameRoot.CurrentSceneControl).Invoke("AujstGunZoom", 2.0f);
        //((SceneControlGame)GameRoot.CurrentSceneControl).AujstGunZoom();
    }

    private void FireSkill()
    {
        if (IGameCenterEviroment.effectGameCharge)
        {
            if (IGamerProfile.Instance.playerdata.skillCount[(int)currentSkillpromptId] >= 1)
            {
                IsStartWork = true;
                ReleaseSkillprompt();

                IGamerProfile.Instance.playerdata.skillCount[(int)currentSkillpromptId] -= 1;
                UpdateSkillCount();
                //需要存档
                IGamerProfile.Instance.SaveGamerProfileToServer();
                //释放这个技能
                EffectSkill(currentSkillpromptId);
                return;
            }
            else
            {
                GameSkill.SkillData data = IGamerProfile.gameSkill.FindSkillData(currentSkillpromptId);
                IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_Skill,
                                               data.oncemoney * data.oncebuycount,
                                               0,
                                               PayMoneyCallback), this);
            }
        }
        else
        {
            IsStartWork = true;
            ReleaseSkillprompt();
            //释放这个技能
            EffectSkill(currentSkillpromptId);
            return;
        }
        
        
    }
    private void PayMoneyCallback(IGamerProfile.PayMoneyData paydata, bool isSucceed)
    {
        switch (paydata.item)
        {
            case IGamerProfile.PayMoneyItem.PayMoneyItem_Skill:
                {
                    IsStartWork = true;
                    ReleaseSkillprompt();
                    if (!isSucceed)
                    {
                        //如果技能释放失败了就不要提示后面的了
                        skillCheckTimerLock = new FTLibrary.Time.TimeLocker(int.MaxValue);
                        skillCheckTimerLock.IsLocked = true;
                        return;
                    }
                    //增加技能数量
                    GameSkill.SkillData data = IGamerProfile.gameSkill.FindSkillData(currentSkillpromptId);
                    IGamerProfile.Instance.playerdata.skillCount[(int)currentSkillpromptId] += data.oncebuycount;
                    IGamerProfile.Instance.playerdata.skillCount[(int)currentSkillpromptId] -= 1;
                    UpdateSkillCount();
                    playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
                    //需要存档
                    IGamerProfile.Instance.SaveGamerProfileToServer();
                    //释放这个技能
                    EffectSkill(currentSkillpromptId);
                }
                break;

        }
    }

    public void AddFireCount()
    {
        if (!IsStartWork)
            return;
        IGamerProfile.Instance.gameEviroment.fireCount += 1;
    }
    public void AddKillCount()
    {
        if (!IsStartWork)
            return;
        IGamerProfile.Instance.gameEviroment.killCount += 1;
        //给用户加钱
        IGamerProfile.Instance.playerdata.playerMoney += IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                    levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].oneemenymoney;

        IGamerProfile.Instance.SaveGamerProfileToServer();
        playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);

        UpdateMissionNumber();
        if (IsMissionComplete())
        {
            //任务完成
            LevelComplete(true);
        }
    }
    protected virtual void Update()
    {
        if (IsStartWork)
        {
            IGamerProfile.Instance.gameEviroment.useTime -= Time.deltaTime;
            UpdateTime();
            if (IGamerProfile.Instance.gameEviroment.useTime <= 0.0f)
            {
                //任务失败
                LevelComplete(false);
            }
            UpdateCheckSkillprompt();
        }
    }

    private UiSceneGameOver.Result result;
    private void LevelComplete(bool iswin)
    {
        //停止工作
        IsStartWork = false;
        result = iswin ? UiSceneGameOver.Result.Result_Win : UiSceneGameOver.Result.Result_Faile;
        //延迟显示结算界面
        Invoke("OnShowAwardFace", 0.5f);
        
    }
    private void OnShowAwardFace()
    {
        SoundEffectPlayer.Play("buttonok.wav");
        //显示关卡完成字样
        GuiPlaneAnimationPlayer gameoverprompt = null;
        if (result == UiSceneGameOver.Result.Result_Win)
        {
            gameoverprompt = LoadResource_UIPrefabs("gamewinprompt.prefab").GetComponent<GuiPlaneAnimationPlayer>();
        }
        else
        {
            gameoverprompt = LoadResource_UIPrefabs("gamefaileprompt.prefab").GetComponent<GuiPlaneAnimationPlayer>();
        }
        gameoverprompt.Stop();
        gameoverprompt.Play();
        gameoverprompt.DelegateOnPlayEndEvent += GameOverPromptPlayEnd;
        //完成关卡后显示奖励获取界面
        //LoadResource_UIPrefabs("gameaward.prefab");
    }
    private void GameOverPromptPlayEnd()
    {
        if (IGameCenterEviroment.effectTreasure)
        {
            //完成关卡后显示奖励获取界面
            LoadResource_UIPrefabs("gameaward.prefab");
        }
        else
        {
            OnShowLevelCompleteFace();
        }
    }
    public void OnShowLevelCompleteFace()
    {
        //进入结算界面
        ((UiSceneUICamera)UIManager).CreateAloneScene(UiSceneUICamera.UISceneId.Id_UIGameOver,result);
    }


    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate() 
    { 
        if (InputDevice.ButtonBack)
        {
            SoundEffectPlayer.Play("buttonok.wav");
            //让本身停止工作
            IsStartWork = false;
            LoadResource_UIPrefabs("gameback.prefab");
        }
        else if (GameCenterEviroment.platformChargeIntensity >= GameCenterEviroment.PlatformChargeIntensity.Intensity_VeryVeryHigh &&
                    (InputDevice.ButtonPressOk ||
                    InputDevice.ButtonPressLeft ||
                    InputDevice.ButtonPressRight ||
                    InputDevice.ButtonPressUp ||
                    InputDevice.ButtonPressDown) &&
                    IsStartWork && 
                    currentSkillprompt != null && 
                    currentSkillpromptmove == null)
        {
            //停止工作
            IsStartWork = false;
            FireSkill();
            return false;

        }
        else if (InputDevice.ButtonOk && IsStartWork && currentSkillprompt != null && currentSkillpromptmove == null)
        {
            //停止工作
            IsStartWork = false;
            FireSkill();
            return false;
        }
        //测试
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            LevelComplete(true);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            LevelComplete(false);
        }
        return true; 
    }
    //一个子UI删除
    public override void OnChildUIRelease(GuiUiSceneBase ui)
    {
        if (ui is UiSceneGameBack)
        {
            //重新开始工作
            IsStartWork = true;
        }
    }


    private int IsHaveFreeSkill()
    {
        for (int i = 0; i < skillEffect.Length; i++)
        {
            if (!skillEffect[i])
                return i;
        }
        return -1;
    }

    private void UpdateCheckSkillprompt()
    {
        if (skillCheckTimerLock.IsLocked)
            return;
        skillCheckTimerLock.IsLocked = true;
        if (currentSkillpromptmove != null || currentSkillprompt != null)
            return;
        int index = IsHaveFreeSkill();
        if (index == -1)
            return;
        currentSkillpromptId = (GameSkill.SkillId)index;

        currentSkillprompt = LoadResource_UIPrefabs(string.Format("skillprompt{0}.prefab", index + 1));
        currentSkillpromptmove = LoadResource_UIPrefabs("skillpromptmovein.prefab").GetComponent<GuiPlaneAnimationPlayer>();
        currentSkillprompt.transform.parent = currentSkillpromptmove.transform;
        currentSkillpromptmove.Stop();
        currentSkillpromptmove.Play();
        currentSkillpromptmove.DelegateOnPlayEndEvent += CurrentSkillPromptMoveIn;
    }
    private void CurrentSkillPromptMoveIn()
    {
        currentSkillprompt.transform.parent = currentSkillpromptmove.transform.parent;
        UnityEngine.Object.DestroyObject(currentSkillpromptmove.gameObject);
        currentSkillpromptmove = null;
    }
    private void ReleaseSkillprompt()
    {
        currentSkillpromptmove = LoadResource_UIPrefabs("skillpromptmoveout.prefab").GetComponent<GuiPlaneAnimationPlayer>();
        currentSkillprompt.transform.parent = currentSkillpromptmove.transform;
        currentSkillpromptmove.Stop();
        currentSkillpromptmove.Play();
        currentSkillpromptmove.DelegateOnPlayEndEvent += CurrentSkillPromptMoveout;
    }
    private void CurrentSkillPromptMoveout()
    {
        UnityEngine.Object.DestroyObject(currentSkillpromptmove.gameObject);
        currentSkillpromptmove = null;
        currentSkillprompt = null;
    }
    private void EffectSkill(GameSkill.SkillId id)
    {
        skillEffect[(int)id] = true;
        //播放释放技能的光效
        //GameObject obj = LoadResource_UIPrefabs("skillpanelfire.prefab");
        //obj.transform.position = skillPosition[(int)id].transform.position;
        //obj = LoadResource_UIPrefabs("skillpanelstatus.prefab");
        //obj.transform.position = skillPosition[(int)id].transform.position;
        //释放人物身上的技能释放光效
        //释放人物身上的技能持续光效。
    }
}
