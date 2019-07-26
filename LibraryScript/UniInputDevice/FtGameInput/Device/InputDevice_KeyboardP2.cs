using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FtGameInput
{
    class InputDevice_KeyboardP2: InputDevice_Keyboard
    {

        public InputDevice_KeyboardP2(InputPlayer player)
            :base(player)
        {

        }

        //加载设备的配置信息
        public override void Initialization()
        {
            Key_Ok = (int)KeyCode.KeypadEnter;
            Key_Up = (int)KeyCode.Keypad8;
            Key_Down = (int)KeyCode.Keypad2;
            Key_Left = (int)KeyCode.Keypad4;
            Key_Right = (int)KeyCode.Keypad6;
            Key_Back = (int)KeyCode.Keypad0;
            Key_Menu = (int)KeyCode.Keypad1;
        }
    }
}
