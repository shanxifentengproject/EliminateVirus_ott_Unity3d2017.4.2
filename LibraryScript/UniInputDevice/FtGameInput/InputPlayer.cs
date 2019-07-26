using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FtGameInput
{
    class InputPlayer
    {
        public enum DeviceType
        {
            Type_KeyboardP1,
            Type_KeyBorarP2,
            Type_RemoteControl,
            Type_Joystick,
            Type_NetInput,
        }
        public static InputDeviceBase AllocDevice(DeviceType type, InputPlayer player)
        {
            switch(type)
            {
                case DeviceType.Type_KeyboardP1:
                    return new InputDevice_KeyboardP1(player);
                case DeviceType.Type_KeyBorarP2:
                    return new InputDevice_KeyboardP2(player);
                case DeviceType.Type_RemoteControl:
                    return new InputDevice_RemoteControl(player);
                case DeviceType.Type_Joystick:
                    return new InputDevice_Joystick(player);
                case DeviceType.Type_NetInput:
                    return new InputDevice_NetInput(player);
            }
            return null;
        }


        //分配的玩家索引
        public int PlayerIndex;
        public InputPlayer(int playerindex)
        {
            PlayerIndex = playerindex;
        }


        protected InputDeviceBase[] deviceList = null;

        protected bool isSupportMouse;
        protected Vector2 mouseSensitivity;
        public enum AxisDefine
        {
            Axis_Horizontal,
            Axis_Vertical,
        }
        //protected Vector2 axisDamp = new Vector2(1.0f, 1.0f);
        private Vector2 axisValue = Vector2.zero;

        public void Initialization(DeviceType[] typelist, bool issupportmouse,Vector2 mousesensitivity)
        {
            deviceList = new InputDeviceBase[typelist.Length];
            for (int i = 0;i<deviceList.Length;i++)
            {
                deviceList[i] = InputPlayer.AllocDevice(typelist[i],this);
                deviceList[i].Initialization();
            }
            isSupportMouse = issupportmouse;
            mouseSensitivity = mousesensitivity;
        }

        public  void Release()
        {

        }
        //周期刷新函数，有些设备需要
        public  void Update()
        {
            foreach (InputDeviceBase d in deviceList)
            {
                d.Update();
            }
            UpdateAxis();
        }

        //震动
        public void Shake(float time)
        {
            foreach (InputDeviceBase d in deviceList)
            {
                d.Shake(time);
            }
        }


        public void AddDevice(DeviceType type)
        {
            InputDeviceBase[] tlist = deviceList;
            int deviceCount = 0;
            if (tlist != null)
            {
                deviceCount = tlist.Length;
            }
            deviceCount += 1;
            deviceList = new InputDeviceBase[deviceCount];
            //首先把旧的移进去
            if (tlist != null)
            {
                for (int i = 0;i<tlist.Length;i++)
                {
                    deviceList[i] = tlist[i];
                }
            }
            //把新加的，增加上
            InputDeviceBase device = InputPlayer.AllocDevice(type,this);
            device.Initialization();
            deviceList[deviceList.Length - 1] = device;
        }


        public float GetAxis(AxisDefine axis)
        {
            if (axis == AxisDefine.Axis_Horizontal)
            {
                return axisValue.x;
            }
            else if (axis == AxisDefine.Axis_Vertical)
            {
                return axisValue.y;
            }
            return 0.0f;
        }
        private void UpdateAxis()
        {
            //这里不可以使用插值计算
            if (ButtonPressLeft)
            {
                //axisValue.x = UnityEngine.Mathf.Lerp(0.0f, -1.0f, UnityEngine.Time.deltaTime * axisDamp.x);
                axisValue.x = -1.0f;
            }
            else if (ButtonPressRight)
            {
                //axisValue.x = UnityEngine.Mathf.Lerp(0.0f, 1.0f, UnityEngine.Time.deltaTime * axisDamp.x);
                axisValue.x = 1.0f;
            }
            else if (isSupportMouse)
            {
                axisValue.x = Input.GetAxis("Mouse X") * mouseSensitivity.x;
            }
            else
            {
                axisValue.x = 0.0f;
            }

            if (ButtonPressUp)
            {
                //axisValue.y = UnityEngine.Mathf.Lerp(0.0f, 1.0f, UnityEngine.Time.deltaTime * axisDamp.y);
                axisValue.y = 1.0f;
            }
            else if (ButtonPressDown)
            {
                //axisValue.y = UnityEngine.Mathf.Lerp(0.0f, -1.0f, UnityEngine.Time.deltaTime * axisDamp.y);
                axisValue.y = -1.0f;
            }
            else if (isSupportMouse)
            {
                axisValue.y = Input.GetAxis("Mouse Y") * mouseSensitivity.y;
            }
            else
            {
                axisValue.y = 0.0f;
            }
        }


        public bool ButtonOk
        {
            get
            {
                foreach(InputDeviceBase d in deviceList)
                {
                    if (d.ButtonOk)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonDown(0))
                    return true;
                return false;
            }
        }
        public  bool ButtonLeft
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonLeft)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonRight
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonRight)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonBack
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonBack)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonDown(1))
                    return true;
                return false;
            }
        }
        public  bool ButtonMenu
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonMenu)
                        return true;
                }
                return false;
            }
        }

        public  bool ButtonOkDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonOkDown)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonDown(0))
                    return true;
                return false;
            }
        }
        public  bool ButtonLeftDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonLeftDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonRightDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonRightDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonUpDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonUpDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonDownDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonDownDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonBackDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonBackDown)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonDown(1))
                    return true;
                return false;
            }
        }
        public  bool ButtonMenuDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonMenuDown)
                        return true;
                }
                return false;
            }
        }


        public  bool ButtonOkUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonOkUp)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonUp(0))
                    return true;
                return false;
            }
        }
        public  bool ButtonLeftUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonLeftUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonRightUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonRightUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonUpUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonUpUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonDownUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonDownUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonBackUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonBackUp)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButtonUp(1))
                    return true;
                return false;
            }
        }
        public  bool ButtonMenuUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonMenuUp)
                        return true;
                }
                return false;
            }
        }


        public  bool ButtonPressOk
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressOk)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButton(0))
                    return true;
                return false;
            }
        }
        public  bool ButtonPressLeft
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressLeft)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonPressRight
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressRight)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonPressUp
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressUp)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonPressDown
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressDown)
                        return true;
                }
                return false;
            }
        }
        public  bool ButtonPressBack
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressBack)
                        return true;
                }
                if (isSupportMouse && Input.GetMouseButton(1))
                    return true;
                return false;
            }
        }
        public  bool ButtonPressMenu
        {
            get
            {
                foreach (InputDeviceBase d in deviceList)
                {
                    if (d.ButtonPressMenu)
                        return true;
                }
                return false;
            }
        }
    }
}
