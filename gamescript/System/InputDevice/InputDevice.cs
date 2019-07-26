using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using UnityEngine;

partial class InputDevice : InputDeviceBase
{
    private static InputDevice myInstance = null;
    public static InputDevice Instance
    {
        get
        {
            return myInstance;
        }
        set
        {
            myInstance = value;
        }
    }


    protected override void Awake()
    {
        base.Awake();
        InputDevice.Instance = this;
    }

    public static void Initialization()
    {

    }

    public static void Release()
    {
       
    }

    //一下函数是之前没有划分玩家位的时候遗留下来的函数，都对接到P1位上去
    public static bool ButtonOk { get { return InputDevice.Instance.InputP1.ButtonOk; } }
    public static bool ButtonLeft { get { return InputDevice.Instance.InputP1.ButtonLeft; } }
    public static bool ButtonRight { get { return InputDevice.Instance.InputP1.ButtonRight; } }
    public static bool ButtonUp { get { return InputDevice.Instance.InputP1.ButtonUp; } }
    public static bool ButtonDown { get { return InputDevice.Instance.InputP1.ButtonDown; } }
    public static bool ButtonBack { get { return InputDevice.Instance.InputP1.ButtonBack; } }
    public static bool ButtonMenu { get { return InputDevice.Instance.InputP1.ButtonMenu; } }


    public static bool ButtonPressOk { get { return InputDevice.Instance.InputP1.ButtonPressOk; } }
    public static bool ButtonPressLeft { get { return InputDevice.Instance.InputP1.ButtonPressLeft; } }
    public static bool ButtonPressRight { get { return InputDevice.Instance.InputP1.ButtonPressRight; } }
    public static bool ButtonPressUp { get { return InputDevice.Instance.InputP1.ButtonPressUp; } }
    public static bool ButtonPressDown { get { return InputDevice.Instance.InputP1.ButtonPressDown; } }
    public static bool ButtonPressBack { get { return InputDevice.Instance.InputP1.ButtonPressBack; } }
    public static bool ButtonPressMenu { get { return InputDevice.Instance.InputP1.ButtonPressMenu; } }

    public static bool ButtonLeftDown { get { return InputDevice.Instance.InputP1.ButtonLeftDown; } }
    public static bool ButtonRightDown { get { return InputDevice.Instance.InputP1.ButtonRightDown; } }
    public static bool ButtonUpDown { get { return InputDevice.Instance.InputP1.ButtonUpDown; } }
    public static bool ButtonDownDown { get { return InputDevice.Instance.InputP1.ButtonDownDown; } }
    public static bool ButtonOkDown { get { return InputDevice.Instance.InputP1.ButtonOkDown; } }
    public static bool ButtonBackDown { get { return InputDevice.Instance.InputP1.ButtonBackDown; } }
    public static bool ButtonMenuDown { get { return InputDevice.Instance.InputP1.ButtonMenuDown; } }

    public static bool ButtonLeftUp { get { return InputDevice.Instance.InputP1.ButtonLeftUp; } }
    public static bool ButtonRightUp { get { return InputDevice.Instance.InputP1.ButtonRightUp; } }
    public static bool ButtonUpUp { get { return InputDevice.Instance.InputP1.ButtonUpUp; } }
    public static bool ButtonDownUp { get { return InputDevice.Instance.InputP1.ButtonDownUp; } }
    public static bool ButtonOkUp { get { return InputDevice.Instance.InputP1.ButtonOkUp; } }
    public static bool ButtonBackUp { get { return InputDevice.Instance.InputP1.ButtonBackUp; } }
    public static bool ButtonMenuUp { get { return InputDevice.Instance.InputP1.ButtonMenuUp; } }

    public enum AxisDefine
    {
        Axis_Horizontal,
        Axis_Vertical,
    }
    public static float GetAxis(AxisDefine axis)
    {
        if (axis == AxisDefine.Axis_Horizontal)
        {
            return InputDevice.Instance.InputP1.GetAxis(FtGameInput.InputPlayer.AxisDefine.Axis_Horizontal);
        }
        else if (axis == AxisDefine.Axis_Vertical)
        {
            return InputDevice.Instance.InputP1.GetAxis(FtGameInput.InputPlayer.AxisDefine.Axis_Vertical);
        }
        return 0.0f;
    }


    //模拟Joystick
    public static Vector3 JoystickRelativePosition = new Vector3(100, 100, 0);
    public enum JoystickRelativeDirect
    {
        Direct_Left,
        Direct_LeftUp,
        Direct_Up,
        Direct_UpRight,
        Direct_Right,
        Direct_RightDown,
        Direct_Down,
        Direct_DownLeft,
    }
    public static Vector3 GetJoystickPosition(Vector3 basePosition)
    {
        JoystickRelativeDirect direct = JoystickRelativeDirect.Direct_Left;
        if (InputDevice.ButtonPressLeft)
        {
            if (InputDevice.ButtonPressUp)
            {
                direct = JoystickRelativeDirect.Direct_LeftUp;
            }
            else if (InputDevice.ButtonPressDown)
            {
                direct = JoystickRelativeDirect.Direct_DownLeft;
            }
            else
            {
                direct = JoystickRelativeDirect.Direct_Left;
            }
        }
        else if (InputDevice.ButtonPressUp)
        {
            if (InputDevice.ButtonPressLeft)
            {
                direct = JoystickRelativeDirect.Direct_LeftUp;
            }
            else if (InputDevice.ButtonPressRight)
            {
                direct = JoystickRelativeDirect.Direct_UpRight;
            }
            else
            {
                direct = JoystickRelativeDirect.Direct_Up;
            }
        }
        else if (InputDevice.ButtonPressRight)
        {
            if (InputDevice.ButtonPressUp)
            {
                direct = JoystickRelativeDirect.Direct_UpRight;
            }
            else if (InputDevice.ButtonPressDown)
            {
                direct = JoystickRelativeDirect.Direct_RightDown;
            }
            else
            {
                direct = JoystickRelativeDirect.Direct_Right;
            }
        }
        else if (InputDevice.ButtonPressDown)
        {
            if (InputDevice.ButtonPressRight)
            {
                direct = JoystickRelativeDirect.Direct_RightDown;
            }
            else if (InputDevice.ButtonPressLeft)
            {
                direct = JoystickRelativeDirect.Direct_DownLeft;
            }
            else
            {
                direct = JoystickRelativeDirect.Direct_Down;
            }
        }
        return GetJoystickPosition(direct, basePosition);
    }
    public static Vector3 GetJoystickPosition(JoystickRelativeDirect direct, Vector3 basePosition)
    {
        switch (direct)
        {
            case JoystickRelativeDirect.Direct_Left:
                return basePosition + new Vector3(-JoystickRelativePosition.x, 0, 0);
            case JoystickRelativeDirect.Direct_LeftUp:
                return basePosition + new Vector3(-JoystickRelativePosition.x, JoystickRelativePosition.y, 0);
            case JoystickRelativeDirect.Direct_Up:
                return basePosition + new Vector3(0, JoystickRelativePosition.y, 0);
            case JoystickRelativeDirect.Direct_UpRight:
                return basePosition + new Vector3(JoystickRelativePosition.x, JoystickRelativePosition.y, 0);
            case JoystickRelativeDirect.Direct_Right:
                return basePosition + new Vector3(JoystickRelativePosition.x, 0, 0);
            case JoystickRelativeDirect.Direct_RightDown:
                return basePosition + new Vector3(JoystickRelativePosition.x, -JoystickRelativePosition.y, 0);
            case JoystickRelativeDirect.Direct_Down:
                return basePosition + new Vector3(0, -JoystickRelativePosition.y, 0);
            case JoystickRelativeDirect.Direct_DownLeft:
                return basePosition + new Vector3(-JoystickRelativePosition.x, -JoystickRelativePosition.y, 0);
            default:
                return basePosition;
        }
    }

}
