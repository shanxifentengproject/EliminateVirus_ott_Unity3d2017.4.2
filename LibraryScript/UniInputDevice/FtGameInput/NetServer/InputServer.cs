using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FtGameInput
{
    class InputServer : InputUdpServerBase
    {

        private static InputServer myInstance = null;
        public static InputServer Instance
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
        //游戏名称
        private string gameName;
        //当前游戏设置使用的设备
        public NetInputGameDeviceType gameDeviceSelect = NetInputGameDeviceType.Type_Joystick;
        //当前游戏设置使用的设备功能
        public int gameDeviceFunction = 0x00;

        //网络玩家列表
        protected NetInputPlayer[] netPlayerList = null;
        public int SupportPlayers { get { return netPlayerList.Length; } }
        public NetInputPlayer getNetPlayer(int index)
        {
            if (index < 0 || index >= netPlayerList.Length)
                return null;
            return netPlayerList[index];
        }
        //是否还有空闲的玩家位置
        public int freePlayerPosition
        {
            get
            {
                for (int i = 0;i< netPlayerList.Length;i++)
                {
                    if (!netPlayerList[i].isActive)
                        return i;
                }
                return -1;
            }
        }
        //获取有多少个空闲位置
        public int freePlayerPositionCount
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < netPlayerList.Length; i++)
                {
                    if (!netPlayerList[i].isActive)
                        ret += 1;
                }
                return ret;
            }
        }

        //选位计算服务
        private struct SelectPositionData
        {
            public SelectPositionData(IPEndPoint ip,int selectposition)
            {
                remoteIp = ip;
                selectPosition = selectposition;
            }
            public IPEndPoint remoteIp;
            public int selectPosition;
        }
        private List<SelectPositionData> selectPositionList = new List<SelectPositionData>(8);
        private void AddPlayerToSelectList(IPEndPoint remoteIp)
        {
            selectPositionList.Add(new SelectPositionData(remoteIp, -1));
        }
        private int FindPlayerFromSelectList(IPEndPoint remoteIp)
        {
            for (int i = 0;i< selectPositionList.Count;i++)
            {
                if (selectPositionList[i].remoteIp.Equals(remoteIp))
                    return i;
            }
            return -1;
        }
        private void RemovePlayerFromSelectList(IPEndPoint remoteIp)
        {
            int index = FindPlayerFromSelectList(remoteIp);
            if (index == -1)
                return;
            selectPositionList.RemoveAt(index);
        }
        private int AccountSelectPositionInfo()
        {
            int ret = 0x00000000;
            //从用户列表内找出空闲的位置，然后在选位信息内查取，看那些位置还没被选
            for (int i = 0;i< netPlayerList.Length;i++)
            {
                if (netPlayerList[i].isActive)
                {
                    ret |= (1 << i);
                    continue;
                }
                    
                bool isselect = false;
                for (int j = 0;j< selectPositionList.Count;j++)
                {
                    if (selectPositionList[j].selectPosition == i)
                    {
                        isselect = true;
                        break;
                    }
                }
                if (isselect)
                {
                    ret |= (1 << i);
                }
            }
            return ret;
        }
        


        public InputServer(int listenPort,int supportPlayers,
            string gamename, NetInputGameDeviceType gameDvice,int deviceFunction)
            :base(listenPort)
        {
            InputServer.Instance = this;
            netPlayerList = new NetInputPlayer[supportPlayers];
            for (int i = 0;i< netPlayerList.Length;i++)
            {
                netPlayerList[i] = new NetInputPlayer(this, i);
            }

            gameName = gamename;
            gameDeviceSelect = gameDvice;
            gameDeviceFunction = deviceFunction;
        }

        protected override void OnStop()
        {
            NetInputData data = NetInputData.Create_Command_ServerStop();
            for (int i = 0; i < netPlayerList.Length; i++)
            {
                //发送服务端关闭封包
                if (netPlayerList[i].isActive)
                {
                    netPlayerList[i].Send(data);
                }
                netPlayerList[i].SetActive(false, null);
            }
        }




        private void CommandHandle_ClientClose(IPEndPoint remoteIp,NetInputData package)
        {
            //携带的玩家索引为空
            if (package.PlayerIndex == NetInputData.PlayerIndexEmpty)
                return;
            Monitor.Enter(netPlayerList);
            try
            {
                //尝试清除一次分配列表内的信息
                RemovePlayerFromSelectList(remoteIp);
                NetInputPlayer player = getNetPlayer(package.PlayerIndex);
                //检测这个用户是否激活了。并且和客户端是匹配的
                if (player != null &&
                    player.isActive && 
                    player.remoteIp != null &&
                     player.remoteIp.Equals(remoteIp))
                {
                    //注销这个客户端位置
                    player.SetActive(false, null);
                }
            }
            catch (System.Exception ex)
            {

            }
            Monitor.Exit(netPlayerList);
        }

        //分配一个位置给客户端
        private void AllocationPlayerPosition(IPEndPoint remoteIp,int index)
        {
            NetInputPlayer player = netPlayerList[index];
            player.SetActive(true, remoteIp);
            player.Send(NetInputData.Create_Command_ResetPlayerIndex(index));
            //设定游戏设备信息
            player.Send(NetInputData.Create_Command_SetGameDeviceInfo((int)gameDeviceSelect,
                (int)gameDeviceFunction));
        }


        //由于在处理键位刷新消息的时候，后续的消息已经持续发送到服务端了，会导致消息重复处理
        //这里使用列表保存客户端的关联信息，设定一定的时间内锁定不再响应客户端的数据请求
        private struct RemoteIpLocker
        {
            public IPEndPoint remoteIp;
            public FTLibrary.Time.TimeLocker locker;
        }
        private const int remoteIpLockTime = 2000;

        private List<RemoteIpLocker> remoteIpLockerList = new List<RemoteIpLocker>(8);
        private int FindLockRemoteIp(IPEndPoint remoteIp)
        {
            for (int i = 0;i<remoteIpLockerList.Count;i++)
            {
                if (remoteIpLockerList[i].remoteIp.Equals(remoteIp))
                    return i;
            }
            return -1;

        }
        private void LockRemoteIp(IPEndPoint remoteIp)
        {
            int index = FindLockRemoteIp(remoteIp);
            if (index == -1)
            {
                RemoteIpLocker data = new RemoteIpLocker();
                data.remoteIp = remoteIp;
                data.locker = new FTLibrary.Time.TimeLocker(remoteIpLockTime);
                data.locker.IsLocked = true;
                remoteIpLockerList.Add(data);
            }
            else
            {
                RemoteIpLocker data = remoteIpLockerList[index];
                data.locker.IsLocked = true;
                remoteIpLockerList[index] = data;
            }
        }

        private bool IsRemoteIpLocked(IPEndPoint remoteIp)
        {
            int index = FindLockRemoteIp(remoteIp);
            if (index == -1)
                return false;
            if (!remoteIpLockerList[index].locker.IsLocked)
            {
                remoteIpLockerList.RemoveAt(index);
                return false;
            }
            return true;
        }

        private void CommandHandle_InputKeyUpdate(IPEndPoint remoteIp, NetInputData package)
        {
            Monitor.Enter(netPlayerList);
            try
            {
                //如果客户端的玩家索引为空，未分配
                //检测分配的索引是否溢出当前用户总数
                if (package.PlayerIndex == NetInputData.PlayerIndexEmpty ||
                     package.PlayerIndex >= SupportPlayers)
                {
                    if (IsRemoteIpLocked(remoteIp))
                        return;
                    
                    //同步游戏信息
                    this.Send(remoteIp, NetInputData.Create_Command_UpdateGameInfo(gameName));
                    int freeIndex = freePlayerPosition;
                    //没有空闲的位置了
                    if (freeIndex == -1)
                    {
                        this.Send(remoteIp, NetInputData.Create_Command_PlayersFull());
                    }
                    //游戏只支持一名玩家，或者，游戏剩余的位置就一个位置了，没必要选位置了
                    else if (SupportPlayers == 1 || freePlayerPositionCount == 1)
                    {
                        AllocationPlayerPosition(remoteIp, freeIndex);
                    }
                    //加入选位清单，并发生选位请求
                    else
                    {
                        //把这个用户加入选位列表
                        AddPlayerToSelectList(remoteIp);
                        //向客户端发起选位信息
                        this.Send(remoteIp,
                              NetInputData.Create_Command_UpdateSelectPositionInfo(SupportPlayers,
                              AccountSelectPositionInfo()));
                    }
                    LockRemoteIp(remoteIp);
                }
                else
                {
                    NetInputPlayer player = getNetPlayer(package.PlayerIndex);
                    if (!player.isActive)
                    {
                        //同步游戏信息
                        this.Send(remoteIp, NetInputData.Create_Command_UpdateGameInfo(gameName));
                        //分配这个位置给用户
                        AllocationPlayerPosition(remoteIp, package.PlayerIndex);
                    }
                    //这个位置已经被其他人激活了
                    else if (!player.remoteIp.Equals(remoteIp))
                    {
                        if (IsRemoteIpLocked(remoteIp))
                            return;
                        //同步游戏信息
                        this.Send(remoteIp, NetInputData.Create_Command_UpdateGameInfo(gameName));
                        int freeIndex = freePlayerPosition;
                        //没有空闲的位置了
                        if (freeIndex == -1)
                        {
                            this.Send(remoteIp, NetInputData.Create_Command_PlayersFull());
                        }
                        //游戏只支持一名玩家，或者，游戏剩余的位置就一个位置了，没必要选位置了
                        else if (SupportPlayers == 1 || freePlayerPositionCount == 1)
                        {
                            AllocationPlayerPosition(remoteIp, freeIndex);
                        }
                        //加入选位清单，并发生选位请求
                        else
                        {
                            //把这个用户加入选位列表
                            AddPlayerToSelectList(remoteIp);
                            //向客户端发起选位信息
                            this.Send(remoteIp,
                                  NetInputData.Create_Command_UpdateSelectPositionInfo(SupportPlayers,
                                  AccountSelectPositionInfo()));
                        }
                        LockRemoteIp(remoteIp);
                    }
                    //这个封包就是我的封包被确认，开始处理
                    else
                    {
                        OnHandleReceiveCommand(player, package);
                    }

                        
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                Monitor.Exit(netPlayerList);
            }
            
        }

        private void CommandHandle_SelectFreePosition(IPEndPoint remoteIp, NetInputData package)
        {
            Monitor.Enter(netPlayerList);
            try
            {
                //检测是否在选位列表内
                int listIndex = FindPlayerFromSelectList(remoteIp);
                if (listIndex == -1)
                {
                    this.Send(remoteIp, NetInputData.Create_Command_SelectFreePositionErr());
                    return;
                }
                int selectpositionindex = package.Command_SelectFreePosition_SelectPosition;
                int positionInfo = AccountSelectPositionInfo();
                //这个位置已经被占用
                if ((positionInfo & (1 << selectpositionindex)) == (1 << selectpositionindex))
                {
                    this.Send(remoteIp, NetInputData.Create_Command_SelectPositionCantUse());
                    return;
                }
                //标记这个位置占用
                SelectPositionData data = selectPositionList[listIndex];
                data.selectPosition = selectpositionindex;
                selectPositionList[listIndex] = data;
                //通知客户端选择这个位置成功了
                NetInputData retpackage = package.Clone();
                retpackage.Command = NetInputData.NetCommand.Command_SelectFreePositionReback;
                this.Send(remoteIp, retpackage);
                //把新的信息同步给所有选位客户端
                retpackage = NetInputData.Create_Command_UpdateSelectPositionInfo(
                        SupportPlayers, AccountSelectPositionInfo());
                for (int i = 0; i < selectPositionList.Count; i++)
                {
                    this.Send(selectPositionList[i].remoteIp, retpackage);
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                Monitor.Exit(netPlayerList);
            }
            
        }
        private void CommandHandle_SelectPositionOk(IPEndPoint remoteIp, NetInputData package)
        {
            Monitor.Enter(netPlayerList);
            try
            {
                //检测是否在选位列表内
                int listIndex = FindPlayerFromSelectList(remoteIp);
                if (listIndex == -1 || selectPositionList[listIndex].selectPosition == -1)
                {
                    return;
                }
                int selectPosition = selectPositionList[listIndex].selectPosition;
                NetInputPlayer player = getNetPlayer(selectPosition);
                if (player == null || player.isActive)
                    return;
                //从分配列表内清除这个角色
                RemovePlayerFromSelectList(remoteIp);
                this.Send(remoteIp, NetInputData.Create_Command_SelectPositionOkReback());
                AllocationPlayerPosition(remoteIp, selectPosition);
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                Monitor.Exit(netPlayerList);
            }
        }
        protected override void Receive(IPEndPoint remoteIp, byte[] data)
        {
            NetInputData package = new NetInputData(data);
            if (!package.IsValid)
                return;
            switch(package.Command)
            {
                case NetInputData.NetCommand.Command_InputKeyUpdate:
                    CommandHandle_InputKeyUpdate(remoteIp, package);
                    break;
                case NetInputData.NetCommand.Command_ClientClose:
                    CommandHandle_ClientClose(remoteIp, package);
                    break;
                case NetInputData.NetCommand.Command_SelectFreePosition:
                    CommandHandle_SelectFreePosition(remoteIp, package);
                    break;
                case NetInputData.NetCommand.Command_SelectPositionOk:
                    CommandHandle_SelectPositionOk(remoteIp, package);
                    break;
                case NetInputData.NetCommand.Command_ClientSense:
                    this.Send(remoteIp, NetInputData.Create_Command_ServerRebackSense(package.Command_Sense_IpListIndex));
                    break;
            }
            
        }

        protected virtual void OnHandleReceiveCommand(NetInputPlayer player,NetInputData package)
        {
            player.activeTime = 0.0f;
            switch (package.Command)
            {
                case NetInputData.NetCommand.Command_InputKeyUpdate:
                    player.netKeyValue = package.Command_InputKeyUpdate_KeyValue;
                    break;
            }
        }

        public void Update()
        {
            Monitor.Enter(netPlayerList);
            try
            {
                for (int i = 0; i < netPlayerList.Length; i++)
                {
                    if (netPlayerList[i].OpenVirtualJoystickSketchMap != netPlayerList[i].isActive)
                    {
                        netPlayerList[i].OpenVirtualJoystickSketchMap = netPlayerList[i].isActive;
                    }
                    if (netPlayerList[i].isActive)
                    {
                        netPlayerList[i].Update();
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
            Monitor.Exit(netPlayerList);
        }
    }
}
