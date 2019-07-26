using UnityEngine;
using System.Collections;
using UnityEngine;

class UiSceneNotEnoughMoney : GuiUiSceneBase 
{

    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UINotEnoughMoney; } }
    protected override void OnInitializationUI()
    {
        GuiExtendDialog dlg = GetComponent<GuiExtendDialog>();
        if (dlg != null)
        {
            dlg.callbackFuntion += OnDialogReback;
        }
    }
    private void OnDialogReback(int dialogid, GuiExtendDialog.DialogFlag ret)
    {
        switch (ret)
        {
            case GuiExtendDialog.DialogFlag.Flag_Ok:
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
        }

    }
    //当前的游戏内付款数据
    private IGamerProfile.PayMoneyData currentPayMoneyData;
    private GuiUiSceneBase currentCallui;
    public void Initialization(IGamerProfile.PayMoneyData paydata, GuiUiSceneBase callui)
    {
        currentPayMoneyData = paydata;
        currentCallui = callui;
    }

    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate()
    {
        if (InputDevice.Instance.InputP1.ButtonBack)
        {
            //OnDialogReback(0, GuiExtendDialog.DialogFlag.Flag_Ok);
            return false;
        }
        return true;
    }
}
