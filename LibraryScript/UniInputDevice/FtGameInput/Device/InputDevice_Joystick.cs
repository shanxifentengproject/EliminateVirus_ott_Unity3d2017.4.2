using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FtGameInput
{
    class InputDevice_Joystick: InputDeviceBase
    {
        /*
         * Joy_Sel = 338,
        Joy_Start = 339,
        Joy_1 = 330,//确定
        Joy_1_1 = 345,//确定  手柄的 1键 上键 Y键
        Joy_2 = 331,//确定
        Joy_2_1 = 346,//确定 手柄的 2键 右键 B键
        Joy_3 = 332,//返回
        Joy_3_1 = 347,//返回 手柄的 3键 下键 A键
        Joy_4 = 333,//返回
        Joy_4_1 = 348,//返回  手柄的 4键 左键 X键
        Joy_R1 = 334,
        Joy_R2 = 336,
        Joy_L1 = 335,//菜单
        Joy_L1_1 = 349,//菜单 手柄的左L1键
        Joy_L2 = 337,
         * 
         * */


        //键值表
        //protected const int Key_Ok_Joy_1 = 330;
        protected const int Key_Ok_Joy_1_1 = 345;
        protected const int Key_Ok_Joy_2 = 331;
        protected const int Key_Ok_Joy_2_1 = 346;
        protected const int Key_Ok_Joy_3 = 332;
        protected const int Key_Ok_Joy_3_1 = 347;

        protected const int Key_Back_Joy_4 = 333;
        protected const int Key_Back_Joy_4_1 = 348;

        protected const int Key_Menu_Joy_L1 = 335;
        protected const int Key_Menu_Joy_L1_1 = 349;



        private const float Horizontal_Up = 1.0f;
        private const float Horizontal_Down = -1.0f;
        private const float Horizontal_Left = -1.0f;
        private const float Horizontal_Right = 1.0f;

        private const string HorizontalAxisName = "HorizontalJoystick";
        private const string VerticalAxisName = "VerticalJoystick";

        private enum JoystickButtonStatus
        {
            Status_Nothing,
            Status_Down,
            Status_Press,
            Status_Up,
        }
        private JoystickButtonStatus JoystickButtonLeftStatus = JoystickButtonStatus.Status_Nothing;
        private JoystickButtonStatus JoystickButtonRightStatus = JoystickButtonStatus.Status_Nothing;
        private JoystickButtonStatus JoystickButtonUpStatus = JoystickButtonStatus.Status_Nothing;
        private JoystickButtonStatus JoystickButtonDownStatus = JoystickButtonStatus.Status_Nothing;


        public InputDevice_Joystick(InputPlayer player)
            :base(player)
        {

        }

        private void UpdateJoystickButtonStatus()
        {
            float valueHorizontalAxis = Input.GetAxis(HorizontalAxisName);
            float valueVerticalAxis = Input.GetAxis(VerticalAxisName);


            if (valueHorizontalAxis == Horizontal_Left)
            {
                if (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Nothing ||
                        JoystickButtonLeftStatus == JoystickButtonStatus.Status_Up)
                {
                    JoystickButtonLeftStatus = JoystickButtonStatus.Status_Down;
                }
                else if (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Down)
                {
                    JoystickButtonLeftStatus = JoystickButtonStatus.Status_Press;
                }
            }
            else
            {
                if (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Down ||
                        JoystickButtonLeftStatus == JoystickButtonStatus.Status_Press)
                {
                    JoystickButtonLeftStatus = JoystickButtonStatus.Status_Up;
                }
                else
                {
                    JoystickButtonLeftStatus = JoystickButtonStatus.Status_Nothing;
                }
                
            }
            if (valueHorizontalAxis == Horizontal_Right)
            {
                if (JoystickButtonRightStatus == JoystickButtonStatus.Status_Nothing ||
                        JoystickButtonRightStatus == JoystickButtonStatus.Status_Up)
                {
                    JoystickButtonRightStatus = JoystickButtonStatus.Status_Down;
                }
                else if (JoystickButtonRightStatus == JoystickButtonStatus.Status_Down)
                {
                    JoystickButtonRightStatus = JoystickButtonStatus.Status_Press;
                }
            }
            else
            {
                if (JoystickButtonRightStatus == JoystickButtonStatus.Status_Down ||
                        JoystickButtonRightStatus == JoystickButtonStatus.Status_Press)
                {
                    JoystickButtonRightStatus = JoystickButtonStatus.Status_Up;
                }
                else
                {
                    JoystickButtonRightStatus = JoystickButtonStatus.Status_Nothing;
                }
            }

            if (valueVerticalAxis == Horizontal_Up)
            {
                if (JoystickButtonUpStatus == JoystickButtonStatus.Status_Nothing ||
                        JoystickButtonUpStatus == JoystickButtonStatus.Status_Up)
                {
                    JoystickButtonUpStatus = JoystickButtonStatus.Status_Down;
                }
                else if (JoystickButtonUpStatus == JoystickButtonStatus.Status_Down)
                {
                    JoystickButtonUpStatus = JoystickButtonStatus.Status_Press;
                }
            }
            else
            {
                if (JoystickButtonUpStatus == JoystickButtonStatus.Status_Down ||
                        JoystickButtonUpStatus == JoystickButtonStatus.Status_Press)
                {
                    JoystickButtonUpStatus = JoystickButtonStatus.Status_Up;
                }
                else
                {
                    JoystickButtonUpStatus = JoystickButtonStatus.Status_Nothing;
                }
            }

            if (valueVerticalAxis == Horizontal_Down)
            {
                if (JoystickButtonDownStatus == JoystickButtonStatus.Status_Nothing ||
                        JoystickButtonDownStatus == JoystickButtonStatus.Status_Up)
                {
                    JoystickButtonDownStatus = JoystickButtonStatus.Status_Down;
                }
                else if (JoystickButtonDownStatus == JoystickButtonStatus.Status_Down)
                {
                    JoystickButtonDownStatus = JoystickButtonStatus.Status_Press;
                }
            }
            else
            {
                if (JoystickButtonDownStatus == JoystickButtonStatus.Status_Down ||
                        JoystickButtonDownStatus == JoystickButtonStatus.Status_Press)
                {
                    JoystickButtonDownStatus = JoystickButtonStatus.Status_Up;
                }
                else
                {
                    JoystickButtonDownStatus = JoystickButtonStatus.Status_Nothing;
                }
            }
        }



        //加载设备的配置信息
        public override void Initialization() { }
        public override void Release() { }
        //周期刷新函数，有些设备需要
        public override void Update()
        {
            UpdateJoystickButtonStatus();
        }

        public override void Shake(float time) { }
#if PLATFORM_CYBER
        public override bool ButtonOk
        {
            get
            {
                return /*Input.GetKeyDown((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_1_1) ||
                        //Input.GetKeyDown((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3_1);
            }
        }
        public override bool ButtonLeft { get { return (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonRight { get { return (JoystickButtonRightStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonUp { get { return (JoystickButtonUpStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonDown { get { return (JoystickButtonDownStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonBack
        {
            get
            {
                return Input.GetKeyUp((KeyCode)Key_Ok_Joy_2) || 
                        Input.GetKeyUp((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyUp((KeyCode)Key_Back_Joy_4_1);
            
            }
        }
        public override bool ButtonMenu
        {
            get
            {
                return Input.GetKeyUp((KeyCode)Key_Menu_Joy_L1) ||
                        Input.GetKeyUp((KeyCode)Key_Menu_Joy_L1_1);
                
            }
        }
#else
        public override bool ButtonOk
        {
            get
            {
                return /*Input.GetKeyDown((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_1_1) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3_1);
            }
        }
        public override bool ButtonLeft { get { return (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonRight { get { return (JoystickButtonRightStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonUp { get { return (JoystickButtonUpStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonDown { get { return (JoystickButtonDownStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonBack
        {
            get
            {
                return Input.GetKeyDown((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyDown((KeyCode)Key_Back_Joy_4_1);
            
            }
        }
        public override bool ButtonMenu
        {
            get
            {
                return Input.GetKeyDown((KeyCode)Key_Menu_Joy_L1) ||
                        Input.GetKeyDown((KeyCode)Key_Menu_Joy_L1_1);
                
            }
        }
#endif

#if PLATFORM_CYBER
        public override bool ButtonOkDown
        {
            get
            {
                return /*Input.GetKeyDown((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_1_1) ||
                        //Input.GetKeyDown((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#else
        public override bool ButtonOkDown
        {
            get
            {
                return /*Input.GetKeyDown((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_1_1) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyDown((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#endif
        public override bool ButtonLeftDown { get { return (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonRightDown { get { return (JoystickButtonRightStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonUpDown { get { return (JoystickButtonUpStatus == JoystickButtonStatus.Status_Down); } }
        public override bool ButtonDownDown { get { return (JoystickButtonDownStatus == JoystickButtonStatus.Status_Down); } }
#if PLATFORM_CYBER
        public override bool ButtonBackDown
        {
            get
            {
                return Input.GetKeyDown((KeyCode)Key_Ok_Joy_2) || 
                        Input.GetKeyDown((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyDown((KeyCode)Key_Back_Joy_4_1);
            }
        }
#else
        public override bool ButtonBackDown
        {
            get
            {
                return Input.GetKeyDown((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyDown((KeyCode)Key_Back_Joy_4_1);
            }
        }
#endif
        public override bool ButtonMenuDown
        {
            get
            {
                return Input.GetKeyDown((KeyCode)Key_Menu_Joy_L1) ||
                        Input.GetKeyDown((KeyCode)Key_Menu_Joy_L1_1);
            }
        }


#if PLATFORM_CYBER
        public override bool ButtonOkUp
        {
            get
            {
                return /*Input.GetKeyUp((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_1_1) ||
                        //Input.GetKeyUp((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#else
        public override bool ButtonOkUp
        {
            get
            {
                return /*Input.GetKeyUp((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_1_1) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKeyUp((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#endif
        public override bool ButtonLeftUp { get { return (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonRightUp { get { return (JoystickButtonRightStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonUpUp { get { return (JoystickButtonUpStatus == JoystickButtonStatus.Status_Up); } }
        public override bool ButtonDownUp { get { return (JoystickButtonDownStatus == JoystickButtonStatus.Status_Up); } }
#if PLATFORM_CYBER
        public override bool ButtonBackUp
        {
            get
            {
                return Input.GetKeyUp((KeyCode)Key_Ok_Joy_2) || 
                        Input.GetKeyUp((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyUp((KeyCode)Key_Back_Joy_4_1);
            }
        }
#else
        public override bool ButtonBackUp
        {
            get
            {
                return Input.GetKeyUp((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKeyUp((KeyCode)Key_Back_Joy_4_1);
            }
        }
#endif
        public override bool ButtonMenuUp
        {
            get
            {
                return Input.GetKeyUp((KeyCode)Key_Menu_Joy_L1) ||
                        Input.GetKeyUp((KeyCode)Key_Menu_Joy_L1_1);
            }
        }

#if PLATFORM_CYBER
        public override bool ButtonPressOk
        {
            get
            {
                return /*Input.GetKey((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKey((KeyCode)Key_Ok_Joy_1_1) ||
                        //Input.GetKey((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#else
        public override bool ButtonPressOk
        {
            get
            {
                return /*Input.GetKey((KeyCode)Key_Ok_Joy_1) ||*/
                        Input.GetKey((KeyCode)Key_Ok_Joy_1_1) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_2) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_2_1) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_3) ||
                        Input.GetKey((KeyCode)Key_Ok_Joy_3_1);
            }
        }
#endif
        public override bool ButtonPressLeft
        {
            get
            {
                return (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Down) ||
                                (JoystickButtonLeftStatus == JoystickButtonStatus.Status_Press);
            }
        }
        public override bool ButtonPressRight
        {
            get
            {
                return (JoystickButtonRightStatus == JoystickButtonStatus.Status_Down) ||
                                (JoystickButtonRightStatus == JoystickButtonStatus.Status_Press);
            }
        }
        public override bool ButtonPressUp
        {
            get
            {
                return (JoystickButtonUpStatus == JoystickButtonStatus.Status_Down) ||
                                (JoystickButtonUpStatus == JoystickButtonStatus.Status_Press);
            }
        }
        public override bool ButtonPressDown
        {
            get
            {
                return (JoystickButtonDownStatus == JoystickButtonStatus.Status_Down) ||
                                (JoystickButtonDownStatus == JoystickButtonStatus.Status_Press);
            }
        }
#if PLATFORM_CYBER
        public override bool ButtonPressBack
        {
            get
            {
                return Input.GetKey((KeyCode)Key_Ok_Joy_2) || 
                        Input.GetKey((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKey((KeyCode)Key_Back_Joy_4_1);
            }
        }
#else
        public override bool ButtonPressBack
        {
            get
            {
                return Input.GetKey((KeyCode)Key_Back_Joy_4) ||
                        Input.GetKey((KeyCode)Key_Back_Joy_4_1);
            }
        }
#endif
        public override bool ButtonPressMenu
        {
            get
            {
                return Input.GetKey((KeyCode)Key_Menu_Joy_L1) ||
                        Input.GetKey((KeyCode)Key_Menu_Joy_L1_1);
            }
        }
    }
}
