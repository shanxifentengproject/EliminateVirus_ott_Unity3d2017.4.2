using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneGameOver : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameOver; } }

    public GameObject winTitle;
    public GameObject faileTitle;

    public GameObject winAward;
    public GameObject faileAward;

    public GameObject newMap;

    public GameObject btn_nextlevel;
    public GameObject btn_restart;

    public GuiPlaneAnimationProgressBar levelprogressbar;
    public GuiPlaneAnimationProgressBar gunprogressbar;

    public GuiPlaneAnimationText levelnumber;
    public GuiPlaneAnimationText firenumber;
    public GuiPlaneAnimationText killerumber;
    public GuiPlaneAnimationText hitratioumber;
    public GuiPlaneAnimationText timeminute;
    public GuiPlaneAnimationText timesecond;


    public GuiPlaneAnimationText levelProcess_Cur;
    public GuiPlaneAnimationText levelProcess_Max;

    public GuiPlaneAnimationText gunProcess_Cur;
    public GuiPlaneAnimationText gunProcess_Max;

    public GuiPlaneAnimationText awardMoney;

    public enum Result
    {
        Result_Win,
        Result_Faile,
    }
    private Result result = Result.Result_Win;
    //设置传递的参数
    public override void SetTransferParameter(params object[] args)
    {
        result = (Result)args[0];
    }
    private GuiExtendButtonGroup buttonGroup = null;
    protected override void OnInitializationUI()
    {
        buttonGroup = GetComponent<GuiExtendButtonGroup>();
        buttonGroup.selectFuntion += OnButtonSelectOk;
        buttonGroup.onDialogCloseFuntion += OnDialogClose;

        if (result == Result.Result_Win)
        {
            winTitle.SetActive(true);
            faileTitle.SetActive(false);

            winAward.SetActive(true);
            faileAward.SetActive(false);

            btn_nextlevel.SetActive(true);
            btn_restart.SetActive(false);

            int money = IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                        levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].winawardmoney;
            IGamerProfile.Instance.playerdata.playerMoney += money;
            awardMoney.Text = money.ToString();
        }
        else if (result == Result.Result_Faile)
        {
            winTitle.SetActive(false);
            faileTitle.SetActive(true);

            winAward.SetActive(false);
            faileAward.SetActive(true);

            btn_nextlevel.SetActive(false);
            btn_restart.SetActive(true);

            int money = IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                       levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].failawardmoney;
            IGamerProfile.Instance.playerdata.playerMoney += money;
            awardMoney.Text = money.ToString();
        }


        //关卡数
        levelnumber.Text = string.Format("{0}",
                            IGamerProfile.Instance.playerdata.AccountLevelTotal(IGamerProfile.Instance.gameEviroment.mapIndex));
        //发生数
        firenumber.Text = IGamerProfile.Instance.gameEviroment.fireCount.ToString();
        //杀敌数
        killerumber.Text = IGamerProfile.Instance.gameEviroment.killCount.ToString();
        //命中率
        float hitrate = (float)IGamerProfile.Instance.gameEviroment.killCount/(float)IGamerProfile.Instance.gameEviroment.fireCount;
        hitratioumber.Text = string.Format("{0}", (int)(hitrate * 100f));
        //使用时间
        float useTime = (float)IGamerProfile.gameLevel.mapData[IGamerProfile.Instance.gameEviroment.mapIndex].
                                    levelData[IGamerProfile.Instance.gameEviroment.mapLevelIndex].time -
                                    IGamerProfile.Instance.gameEviroment.useTime;
        //计算有多少分钟
        int minute = (int)useTime / 60;
        //剩余多少秒
        float remainsecond = useTime - (float)(minute * 60);
        timeminute.Text = minute.ToString();
        timesecond.Text = string.Format("{0}", (int)remainsecond);

        
        
        //更新进度
        IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] += 1;
        if (IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] > 
                IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex])
        {
            IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] = IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex];
        }

        IGamerProfile.Instance.SaveGamerProfileToServer();

        levelProcess_Cur.Text = IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex].ToString();
        levelProcess_Max.Text = IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex].ToString();
        levelprogressbar.SetProgressBar((float)IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] /
                                            (float)IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex]);

        if ((IGamerProfile.Instance.gameEviroment.mapIndex < IGamerProfile.gameLevel.mapData.Length - 1) &&
                (IGamerProfile.Instance.playerdata.levelProcess[IGamerProfile.Instance.gameEviroment.mapIndex] == 
                        IGamerProfile.gameLevel.mapMaxLevel[IGamerProfile.Instance.gameEviroment.mapIndex]))    
        {
            newMap.SetActive(true);
        }
        else
        {
            newMap.SetActive(false);
        }


        gunProcess_Cur.Text = IGamerProfile.Instance.playerdata.characterData[IGamerProfile.Instance.gameEviroment.characterIndex].level.ToString();
        gunProcess_Max.Text = IGamerProfile.gameCharacter.characterDataList[IGamerProfile.Instance.gameEviroment.characterIndex].maxlevel.ToString();
        gunprogressbar.SetProgressBar((float)IGamerProfile.Instance.playerdata.characterData[IGamerProfile.Instance.gameEviroment.characterIndex].level /
                                            (float)IGamerProfile.gameCharacter.characterDataList[IGamerProfile.Instance.gameEviroment.characterIndex].maxlevel);
    }
    private void OnButtonSelectOk(int index)
    {
        SoundEffectPlayer.Play("buttonok.wav");
        if (result == Result.Result_Win)
        {
            UiSceneUICamera.Instance.LoadNextLevel();
        }
        else if (result == Result.Result_Faile)
        {
            //任务失败重新加载当前地图
            UiSceneUICamera.Instance.RestartLoadLevel();
        }
    }

    private void OnDialogClose ( )
    {
        OnButtonSelectOk ( (int)Result.Result_Faile );
    }


}

