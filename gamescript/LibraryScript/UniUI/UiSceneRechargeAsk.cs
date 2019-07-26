using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneRechargeAsk : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameRechargeAsk; } }
    public GuiPlaneAnimationText rmbNumber;
    public GuiPlaneAnimationText moneyNumber;
    protected override void OnInitializationUI()
    {
        GuiExtendDialog dlg = GetComponent<GuiExtendDialog>();
        if (dlg != null)
        {
            dlg.callbackFuntion += OnDialogReback;
            dlg.buttonSelectStatus = ( GuiExtendDialog.DialogFlag ) IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_RechargeAsk_BtnIndex;
        }
    }
    private void OnDialogReback(int dialogid, GuiExtendDialog.DialogFlag ret)
    {
        switch (ret)
        {
            case GuiExtendDialog.DialogFlag.Flag_Cancel:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    UnityEngine.Object.DestroyObject(this.gameObject);
                    GuiExtendButtonGroup buttonGroup = currentCallui.GetComponent<GuiExtendButtonGroup>();
                    if (buttonGroup != null)
                    {
                        buttonGroup.IsWorkDo = true;
                    }
                    //放弃充值，直接返回
                    currentPayMoneyData.callbackFun(currentPayMoneyData, false);
                }
                break;
            case GuiExtendDialog.DialogFlag.Flag_Ok:
                {
                    SoundEffectPlayer.Play("buttonok.wav");
                    IGameCenterEviroment.currentGameCenterEviroment.OpenPlayerPayMoney(rechargeId, RechargePayCallback);
                }
                break;
        }

    }
    //当前的游戏内付款数据
    private IGamerProfile.PayMoneyData currentPayMoneyData;
    private GuiUiSceneBase currentCallui;
    //当前充值ID
    private int rechargeId;
    public void Initialization(IGamerProfile.PayMoneyData paydata, GuiUiSceneBase callui)
    {
        currentPayMoneyData = paydata;
        currentCallui = callui;
        //获取使用的充值ID
        rechargeId = IGamerProfile.Instance.CheckDefBuyItemId(currentPayMoneyData);
        rmbNumber.Text = IGamerProfile.gameBaseDefine.jewelData.buyJewelList[rechargeId].rmb.ToString();
        moneyNumber.Text = IGamerProfile.gameBaseDefine.jewelData.buyJewelList[rechargeId].jewel.ToString();
    }
    //付费成功回调
    private void RechargePayCallback(int payid, bool issucceed)
    {
        GuiExtendButtonGroup buttonGroup = currentCallui.GetComponent<GuiExtendButtonGroup>();
        if (buttonGroup != null)
        {
            buttonGroup.IsWorkDo = true;
        }
        if (!issucceed)
        {
            //充值失败
            //重新调整充值策略
            IGamerProfile.Instance.AjustDefBuyItemId(payid);
            //删除对象
            UnityEngine.Object.DestroyObject(this.gameObject);
            //回调充值失败
            currentPayMoneyData.callbackFun(currentPayMoneyData, false);
            return;
        }
        //给用户加钱
        IGamerProfile.Instance.playerdata.playerMoney += IGamerProfile.gameBaseDefine.jewelData.buyJewelList[rechargeId].jewel;
        IGamerProfile.Instance.SaveGamerProfileToServer();
        //删除对象
        UnityEngine.Object.DestroyObject(this.gameObject);
        //重新尝试付款
        IGamerProfile.Instance.PayMoney(currentPayMoneyData, currentCallui);
    }
}

