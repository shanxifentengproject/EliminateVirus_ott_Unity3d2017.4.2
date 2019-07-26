using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FtGameInput
{
    class InputDevice_RemoteControl: InputDeviceBase
    {
        //键值表
        protected const int Key_Ok_1 = 330;
        protected const int Key_Ok_2 = 10;
        protected const int Key_Up = 273;
        protected const int Key_Down = 274;
        protected const int Key_Left = 276;
        protected const int Key_Right = 275;
        protected const int Key_Back = 27;
        protected const int Key_Menu = 319;

        public InputDevice_RemoteControl(InputPlayer player)
            :base(player)
        {

        }


        //加载设备的配置信息
        public override void Initialization() { }
        public override void Release() { }
        //周期刷新函数，有些设备需要
        public override void Update() { }

        public override void Shake(float time) { }

#if PLATFORM_CYBER
        public override bool ButtonOk { get { return Input.GetKeyUp((KeyCode)Key_Ok_1) || Input.GetKeyUp((KeyCode)Key_Ok_2); } }
        public override bool ButtonLeft { get { return Input.GetKeyUp((KeyCode)Key_Left); } }
        public override bool ButtonRight { get { return Input.GetKeyUp((KeyCode)Key_Right); } }
        public override bool ButtonUp { get { return Input.GetKeyUp((KeyCode)Key_Up); } }
        public override bool ButtonDown { get { return Input.GetKeyUp((KeyCode)Key_Down); } }
        public override bool ButtonBack { get { return Input.GetKeyUp((KeyCode)Key_Back); } }
        public override bool ButtonMenu { get { return Input.GetKeyUp((KeyCode)Key_Menu); } }
#else
        public override bool ButtonOk { get { return Input.GetKeyDown((KeyCode)Key_Ok_1) || Input.GetKeyDown((KeyCode)Key_Ok_2); } }
        public override bool ButtonLeft { get { return Input.GetKeyDown((KeyCode)Key_Left); } }
        public override bool ButtonRight { get { return Input.GetKeyDown((KeyCode)Key_Right); } }
        public override bool ButtonUp { get { return Input.GetKeyDown((KeyCode)Key_Up); } }
        public override bool ButtonDown { get { return Input.GetKeyDown((KeyCode)Key_Down); } }
        public override bool ButtonBack { get { return Input.GetKeyDown((KeyCode)Key_Back); } }
        public override bool ButtonMenu { get { return Input.GetKeyDown((KeyCode)Key_Menu); } }
#endif

        public override bool ButtonOkDown { get { return Input.GetKeyDown((KeyCode)Key_Ok_1) || Input.GetKeyDown((KeyCode)Key_Ok_2); } }
        public override bool ButtonLeftDown { get { return Input.GetKeyDown((KeyCode)Key_Left); } }
        public override bool ButtonRightDown { get { return Input.GetKeyDown((KeyCode)Key_Right); } }
        public override bool ButtonUpDown { get { return Input.GetKeyDown((KeyCode)Key_Up); } }
        public override bool ButtonDownDown { get { return Input.GetKeyDown((KeyCode)Key_Down); } }
        public override bool ButtonBackDown { get { return Input.GetKeyDown((KeyCode)Key_Back); } }
        public override bool ButtonMenuDown { get { return Input.GetKeyDown((KeyCode)Key_Menu); } }



        public override bool ButtonOkUp { get { return Input.GetKeyUp((KeyCode)Key_Ok_1) || Input.GetKeyUp((KeyCode)Key_Ok_2); } }
        public override bool ButtonLeftUp { get { return Input.GetKeyUp((KeyCode)Key_Left); } }
        public override bool ButtonRightUp { get { return Input.GetKeyUp((KeyCode)Key_Right); } }
        public override bool ButtonUpUp { get { return Input.GetKeyUp((KeyCode)Key_Up); } }
        public override bool ButtonDownUp { get { return Input.GetKeyUp((KeyCode)Key_Down); } }
        public override bool ButtonBackUp { get { return Input.GetKeyUp((KeyCode)Key_Back); } }
        public override bool ButtonMenuUp { get { return Input.GetKeyUp((KeyCode)Key_Menu); } }


        public override bool ButtonPressOk { get { return Input.GetKey((KeyCode)Key_Ok_1) || Input.GetKey((KeyCode)Key_Ok_2); } }
        public override bool ButtonPressLeft { get { return Input.GetKey((KeyCode)Key_Left); } }
        public override bool ButtonPressRight { get { return Input.GetKey((KeyCode)Key_Right); } }
        public override bool ButtonPressUp { get { return Input.GetKey((KeyCode)Key_Up); } }
        public override bool ButtonPressDown { get { return Input.GetKey((KeyCode)Key_Down); } }
        public override bool ButtonPressBack { get { return Input.GetKey((KeyCode)Key_Back); } }
        public override bool ButtonPressMenu { get { return Input.GetKey((KeyCode)Key_Menu); } }
    }
}
