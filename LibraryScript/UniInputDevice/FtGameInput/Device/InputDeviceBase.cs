using System;
using System.Collections.Generic;
using System.Text;
namespace FtGameInput
{
    abstract class InputDeviceBase
    {

        protected InputPlayer inputPlayer;

        public InputDeviceBase(InputPlayer player)
        {
            inputPlayer = player;
        }

        //加载设备的配置信息
        public abstract void Initialization();
        public abstract void Release();
        //周期刷新函数，有些设备需要
        public abstract void Update();

        public abstract void Shake(float time);

        public abstract bool ButtonOk { get; }
        public abstract bool ButtonLeft { get; }
        public abstract bool ButtonRight { get; }
        public abstract bool ButtonUp { get; }
        public abstract bool ButtonDown { get; }
        public abstract bool ButtonBack { get; }
        public abstract bool ButtonMenu { get; }

        public abstract bool ButtonOkDown { get; }
        public abstract bool ButtonLeftDown { get; }
        public abstract bool ButtonRightDown { get; }
        public abstract bool ButtonUpDown { get; }
        public abstract bool ButtonDownDown { get; }
        public abstract bool ButtonBackDown { get; }
        public abstract bool ButtonMenuDown { get; }


        public abstract bool ButtonOkUp { get; }
        public abstract bool ButtonLeftUp { get; }
        public abstract bool ButtonRightUp { get; }
        public abstract bool ButtonUpUp { get; }
        public abstract bool ButtonDownUp { get; }
        public abstract bool ButtonBackUp { get; }
        public abstract bool ButtonMenuUp { get; }


        public abstract bool ButtonPressOk { get; }
        public abstract bool ButtonPressLeft { get; }
        public abstract bool ButtonPressRight { get; }
        public abstract bool ButtonPressUp { get; }
        public abstract bool ButtonPressDown { get; }
        public abstract bool ButtonPressBack { get; }
        public abstract bool ButtonPressMenu { get; }
    }
}

