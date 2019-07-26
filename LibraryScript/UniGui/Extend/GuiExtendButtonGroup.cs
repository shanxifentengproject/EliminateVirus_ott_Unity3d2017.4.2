using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("UniGui/Extend/GuiExtendButtonGroup")]
class GuiExtendButtonGroup : MonoBehaviourInputSupport
{
    //按钮列表,frame 0 未选中  1 选中
    //可以给个空数组
    public GameObject[] buttonList = null;
    //public GuiPlaneAnimationUVAnimation[] buttonList = null;
    protected void SetButtonFrame(int index,int frame)
    {
        if (!isCtrlFrame)
            return;
        if (buttonList == null || buttonList[index] == null)
            return;
        GuiPlaneAnimationUVAnimation uvani = buttonList[index].GetComponent<GuiPlaneAnimationUVAnimation>();
        if (uvani != null)
        {
            uvani.frame = frame;
        }
        
    }
    //是否直接开始工作
    public bool isAutoWork = true;
    //是否控制帧
    public bool isCtrlFrame = true;
    //选中动画对象
    public GuiSelectFlag selectObject = null;
    //选中的锚点对象
    public GuiAnchorObject[] selectAnchorList = null;

    //是支持确定键按下还是弹起响应
    public bool IsUseButtonOkUp = false;
    private bool UseButtonOkUpButtonIsDown = false;

    public delegate void OnDialogClose();
    public OnDialogClose onDialogCloseFuntion = null;

    //当前模块支持操作的玩家位
    public enum SupportPlayerPosition
    {
        Support_P1 = 0,
        Support_P2 = 1,
        Support_P1AndP2 = 3,
    }
    public SupportPlayerPosition supportPlayer = SupportPlayerPosition.Support_P1;
    
    private bool ButtonOkDown
    {
        get
        {
            switch(supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonOkDown;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonOkDown;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonOkDown ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonOkDown;
            }
            return false;
        }
    }
    private bool ButtonOkUp
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonOkUp;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonOkUp;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonOkUp ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonOkUp;
            }
            return false;
        }
    }
    protected bool ButtonOk
    {
        get
        {
            if (IsUseButtonOkUp)
            {
                if (!UseButtonOkUpButtonIsDown)
                {
                    UseButtonOkUpButtonIsDown = ButtonOkDown;
                    return false;
                }
                else if (UseButtonOkUpButtonIsDown && ButtonOkUp)
                {
                    UseButtonOkUpButtonIsDown = false;
                    return true;
                }
                return false;
            }
            else
            {
                return ButtonOkDown;
            }

        }
    }

    protected bool ButtonLeft
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonLeft;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonLeft;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonLeft ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonLeft;
            }
            return false;
        }
    }
    protected bool ButtonRight
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonRight;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonRight;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonRight ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonRight;
            }
            return false;
        }
    }
    protected bool ButtonUp
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonUp;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonUp;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonUp ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonUp;
            }
            return false;
        }
    }
    protected bool ButtonDown
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonDown;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonDown;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonDown ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonDown;
            }
            return false;
        }
    }
    protected bool ButtonBack
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonBack;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonBack;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonBack ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonBack;
            }
            return false;
        }
    }
    protected bool ButtonMenu
    {
        get
        {
            switch (supportPlayer)
            {
                case SupportPlayerPosition.Support_P1:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonMenu;
                case SupportPlayerPosition.Support_P2:
                    return InputDeviceBase.currentInputDevice.InputP2.ButtonMenu;
                case SupportPlayerPosition.Support_P1AndP2:
                    return InputDeviceBase.currentInputDevice.InputP1.ButtonMenu ||
                            InputDeviceBase.currentInputDevice.InputP2.ButtonMenu;
            }
            return false;
        }
    }

    //选择模式
    public enum ButtonSelectMode
    {
        Mode_Nothing = 0,
        Mode_LeftRight = 1,
        Mode_LeftRightRecycle = 2,//选择后会转回来
        Mode_UpDown = 3,
        Mode_UpDownRecycle =4,
        Mode_Rank = 5,
        Mode_RankRecycle = 6,
    }
    //当前使用的模式
    public ButtonSelectMode selectMode = ButtonSelectMode.Mode_LeftRight;
    public ButtonSelectMode selectMode2 = ButtonSelectMode.Mode_Nothing;
    //如果是矩阵选择模式，提供矩阵索引
    public int[][] selectRank = null;
    //当前选择的按钮索引
    private int currentSelectButtonIndex = 0;
    public int CurrentSelectButtonIndex
    {
        get { return currentSelectButtonIndex; }
        set
        {
            if (value >= buttonList.Length || value < 0)
                return;
            currentSelectButtonIndex = value;
            //除了这个按钮外，其他按钮都被设置为非选中模式
            for (int i = 0; i < buttonList.Length; i++)
            {
                if (i == currentSelectButtonIndex)
                {
                    //buttonList[i].frame = 1;
                    SetButtonFrame(i, 1);
                }
                else
                {
                    //buttonList[i].frame = 0;
                    SetButtonFrame(i, 0);
                }
            }
            //如果有选中标记对象，需要移动这个对象
            if (selectObject != null && selectAnchorList != null && selectObject.gameObject.activeSelf)
            {
                selectObject.MoveToAnchor(selectAnchorList[currentSelectButtonIndex]);
            }
        }
    }

    //是否开始工作
    private bool isWorkDo = true;
    public bool IsWorkDo
    {
        get { return isWorkDo; }
        set 
        {
            isWorkDo = value;
            if (isWorkDo)
            {
                if (selectObject != null)
                {
                    selectObject.gameObject.SetActive(true);
                }
                CurrentSelectButtonIndex = CurrentSelectButtonIndex;
            }
            else
            {
                if (selectObject != null)
                {
                    selectObject.gameObject.SetActive(false);
                }
            }
        }
    }


    //4个方向的交接按钮组，实现多按钮组之间的切换
    public GuiExtendButtonGroup leftButtonGroup = null;
    public GuiExtendButtonGroup rightButtonGroup = null;
    public GuiExtendButtonGroup upButtonGroup = null;
    public GuiExtendButtonGroup downButtonGroup = null;



    public delegate void OnButtonSelectOk(int index);
    public OnButtonSelectOk selectFuntion = null;

    protected override void Start()
    {
        base.Start();
        if (selectObject  == null)
        {
            GuiUiSceneBase uiscene = GetComponent<GuiUiSceneBase>();
            if (uiscene != null)
            {
                selectObject = uiscene.AllocSelectFlag();
                if (selectObject != null)
                {
                    selectObject.transform.parent = transform;
                }
            }
        }
        if (isAutoWork)
        {
            IsWorkDo = true;
        }
        else
        {
            CurrentSelectButtonIndex = CurrentSelectButtonIndex;
        }
#if PLATFORM_CYBER
		IsUseButtonOkUp = true;
#endif
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (selectObject != null)
        {
            selectObject.gameObject.SetActive(false);
        }
    }
    private void getRankModePosition(out int x,out int y)
    {
        x = 0;
        y = 0;
        if (selectRank == null)
            throw new Exception("GuiExtendButtonGroup.selectRank = null?");
        for (y = 0;y<selectRank.Length;y++)
        {
            for (x = 0;x<selectRank[y].Length;x++)
            {
                if (selectRank[y][x] == CurrentSelectButtonIndex)
                    return;
            }
        }
    }
    private bool setRankModePosition(int x,int y)
    {
        if (selectRank == null)
            throw new Exception("GuiExtendButtonGroup.selectRank = null?");
        if (y < 0 || y >= selectRank.Length)
            return false;
        if (x < 0 || x >= selectRank[y].Length)
            return false;
        CurrentSelectButtonIndex = selectRank[y][x];
        return true;
    }
    public override bool OnInputUpdate()
    {
        if (!IsWorkDo)
            return true;
        if (this.ButtonLeft)
        {
            if (selectMode == ButtonSelectMode.Mode_Rank)
            {
                int x,y;
                getRankModePosition(out x, out y);
                if (!setRankModePosition(x - 1, y))
                {
                    if (leftButtonGroup != null)
                    {
                        IsWorkDo = false;
                        leftButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_RankRecycle)
            {
                int x, y;
                getRankModePosition(out x, out y);
                x -= 1;
                if (x < 0)
                {
                    x = selectRank[y].Length - 1;
                }
                setRankModePosition(x, y);
            }
            else if (selectMode == ButtonSelectMode.Mode_LeftRight ||
                        selectMode2 == ButtonSelectMode.Mode_LeftRight)
            {
                if (CurrentSelectButtonIndex > 0)
                {
                    CurrentSelectButtonIndex -= 1;
                }
                else
                {
                    if (leftButtonGroup != null)
                    {
                        IsWorkDo = false;
                        leftButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_LeftRightRecycle ||
                        selectMode2 == ButtonSelectMode.Mode_LeftRightRecycle)
            {
                int index = CurrentSelectButtonIndex - 1;
                if (index < 0)
                {
                    index = buttonList.Length - 1;
                }
                CurrentSelectButtonIndex = index;
            }
            else if (leftButtonGroup != null)
            {
                IsWorkDo = false;
                leftButtonGroup.IsWorkDo = true;
            }
            else
            {
                return true;
            }
        }
        else if (this.ButtonRight)
        {
            if (selectMode == ButtonSelectMode.Mode_Rank)
            {
                int x, y;
                getRankModePosition(out x, out y);
                if (!setRankModePosition(x + 1, y))
                {
                    if (rightButtonGroup != null)
                    {
                        IsWorkDo = false;
                        rightButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_RankRecycle)
            {
                int x, y;
                getRankModePosition(out x, out y);
                x += 1;
                if (x >= selectRank[y].Length)
                {
                    x = 0;
                }
                setRankModePosition(x, y);
            }
            else if (selectMode == ButtonSelectMode.Mode_LeftRight ||
                        selectMode2 == ButtonSelectMode.Mode_LeftRight)
            {
                if (CurrentSelectButtonIndex < buttonList.Length - 1)
                {
                    CurrentSelectButtonIndex += 1;
                }
                else
                {
                    if (rightButtonGroup != null)
                    {
                        IsWorkDo = false;
                        rightButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_LeftRightRecycle ||
                        selectMode2 == ButtonSelectMode.Mode_LeftRightRecycle)
            {
                int index = CurrentSelectButtonIndex + 1;
                if (index >= buttonList.Length)
                {
                    index = 0;
                }
                CurrentSelectButtonIndex = index;
            }
            else if (rightButtonGroup != null)
            {
                IsWorkDo = false;
                rightButtonGroup.IsWorkDo = true;
            }
            else
            {
                return true;
            }
        }
        else if (this.ButtonUp)
        {
            if (selectMode == ButtonSelectMode.Mode_Rank)
            {
                int x, y;
                getRankModePosition(out x, out y);
                if (!setRankModePosition(x, y - 1))
                {
                    if (upButtonGroup != null)
                    {
                        IsWorkDo = false;
                        upButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_RankRecycle)
            {
                int x, y;
                getRankModePosition(out x, out y);
                y -= 1;
                if (y < 0)
                {
                    y = selectRank.Length;
                }
                setRankModePosition(x, y);
            }
            else if (selectMode == ButtonSelectMode.Mode_UpDown ||
                        selectMode2 == ButtonSelectMode.Mode_UpDown)
            {
                if (CurrentSelectButtonIndex > 0)
                {
                    CurrentSelectButtonIndex -= 1;
                }
                else
                {
                    if (upButtonGroup != null)
                    {
                        IsWorkDo = false;
                        upButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_UpDownRecycle ||
                        selectMode2 == ButtonSelectMode.Mode_UpDownRecycle)
            {
                int index = CurrentSelectButtonIndex - 1;
                if (index < 0)
                {
                    index = buttonList.Length - 1;
                }
                CurrentSelectButtonIndex = index;
            }
            else if (upButtonGroup != null)
            {
                IsWorkDo = false;
                upButtonGroup.IsWorkDo = true;
            }
            else
            {
                return true;
            }
        }
        else if (this.ButtonDown)
        {
            if (selectMode == ButtonSelectMode.Mode_Rank)
            {
                int x, y;
                getRankModePosition(out x, out y);
                if (!setRankModePosition(x, y + 1))
                {
                    if (downButtonGroup != null)
                    {
                        IsWorkDo = false;
                        downButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_RankRecycle)
            {
                int x, y;
                getRankModePosition(out x, out y);
                y += 1;
                if (y >= selectRank.Length)
                {
                    y = 0;
                }
                setRankModePosition(x, y);
            }
            else if (selectMode == ButtonSelectMode.Mode_UpDown ||
                        selectMode2 == ButtonSelectMode.Mode_UpDown)
            {
                if (CurrentSelectButtonIndex < buttonList.Length - 1)
                {
                    CurrentSelectButtonIndex += 1;
                }
                else
                {
                    if (downButtonGroup != null)
                    {
                        IsWorkDo = false;
                        downButtonGroup.IsWorkDo = true;
                    }
                }
            }
            else if (selectMode == ButtonSelectMode.Mode_UpDownRecycle ||
                        selectMode2 == ButtonSelectMode.Mode_UpDownRecycle)
            {
                int index = CurrentSelectButtonIndex + 1;
                if (index >= buttonList.Length)
                {
                    index = 0;
                }
                CurrentSelectButtonIndex = index;
            }
            else if (downButtonGroup != null)
            {
                IsWorkDo = false;
                downButtonGroup.IsWorkDo = true;
            }
            else
            {
                return true;
            }
        }
        else if (this.ButtonOk)
        {
            if (selectFuntion != null)
            {
                selectFuntion(CurrentSelectButtonIndex);
            }
        }
        else if (this.ButtonBack)
        {
            if (onDialogCloseFuntion == null || selectObject == null || !selectObject.gameObject.activeSelf)
                return true;
            onDialogCloseFuntion();
            return false;
        }
        else
        {
            return true;
        }
        return false;
    }

}
