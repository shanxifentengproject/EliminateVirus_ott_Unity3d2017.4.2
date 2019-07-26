using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtGameInput
{
    class InputDevice_NetInput: InputDeviceBase
    {
        public InputDevice_NetInput(InputPlayer player)
            :base(player)
        {

        }
        private NetInputPlayer linkNetPlayer = null;

        private bool IsValid
        {
            get
            {
                return (linkNetPlayer != null) && !NetInputDefine.Rational_Hang_Up;
            }
        }
        //加载设备的配置信息
        public override void Initialization()
        {
            if (InputServer.Instance != null)
            {
                linkNetPlayer = InputServer.Instance.getNetPlayer(inputPlayer.PlayerIndex);
            }
        }
        public override void Release() { }
        //周期刷新函数，有些设备需要
        public override void Update() { }

        public override void Shake(float time)
        {
            if (IsValid)
            {
                linkNetPlayer.Shake(time);
            }
        }

#if PLATFORM_CYBER
        public override bool ButtonOk { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Ok); } }
        public override bool ButtonLeft { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Left); } }
        public override bool ButtonRight { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Right); } }
        public override bool ButtonUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Up); } }
        public override bool ButtonDown { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Down); } }
        public override bool ButtonBack { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Back); } }
        public override bool ButtonMenu { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Menu); } }
#else
        public override bool ButtonOk { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Ok); } }
        public override bool ButtonLeft { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Left); } }
        public override bool ButtonRight { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Right); } }
        public override bool ButtonUp { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Up); } }
        public override bool ButtonDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Down); } }
        public override bool ButtonBack { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Back); } }
        public override bool ButtonMenu { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Menu); } }
#endif

        public override bool ButtonOkDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Ok); } }
        public override bool ButtonLeftDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Left); } }
        public override bool ButtonRightDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Right); } }
        public override bool ButtonUpDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Up); } }
        public override bool ButtonDownDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Down); } }
        public override bool ButtonBackDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Back); } }
        public override bool ButtonMenuDown { get { return !IsValid ? false : linkNetPlayer.GetKeyDown(NetInputKeyCode.Key_Menu); } }


        public override bool ButtonOkUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Ok); } }
        public override bool ButtonLeftUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Left); } }
        public override bool ButtonRightUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Right); } }
        public override bool ButtonUpUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Up); } }
        public override bool ButtonDownUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Down); } }
        public override bool ButtonBackUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Back); } }
        public override bool ButtonMenuUp { get { return !IsValid ? false : linkNetPlayer.GetKeyUp(NetInputKeyCode.Key_Menu); } }


        public override bool ButtonPressOk { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Ok); } }
        public override bool ButtonPressLeft { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Left); } }
        public override bool ButtonPressRight { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Right); } }
        public override bool ButtonPressUp { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Up); } }
        public override bool ButtonPressDown { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Down); } }
        public override bool ButtonPressBack { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Back); } }
        public override bool ButtonPressMenu { get { return !IsValid ? false : linkNetPlayer.GetKeyPress(NetInputKeyCode.Key_Menu); } }
    }
}
