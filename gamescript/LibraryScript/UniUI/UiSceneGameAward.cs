using System;
using System.Collections.Generic;
using UnityEngine;


class UiSceneGameAward : GuiUiSceneBase
{
    public GameObject[] boxposition = null;
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameAward; } }
    public GuiPlaneAnimationText oneopenMoney;
    public GuiPlaneAnimationText allopenMoney;


    private enum ButtonId
    {
        Id_Cancel = 0,
        Id_OpenOne = 1,
        Id_OpenAll = 2,
    }
    private bool[] boxisopen = null;
    private bool IsAllBoxOpen
    {
        get
        {
            for (int i = 0; i < boxisopen.Length; i++)
            {
                if (!boxisopen[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    private class OpenBoxData
    {
        public UiSceneGameAward parent;
        public int boxIndex;
        public void OnPlayEventEnd()
        {
            parent.OnPlayOpenBoxEffectEnd(boxIndex);
        }
    }


    private GameObject[] awardIcon = null;
    protected override void OnInitializationUI()
    {
        GuiExtendButtonGroup g = GetComponent<GuiExtendButtonGroup>();
        g.selectFuntion += OnButtonSelectOk;
        //g.CurrentSelectButtonIndex = g.selectAnchorList.Length - 1;
        g.CurrentSelectButtonIndex = IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_Treasure_BtnIndex;


        oneopenMoney.Text = IGamerProfile.gameBaseDefine.gameParameter.treasure.openonce.ToString();
        allopenMoney.Text = IGamerProfile.gameBaseDefine.gameParameter.treasure.openall.ToString();


        boxisopen = new bool[boxposition.Length];
        for (int i = 0; i < boxisopen.Length;i++ )
            boxisopen[i] = false;

        awardIcon = new GameObject[boxposition.Length];
        for (int i = 0; i < awardIcon.Length; i++)
            awardIcon[i] = null;
    }
    private void OnButtonSelectOk(int index)
    {
        switch (index)
        {
            case (int)ButtonId.Id_Cancel:
                SoundEffectPlayer.Play("buttonok.wav");
                GotoNext();
                break;
            case (int)ButtonId.Id_OpenOne:
                
                //如果所有宝箱正在开启就不要响应了
                if (!IsAllBoxOpen)
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_OneTreasure,
                                            IGamerProfile.gameBaseDefine.gameParameter.treasure.openonce,
                                            0,
                                            PayMoneyCallback), this);
                }
                break;
            case (int)ButtonId.Id_OpenAll:
                
                if (!IsAllBoxOpen)
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    IGamerProfile.Instance.PayMoney(new IGamerProfile.PayMoneyData(IGamerProfile.PayMoneyItem.PayMoneyItem_AllTreasure,
                                            IGamerProfile.gameBaseDefine.gameParameter.treasure.openall,
                                            0,
                                            PayMoneyCallback), this);
                }
                break;
        }
    }
    private void PayMoneyCallback(IGamerProfile.PayMoneyData paydata, bool isSucceed)
    {
        switch (paydata.item)
        {
            case IGamerProfile.PayMoneyItem.PayMoneyItem_OneTreasure:
                {
                    if (!isSucceed)
                    {
                        //就选择到第一个去
                        GuiExtendButtonGroup g = GetComponent<GuiExtendButtonGroup>();
                        g.CurrentSelectButtonIndex = (int)ButtonId.Id_Cancel;
                        return;
                    }
                    int index = -1;
                    for (int i = 0; i < boxisopen.Length; i++)
                    {
                        if (!boxisopen[i])
                        {
                            index = i;
                        }
                    }
                    if (index == -1)
                        return;
                    boxisopen[index] = true;
                    GameObject obj = LoadResource_UIPrefabs("OpenBoxEffect.prefab");
                    obj.transform.localPosition = boxposition[index].transform.localPosition;
                    GuiPlaneAnimationPlayer pl = obj.GetComponent<GuiPlaneAnimationPlayer>();
                    OpenBoxData data = new OpenBoxData();
                    data.parent = this;
                    data.boxIndex = index;
                    pl.DelegateOnPlayEndEvent = data.OnPlayEventEnd;
                }
                break;
            case IGamerProfile.PayMoneyItem.PayMoneyItem_AllTreasure:
                {
                    if (!isSucceed)
                    {
                        //就选择到第一个去
                        GuiExtendButtonGroup g = GetComponent<GuiExtendButtonGroup>();
                        g.CurrentSelectButtonIndex = (int)ButtonId.Id_Cancel;
                        return;
                    }
                    //标记为全开
                    for (int i = 0; i < boxisopen.Length; i++)
                        boxisopen[i] = true;
                    //先播放光效
                    GameObject obj = null;
                    for (int i = 0; i < boxposition.Length; i++)
                    {
                        obj = LoadResource_UIPrefabs("OpenBoxEffect.prefab");
                        obj.transform.localPosition = boxposition[i].transform.localPosition;
                    }
                    if (obj != null)
                    {
                        GuiPlaneAnimationPlayer pl = obj.GetComponent<GuiPlaneAnimationPlayer>();
                        pl.DelegateOnPlayEndEvent = OnEventOpenAllBoxEffectPlayEnd;
                    }
                }
                break;
        }
    }
    private void OnPlayOpenBoxEffectEnd(int index)
    {
        if (awardIcon[index] != null)
        {
            UnityEngine.Object.DestroyObject(awardIcon[index]);
            awardIcon[index] = null;
        }
        GameBaseDefine.GameParameter.Treasure.AwardValueData awardData = IGamerProfile.gameBaseDefine.gameParameter.treasure.RandomAwardValueData();
        switch(awardData.type)
        {
            case GameBaseDefine.GameParameter.Treasure.AwardType.AwardType_Buttle:
                {
                    awardIcon[index] = LoadResource_UIPrefabs("buttleicon.prefab");
                    GuiPlaneAnimationText num = awardIcon[index].GetComponentInChildren<GuiPlaneAnimationText>();
                    num.Text = awardData.value.ToString();
                    awardIcon[index].transform.localPosition = boxposition[index].transform.localPosition;
                    //awardIcon[index].transform.localScale = boxposition[index].transform.localScale;
                    //修改档案
                    //IGamerProfile.Instance.playerdata.playerButtle += awardData.value;
                    IGamerProfile.Instance.SaveGamerProfileToServer();

                    UiSceneGameMain uiMain = this.transform.parent.GetComponent<UiSceneGameMain>();
                    //uiMain.playerButtle.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerButtle);
                }
                break;
            case GameBaseDefine.GameParameter.Treasure.AwardType.AwardType_Money:
                {
                    awardIcon[index] = LoadResource_UIPrefabs("moneyicon.prefab");
                    GuiPlaneAnimationText num = awardIcon[index].GetComponentInChildren<GuiPlaneAnimationText>();
                    num.Text = awardData.value.ToString();
                    awardIcon[index].transform.localPosition = boxposition[index].transform.localPosition;
                    //awardIcon[index].transform.localScale = boxposition[index].transform.localScale;
                    //修改档案
                    IGamerProfile.Instance.playerdata.playerMoney += awardData.value;
                    IGamerProfile.Instance.SaveGamerProfileToServer();

                    UiSceneGameMain uiMain = this.transform.parent.GetComponent<UiSceneGameMain>();
                    uiMain.playerMoney.SetIntegerRollValue(IGamerProfile.Instance.playerdata.playerMoney);
                }
                break;
            case GameBaseDefine.GameParameter.Treasure.AwardType.AwardType_Equip:
                {
                    int equipIndex = FTLibrary.Command.FTRandom.Next(GameEquip.EquipMaxCount);
                    awardIcon[index] = LoadResource_UIPrefabs(string.Format("equip{0}icon.prefab", equipIndex + 1));
                    GuiPlaneAnimationText num = awardIcon[index].GetComponentInChildren<GuiPlaneAnimationText>();
                    num.Text = awardData.value.ToString();
                    awardIcon[index].transform.localPosition = boxposition[index].transform.localPosition;
                    //awardIcon[index].transform.localScale = boxposition[index].transform.localScale;
                    //修改档案
                    IGamerProfile.Instance.playerdata.equipCount[equipIndex] += awardData.value;
                    IGamerProfile.Instance.SaveGamerProfileToServer();

                    UiSceneGameMain uiMain = this.transform.parent.GetComponent<UiSceneGameMain>();
                    uiMain.UpdateEquipCount();
                }
                break;
        }
        if (IsAllBoxOpen)
        {
            Invoke("GotoNext", 2.0f);
        }
    }
    private void OnEventOpenAllBoxEffectPlayEnd()
    {
        for (int i = 0; i < boxposition.Length; i++)
        {
            OnPlayOpenBoxEffectEnd(i);

        }
    }
    private void GotoNext()
    {
        UiSceneGameMain uiMain = this.transform.parent.GetComponent<UiSceneGameMain>();
        uiMain.OnShowLevelCompleteFace();
    }
}

