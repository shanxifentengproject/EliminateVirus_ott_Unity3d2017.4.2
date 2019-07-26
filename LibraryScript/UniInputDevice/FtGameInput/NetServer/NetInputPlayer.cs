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
    class NetInputPlayer
    {
        //server对象
        internal InputServer server = null;
        //连接玩家的回执地址
        internal IPEndPoint remoteIp = null;
        //当前我在的索引
        internal int playerIndex;
        //同步的键值
        internal int netKeyValue = 0;
        //激活的时间，每次进行数据处理的时候都会清除这个值
        //当这个值累计的到一定时间，说明这个用户已经解除绑定了
        internal float activeTime = 0.0f;
        public bool isActive { set; get; }

        private VirtualJoystickSketchMap joystickSketchMap = null;
        public bool OpenVirtualJoystickSketchMap
        {
            get
            {
                return joystickSketchMap != null;
            }
            set
            {
                if (joystickSketchMap != null)
                {
                    UnityEngine.Object.DestroyObject(joystickSketchMap.gameObject);
                    joystickSketchMap = null;
                }
                if (value)
                {
                    UnityEngine.GameObject obj = UniGameResources.currentUniGameResources.LoadResource_Prefabs(
                        string.Format("VirtualJoystickP{0}.prefab", playerIndex + 1));
                    joystickSketchMap = obj.GetComponent<VirtualJoystickSketchMap>();
                }
            }
        }

        //键位状态
        protected NetInputKeyStatus[] keyStatus = new NetInputKeyStatus[(int)NetInputKeyCode.Key_Count];


        public NetInputPlayer(InputServer s,int playerindex)
        {
            server = s;
            playerIndex = playerindex;
            for (int i = 0;i< keyStatus.Length;i++)
            {
                keyStatus[i] = NetInputKeyStatus.Status_Nothing;
            }
        }




        public void SetActive(bool isactive, IPEndPoint remoteip)
        {
            remoteIp = remoteip;
            if (isActive != isactive)
            {
                isActive = isactive;
                if (!isActive)
                {
                    netKeyValue = 0;
                    for (int i = 0; i < keyStatus.Length; i++)
                    {
                        keyStatus[i] = NetInputKeyStatus.Status_Nothing;
                    }
                }
            }
        }

        public void Send(byte[] data)
        {
            if (isActive)
            {
                server.Send(remoteIp, data);
            }
        }
        public void Send(NetInputData data)
        {
            if (isActive)
            {
                server.Send(remoteIp, data);
            }
        }

        public void Update()
        {
            activeTime += UnityEngine.Time.deltaTime;
            if (activeTime >= NetInputDefine.RelieveActivePlayerTime)
            {
                SetActive(false, null);
                return;
            }
            for (int i = 0;i< keyStatus.Length;i++)
            {
                int keyvalue = (1 << i);
                //键位被按下了
                if ((keyvalue & netKeyValue) == keyvalue)
                {
                    if (keyStatus[i] == NetInputKeyStatus.Status_Nothing)
                    {
                        keyStatus[i] = NetInputKeyStatus.Status_Down;
                    }
                    else if (keyStatus[i] == NetInputKeyStatus.Status_Down)
                    {
                        keyStatus[i] = NetInputKeyStatus.Status_Press;
                    }
                }
                else
                {
                    if (keyStatus[i] == NetInputKeyStatus.Status_Down ||
                         keyStatus[i] == NetInputKeyStatus.Status_Press)
                    {
                        keyStatus[i] = NetInputKeyStatus.Status_Up;
                    }
                    else if (keyStatus[i] == NetInputKeyStatus.Status_Up)
                    {
                        keyStatus[i] = NetInputKeyStatus.Status_Nothing;
                    }
                }
                if (joystickSketchMap != null)
                {
                    joystickSketchMap.SetKeyStatus((NetInputKeyCode)i, keyStatus[i]);
                }
            }
        }

        public bool GetKeyDown(NetInputKeyCode code)
        {
            return keyStatus[(int)code] == NetInputKeyStatus.Status_Down;
        }
        public bool GetKeyPress(NetInputKeyCode code)
        {
            return keyStatus[(int)code] == NetInputKeyStatus.Status_Press;
        }
        public bool GetKeyUp(NetInputKeyCode code)
        {
            return keyStatus[(int)code] == NetInputKeyStatus.Status_Up;
        }

        public void Shake(float time)
        {
            if (isActive)
            {
                this.Send(NetInputData.Create_Command_Shake(time));
            }
            
        }
    }
}
