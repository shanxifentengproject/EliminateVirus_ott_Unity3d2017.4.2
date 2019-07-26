using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtGameInput
{
    public enum NetInputKeyCode
    {
        Key_Ok = 0,
        Key_Up = 1,
        Key_Down = 2,
        Key_Left = 3,
        Key_Right = 4,
        Key_Back = 5,
        Key_Menu = 6,
        Key_Count = 7,
    }

    enum NetInputKeyStatus
    {
        Status_Nothing = 0,
        Status_Down = 1,
        Status_Press = 2,
        Status_Up = 3,
    }

    //游戏使用的设备标识
    public enum NetInputGameDeviceType
    {
        Type_RemoteControl = 0x01,//遥控器
        Type_Joystick = 0x02,//手柄
        Type_SteeringWheel = 0x03,//方向盘
        Type_GravitySensor = 0x04,//重力感应
        Type_Responder = 0x05,//抢答器
        Type_Count = 0x06,
    }

    public class NetInputDefine
    {
        internal const float RelieveActivePlayerTime = 5.0f;
        //合理挂起，一般在付费的时候需要打开新的窗口，打开这个开关
        //这时候，再处理挂起的时候，不会断开网络，同时，网络输入，被忽略
        //这样，后台操作不至于出问题
        public static bool Rational_Hang_Up = false;

        //设备功能
        public const int DeviceFunction_Shake = 0x01;

        //获取二维码内容字符串
        public static string CreateQRCodeString()
        {
            return "FTGAMEPAD|" + InputUdpServerBase.localip.ToString();
        }
        //解析读取二维码获得的字符串
        public static string AnalysisQRCodeString_Ip(string s)
        {
            string[] list = s.Split('|');
            if (list.Length <= 1 || list[0] != "FTGAMEPAD")
                return "";
            return list[1];
        }
    }
}
