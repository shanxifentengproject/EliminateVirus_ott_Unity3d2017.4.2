using System;
using System.Collections.Generic;
using UnityEngine;

class UiSceneGameClose : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameClose; } }
    protected override void OnInitializationUI()
    {
        GuiExtendDialog dlg = GetComponent<GuiExtendDialog>();
        if (dlg != null)
        {
            dlg.callbackFuntion += OnDialogReback;
            dlg.buttonSelectStatus = ( GuiExtendDialog.DialogFlag ) IGamerProfile.gameBaseDefine.platformChargeIntensityData.closeLevel_ExitGameDlg_BtnIndex;
        }
    }
    private void OnDialogReback(int dialogid, GuiExtendDialog.DialogFlag ret)
    {
        switch (ret)
        {
            case GuiExtendDialog.DialogFlag.Flag_Cancel:
                {
                    UnityEngine.Object.DestroyObject(this.gameObject);
                }
                break;
            case GuiExtendDialog.DialogFlag.Flag_Ok:
                {
                    Application.Quit();
                }
                break;
        }

    }
    //需要重载Input刷新函数
    //如果返回true,表示可以继续刷新后面的对象，否则刷新处理会被截断
    public override bool OnInputUpdate()
    {
        if (InputDevice.ButtonBack)
        {
            OnDialogReback(0, GuiExtendDialog.DialogFlag.Flag_Cancel);
            return false;
        }
        return true;
    }
}
