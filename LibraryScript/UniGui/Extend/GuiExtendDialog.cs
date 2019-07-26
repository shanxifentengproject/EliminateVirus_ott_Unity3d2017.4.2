using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("UniGui/Extend/GuiExtendDialog")]
class GuiExtendDialog : GuiExtendButtonGroup
{
    //按钮的索引
    public int ButtonOkIndex = 0;
    public int ButtonCancelIndex = 1;

    public enum DialogFlag
    {
        Flag_Ok  = 0,
        Flag_Cancel = 1,
    }
    //按钮选择状态
    private DialogFlag selectStatus = DialogFlag.Flag_Ok;
    public DialogFlag SelectStatus
    {
        get { return selectStatus; }
        set
        {
            selectStatus = value;
            if (selectStatus == DialogFlag.Flag_Ok)
            {
                CurrentSelectButtonIndex = ButtonOkIndex;
            }
            else
            {
                CurrentSelectButtonIndex = ButtonCancelIndex;
            }
        }
    }
    public DialogFlag buttonSelectStatus = DialogFlag.Flag_Ok;
    //窗体ID
    public int DialogId = 0;
    public delegate void OnDialogReback(int dialogid,DialogFlag ret);
    public OnDialogReback callbackFuntion = null;
    protected override void Start()
    {
        base.Start();
        selectFuntion += OnButtonSelectOkFun;
        onDialogCloseFuntion += PrivateOnDialogClose;
        SelectStatus = buttonSelectStatus;
    }
    private void PrivateOnDialogClose ( )
    {
        OnButtonSelectOkFun ( ButtonCancelIndex );
    }

    private void OnButtonSelectOkFun(int index)
    {
        if (index == ButtonOkIndex)
        {
            if (callbackFuntion != null)
            {
                callbackFuntion(DialogId,DialogFlag.Flag_Ok);
            }
        }
        else if (index == ButtonCancelIndex)
        {
            if (callbackFuntion != null)
            {
                callbackFuntion(DialogId,DialogFlag.Flag_Cancel);
            }
        }
    }
}
