using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FtGameInput
{
    class NetInputData
    {
        //指令集定义
        public enum NetCommand
        {
            //0x01	基础键位同步指令
            Command_InputKeyUpdate = 0x01,
            //0x02	客户端退出封包
            Command_ClientClose = 0x02,
            //0x03	无空闲可分配玩家位
            Command_PlayersFull = 0x03,
            //0x04	设置客户端玩家索引
            Command_ResetPlayerIndex = 0x04,
            //0x05	设置游戏设备信息封包
            Command_SetGameDeviceInfo = 0x05,
            //0x06	同步选位信息状态封包
            Command_UpdateSelectPositionInfo = 0x06,
            //0x07	用户选择一个空闲位
            Command_SelectFreePosition = 0x07,
            //0x08	选择位已被占用	
            Command_SelectPositionCantUse = 0x08,
            //0x09	用户选位确定
            Command_SelectPositionOk = 0x09,
            //0x10	服务端确定客户端选位成功	
            Command_SelectPositionOkReback = 0x0a,
            //0x11	手机震动
            Command_Shake = 0x0b,
            //0x12	选位信息不相符
            Command_SelectFreePositionErr = 0x0c,
            //0x13	设备侦测封包
            Command_ClientSense = 0x0d,
            //0x14	设备侦测封包回执	
            Command_ServerRebackSense = 0x0e,
            //0x15	服务停止
            Command_ServerStop = 0x0f,
            //0x16	同步游戏信息封包
            Command_UpdateGameInfo = 0x10,
            //0x17	回执用户选择一个空闲位成功
            Command_SelectFreePositionReback = 0x11,

        }

        private static int[] CommandPackageSize = {
            0,
            8,//0x01	基础键位同步指令
            6,//0x02	客户端退出封包
            5,//0x03	无空闲可分配玩家位
            6,//0x04	设置客户端玩家索引
            7,//0x05	设置游戏设备信息封包
            8,//0x06	同步选位信息状态封包
            6,//0x07	用户选择一个空闲位
            5,//0x08	选择位已被占用	
            5,//0x09	用户选位确定
            5,//0x10	服务端确定客户端选位成功	
            6,//0x11	手机震动
            5,//0x12	选位信息不相符
            6,//0x13	设备侦测封包
            6,//0x14	设备侦测封包回执	
            5,//0x15	服务停止
            32,//0x16	同步游戏信息封包
            6,//0x17	回执用户选择一个空闲位成功    
        };


        //包头数据
        private const byte PackageHead1 = 0xaa;
        private const byte PackageHead2 = 0x55;
        private const byte PackageLaste1 = 0xff;
        private const byte PackageLaste2 = 0xff;

        //包头地址
        private const int PackageDataIndex_Head1 = 0;
        private const int PackageDataIndex_Head2 = 1;
        //指令地址
        private const int PackageDataIndex_Command = 2;
        //用户索引地址
        private const int PackageDataIndex_PlayerIndex = 3;
        //数据开始地址
        //private const int PackageDataIndex_DataStart = 4;

        public byte[] buffer = null;
        //是否有效数据
        public bool IsValid
        {
            get
            {
                //只有包头包尾的尺寸
                if (buffer == null || buffer.Length <= 4)
                    return false;
                if (buffer[PackageDataIndex_Head1] != PackageHead1 ||
                    buffer[PackageDataIndex_Head2] != PackageHead2)
                    return false;
                if (buffer[buffer.Length - 2] != PackageLaste2 ||
                     buffer[buffer.Length - 1] != PackageLaste1)
                    return false;
                return true;
            }

        }

        //获取指令数据
        public NetCommand Command
        {
            get
            {
                return (NetCommand)buffer[PackageDataIndex_Command];
            }
            set
            {
                buffer[PackageDataIndex_Command] = (byte)value;
            }
        }

        //没有玩家索引的值
        public const int PlayerIndexEmpty = 255;
        //用户索引
        public int PlayerIndex
        {
            get
            {
                return (int)buffer[PackageDataIndex_PlayerIndex];
            }
            set
            {
                buffer[PackageDataIndex_PlayerIndex] = (byte)value;
            }
        }

        //其他是为各个特殊包的数据地址定义服务
        private const int PackageDataIndex_InputKey_High = 4;
        private const int PackageDataIndex_InputKey_Lower = PackageDataIndex_InputKey_High + 1;
        public int Command_InputKeyUpdate_KeyValue
        {
            get
            {
                return ((buffer[NetInputData.PackageDataIndex_InputKey_High] << 8) |
                                    (buffer[NetInputData.PackageDataIndex_InputKey_Lower]));
            }
            set
            {
                buffer[NetInputData.PackageDataIndex_InputKey_High] = (byte)(value >> 8);
                buffer[NetInputData.PackageDataIndex_InputKey_Lower] = (byte)value;
            }
        }

        //设定设备索引
        private const int PackageDataIndex_GameDeviceInfo_DeviceSelect = 3;
        private const int PackageDataIndex_GameDeviceInfo_DeviceFunction = PackageDataIndex_GameDeviceInfo_DeviceSelect + 1;
        public byte Command_SetGameDeviceInfo_DeviceSelect
        {
            get
            {
                return buffer[PackageDataIndex_GameDeviceInfo_DeviceSelect];
            }
            set
            {
                buffer[PackageDataIndex_GameDeviceInfo_DeviceSelect] = value;
            }
        }

        public byte Command_SetGameDeviceInfo_DeviceFunction
        {
            get
            {
                return buffer[PackageDataIndex_GameDeviceInfo_DeviceFunction];
            }
            set
            {
                buffer[PackageDataIndex_GameDeviceInfo_DeviceFunction] = value;
            }
        }

        private const int PackageDataIndex_UpdateSelectPositionInfo_SupportPlayers = 3;
        private const int PackageDataIndex_UpdateSelectPositionInfo_PositionDataHigh = PackageDataIndex_UpdateSelectPositionInfo_SupportPlayers + 1;
        private const int PackageDataIndex_UpdateSelectPositionInfo_PositionDataLow = PackageDataIndex_UpdateSelectPositionInfo_PositionDataHigh + 1;

        public int Command_UpdateSelectPositionInfo_SupportPlayers
        {
            get
            {
                return buffer[PackageDataIndex_UpdateSelectPositionInfo_SupportPlayers];
            }
            set
            {
                buffer[PackageDataIndex_UpdateSelectPositionInfo_SupportPlayers] = (byte)value;
            }
        }

        public int Command_UpdateSelectPositionInfo_PositionData
        {
            get
            {
                return (buffer[PackageDataIndex_UpdateSelectPositionInfo_PositionDataHigh] << 8) |
                        buffer[PackageDataIndex_UpdateSelectPositionInfo_PositionDataLow];
            }
            set
            {
                buffer[PackageDataIndex_UpdateSelectPositionInfo_PositionDataHigh] = (byte)(value >> 8);
                buffer[PackageDataIndex_UpdateSelectPositionInfo_PositionDataLow] = (byte)value;
            }
        }


        private const int PackageDataIndex_SelectFreePosition_SelectPosition = 3;
        public int Command_SelectFreePosition_SelectPosition
        {
            get
            {
                return buffer[PackageDataIndex_SelectFreePosition_SelectPosition];
            }
            set
            {
                buffer[PackageDataIndex_SelectFreePosition_SelectPosition] = (byte)value;
            }
        }


        //震动的时间粒子
        public const float ShakeEveryTime = 0.5f;
        private const int PackageDataIndex_Shake_ShakeCount = 3;
        public int Command_Shake_ShakeCount
        {
            get
            {
                return buffer[PackageDataIndex_Shake_ShakeCount];
            }
            set
            {
                buffer[PackageDataIndex_Shake_ShakeCount] = (byte)value;
            }
        }

        private const int PackageDataIndex_Sense_IpListIndex = 3;
        public int Command_Sense_IpListIndex
        {
            get
            {
                return (int)buffer[PackageDataIndex_Sense_IpListIndex];
            }
            set
            {
                buffer[PackageDataIndex_Sense_IpListIndex] = (byte)value;
            }
        }



        private const int PackageDataIndex_GameInfo_GameNameLength = 3;
        private const int PackageDataIndex_GameInfo_GameName = PackageDataIndex_GameInfo_GameNameLength + 1;
        private const int PackageData_GameInfo_GameNameMaxLength = 26;
        public string Command_UpdateGameInfo_GameName
        {
            get
            {
                return Encoding.UTF8.GetString(buffer, PackageDataIndex_GameInfo_GameName,
                                (int)buffer[PackageDataIndex_GameInfo_GameNameLength]);
            }
            set
            {
                byte[] nameData = Encoding.UTF8.GetBytes(value);
                int nameLength = (nameData.Length <= PackageData_GameInfo_GameNameMaxLength) ?
                    nameData.Length : PackageData_GameInfo_GameNameMaxLength;
                buffer[PackageDataIndex_GameInfo_GameNameLength] = (byte)nameLength;
                Array.Copy(nameData, 0, buffer, PackageDataIndex_GameInfo_GameName, nameLength);

            }
        }


        public NetInputData(byte[] data)
        {
            buffer = data;
        }
        public NetInputData(NetInputData data)
        {
            buffer = new byte[data.buffer.Length];
            Array.Copy(data.buffer, buffer, buffer.Length);
        }
        public NetInputData(NetCommand command)
        {
            buffer = new byte[NetInputData.CommandPackageSize[(int)command]];
            //填充固定的数据
            buffer[PackageDataIndex_Head1] = PackageHead1;
            buffer[PackageDataIndex_Head2] = PackageHead2;
            buffer[buffer.Length - 2] = PackageLaste2;
            buffer[buffer.Length - 1] = PackageLaste1;

            Command = command;

        }


        //0x01	基础键位同步指令
        public static NetInputData Create_Command_InputKeyUpdate(int playerIndex,int keyValue)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_InputKeyUpdate);
            ret.PlayerIndex = playerIndex;
            ret.Command_InputKeyUpdate_KeyValue = keyValue;
            return ret;
        }
        //0x02	客户端退出封包
        public static NetInputData Create_Command_ClientClose(int playerIndex)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_ClientClose);
            ret.PlayerIndex = playerIndex;
            return ret;
        }
        //0x03	无空闲可分配玩家位
        public static NetInputData Create_Command_PlayersFull()
        {
            return new NetInputData(NetCommand.Command_PlayersFull);
        }
        //0x04	设置客户端玩家索引
        public static NetInputData Create_Command_ResetPlayerIndex(int playerIndex)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_ResetPlayerIndex);
            ret.PlayerIndex = playerIndex;
            return ret;
        }
        //0x05	同步游戏信息封包
        public static NetInputData Create_Command_SetGameDeviceInfo(int deviceSelect,int deviceFunction)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_SetGameDeviceInfo);
            ret.Command_SetGameDeviceInfo_DeviceSelect = (byte)deviceSelect;
            ret.Command_SetGameDeviceInfo_DeviceFunction = (byte)deviceFunction;
            return ret;
        }
        //0x06	同步选位信息状态封包
        public static NetInputData Create_Command_UpdateSelectPositionInfo(int supportPlayers,int positionData)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_UpdateSelectPositionInfo);
            ret.Command_UpdateSelectPositionInfo_SupportPlayers = supportPlayers;
            ret.Command_UpdateSelectPositionInfo_PositionData = positionData;
            return ret;
        }
        //0x07	用户选择一个空闲位
        public static NetInputData Create_Command_SelectFreePosition(int selectPosition)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_SelectFreePosition);
            ret.Command_SelectFreePosition_SelectPosition = selectPosition;
            return ret;
        }
        //0x08	选择位已被占用	
        public static NetInputData Create_Command_SelectPositionCantUse()
        {
            return new NetInputData(NetCommand.Command_SelectPositionCantUse);
        }
        //0x09	用户选位确定
        public static NetInputData Create_Command_SelectPositionOk()
        {
            return new NetInputData(NetCommand.Command_SelectPositionOk);
        }
        //0x10	服务端确定客户端选位成功	
        public static NetInputData Create_Command_SelectPositionOkReback()
        {
            return new NetInputData(NetCommand.Command_SelectPositionOkReback);
        }
        //0x11	手机震动
        public static NetInputData Create_Command_Shake(float time)
        {
            int count = (int)(time / ShakeEveryTime);
            if (count == 0)
                count = 1;
            NetInputData ret = new NetInputData(NetCommand.Command_Shake);
            ret.Command_Shake_ShakeCount = count;
            return ret;
        }
        //0x12	选位信息不相符
        public static NetInputData Create_Command_SelectFreePositionErr()
        {
            return new NetInputData(NetCommand.Command_SelectFreePositionErr);
        }
        //0x13	设备侦测封包
        public static NetInputData Create_Command_ClientSense(int iplistIndex)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_ClientSense);
            ret.Command_Sense_IpListIndex = iplistIndex;
            return ret;
        }
        //0x14	设备侦测封包回执	
        public static NetInputData Create_Command_ServerRebackSense(int iplistIndex)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_ServerRebackSense);
            ret.Command_Sense_IpListIndex = iplistIndex;
            return ret;
        }
        //0x15	服务停止
        public static NetInputData Create_Command_ServerStop()
        {
            return new NetInputData(NetCommand.Command_ServerStop);
        }
        //0x16	同步游戏信息封包
        public static NetInputData Create_Command_UpdateGameInfo(string gameName)
        {
            NetInputData ret = new NetInputData(NetCommand.Command_UpdateGameInfo);
            ret.Command_UpdateGameInfo_GameName = gameName;
            return ret;
        }

        public NetInputData Clone() { return new NetInputData(this); }

        public byte GetData(int index) { return buffer[index]; }
        public void SetData(int index, byte value) { buffer[index] = value; }

    }
}
