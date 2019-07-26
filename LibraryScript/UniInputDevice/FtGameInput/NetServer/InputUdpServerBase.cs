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
    abstract class InputUdpServerBase
    {
        public int port { set; get; }
        public static IPAddress localip
        {
            get
            {
                try
                {
                    IPAddress[] ipadrlist = Dns.GetHostAddresses(Dns.GetHostName());
                    foreach (IPAddress ipa in ipadrlist)
                    {
                        if (ipa.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ipa;
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        //接受udp socket
        UdpClient udpServer = null;
        //接受回调节点
        IAsyncResult procHeadReceive = null;
        public bool IsDo
        {
            get
            {
                return (udpServer != null);
            }
        }

        public InputUdpServerBase(int listenPort)
        {
            port = listenPort;
        }

        public bool Start()
        {
            try
            {
                if (udpServer != null)
                    return true;
                //创建网络连接
                udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, port));
                //必须监听广播消息才可以收到
                udpServer.EnableBroadcast = true;
                //开始接收数据的过程
                procHeadReceive = udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), null);

            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public void Stop()
        {
            OnStop();
            try
            {
                if (udpServer != null)
                {
                    if (procHeadReceive != null)
                    {
                        procHeadReceive.AsyncWaitHandle.Close();
                        //udpServer.EndReceive(procHeadReceive, ref remoteIp);
                        procHeadReceive = null;
                    }
                    udpServer.Close();
                    udpServer = null;
                }
            }
            catch (System.Exception ex)
            {
                
            }
            
        }


        protected abstract void OnStop();
        protected abstract void Receive(IPEndPoint remoteIp, byte[] data);


        private IPEndPoint tempRemoteIp = new IPEndPoint(IPAddress.Any, 0);
        private void ReceiveCallback(IAsyncResult ar)
        {
            //这里不再接收到时间后立刻启动一个新监听是因为，本身处理数据的时候
            //就会线程互斥，所以，接受的再快，都会卡在处理的地方
            procHeadReceive = null;
            try
            {
                //接受这次传输的数据
                byte[] receiveBytes = udpServer.EndReceive(ar, ref tempRemoteIp);
                Receive(tempRemoteIp, receiveBytes);
            }
            catch (System.Exception ex)
            {

            }
            try
            {
                //再次启动一个监听
                procHeadReceive = udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
            catch (System.Exception ex)
            {

            }
        }


        public void Send(IPEndPoint remoteIp, byte[] data)
        {
            if (udpServer != null)
            {
                udpServer.Send(data, data.Length, remoteIp);
            }
        }
        public void Send(IPEndPoint remoteIp, NetInputData data)
        {
            if (udpServer != null)
            {
                udpServer.Send(data.buffer, data.buffer.Length, remoteIp);
            }
        }
    }
}
