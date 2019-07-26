using System;
using System.Collections.Generic;
using UnityEngine;


class UiSceneGameHelp : GuiUiSceneBase
{
    public override int uiSceneId { get { return (int)UiSceneUICamera.UISceneId.Id_UIGameHelp; } }
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
        UnityEngine.Object.DestroyObject(this.gameObject);
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

