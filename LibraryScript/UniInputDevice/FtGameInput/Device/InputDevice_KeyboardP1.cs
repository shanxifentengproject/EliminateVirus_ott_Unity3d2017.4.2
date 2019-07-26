using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FtGameInput
{
    class InputDevice_KeyboardP1: InputDevice_Keyboard
    {

        public InputDevice_KeyboardP1(InputPlayer player)
            :base(player)
        {

        }
        //加载设备的配置信息
        public override void Initialization()
        {
            Key_Ok = (int)KeyCode.Return;
            Key_Up = (int)KeyCode.W;
            Key_Down = (int)KeyCode.S;
            Key_Left = (int)KeyCode.A;
            Key_Right = (int)KeyCode.D;
            Key_Back = (int)KeyCode.Backspace;
            Key_Menu = (int)KeyCode.M;
        }
    }
}
