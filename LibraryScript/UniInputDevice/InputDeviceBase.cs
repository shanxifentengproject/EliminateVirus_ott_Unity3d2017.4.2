using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FtGameInput;
abstract class InputDeviceBase : MonoBehaviourIgnoreGui
{
    //输入操作模式
    public enum InputPlayerMode
    {
        Mode_1P = 1,
        Mode_2P = 2,
    }
    //每一位输入的标识
    public enum InputPlayerType
    {
        Type_P1 = 0,
        Type_P2 = 1,
    }
    [System.Serializable]
    public class InputPlayerInfoData
    {
        //支持的设备清单
        public InputPlayer.DeviceType[] devicelist;
        //是否支持鼠标
        public bool isSupportMouse = false;
        //鼠标移动速度
        public Vector3 mouseSensitivity = new Vector3(1.0f, 1.0f);
    }
    //单人输入的时候的配置
    public InputPlayerInfoData singlePlayerInfo;
    //多人输入的配置
    public InputPlayerInfoData[] multiplayerPlayerInfo;
    //默认的输入模式
    public InputPlayerMode defineInputMode = InputPlayerMode.Mode_1P;


    //当前的输入模式
    private InputPlayerMode currentInputMode;
    private FtGameInput.InputPlayer[] inputPlayer = null;
    public InputPlayerMode CurrentInputMode
    {
        get
        {
            return currentInputMode;
        }
        set
        {
            currentInputMode = value;
            switch(currentInputMode)
            {
                case InputPlayerMode.Mode_1P:
                    {
                        inputPlayer = new InputPlayer[1];
                        inputPlayer[(int)InputPlayerType.Type_P1] = new InputPlayer(0);
                        inputPlayer[(int)InputPlayerType.Type_P1].Initialization(singlePlayerInfo.devicelist,
                            singlePlayerInfo.isSupportMouse,
                            singlePlayerInfo.mouseSensitivity);
                    }
                    break;
                case InputPlayerMode.Mode_2P:
                    {
                        inputPlayer = new InputPlayer[2];
                        for (int i = 0;i< inputPlayer.Length;i++)
                        {
                            inputPlayer[i] = new InputPlayer(i);
                            inputPlayer[i].Initialization(multiplayerPlayerInfo[i].devicelist,
                                multiplayerPlayerInfo[i].isSupportMouse,
                                multiplayerPlayerInfo[i].mouseSensitivity);
                        }
                    }
                    break;
            }
        }
    }

    public FtGameInput.InputPlayer InputP1
    {
        get
        {
            return inputPlayer[(int)InputPlayerType.Type_P1];
        }
    }
    public FtGameInput.InputPlayer InputP2
    {
        get
        {
            return inputPlayer[(int)InputPlayerType.Type_P2];
        }
    }

    //是否支持网络输入，如果游戏不用支持星空游戏手柄就关闭，下面的都不用填写了
    public bool IsSupportNetInput;
    //使用的端口,就用这个端口，APP端也用的是这个端口
    public int netInputPort = 8903;
    //支持的玩家数，针对游戏性质如实填写
    public int supportPlayers;
    //给予网络输入设备的游戏名，这个名字会显示在星空游戏手柄的界面上
    public string netInputGameName;
    //网络输入支持的设备,根据不同的游戏有不同的选择。当前游戏使用的手柄
    public NetInputGameDeviceType netInputDevice = NetInputGameDeviceType.Type_Joystick;
    //是否支持震动，启用震动后，由游戏决定什么时候可以震动
    public bool isShake;
    public int netInputDeviceFunction
    {
        get
        {
            int ret = 0;
            if (isShake)
                ret |= NetInputDefine.DeviceFunction_Shake;
            return ret;
        }
    }

    private InputServer inputServer = null;
    //二维码预置
    private QRCodeCreator qrcodeCreator = null;

    public bool OpenQRCodeCreator
    {
        get
        {
            return qrcodeCreator == null;
        }
        set
        {
            if (!IsSupportNetInput)
                return;
            if (value && qrcodeCreator == null)
            {
                GameObject obj = UniGameResources.currentUniGameResources.LoadResource_Prefabs("QRcodePlane.prefab");
                qrcodeCreator = obj.GetComponent<QRCodeCreator>();
#if !UNITY_EDITOR && UNITY_ANDROID
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    qrcodeCreator.text_weixin = jo.Call<string>("getQRCode_Weixing_URL");
                    QRCodeCreator.QRCodeAPPDownPlatform  platform= (QRCodeCreator.QRCodeAPPDownPlatform)jo.Call<int>("QRCodeAppDownPlatform");
                    qrcodeCreator.qrController_Weixin.e_QRLogoTex = qrcodeCreator.weixinPlatformIcon[(int)platform];
                    qrcodeCreator.reportText_Weixin1.text = jo.Call<string>("QRCodeReportText_WeiXin_1");
                    qrcodeCreator.reportText_Weixin2.text = jo.Call<string>("QRCodeReportText_WeiXin_2");
                    qrcodeCreator.reportText_gamePad1.text = jo.Call<string>("QRCodeReportText_GamePad_1");
                    qrcodeCreator.reportText_gamePad2.text = jo.Call<string>("QRCodeReportText_GamePad_2");
                 }

            }
#else
                //提供一个自己的下载地址
                //qrcodeCreator.text_weixin = "http://iptv.sxfenteng.com/StarGamepad/StarGamepad.apk";
                qrcodeCreator.text_weixin = "http://gamepad.sxfenteng.com/";
                //qrcodeCreator.text_weixin = "https://www.baidu.com/";
                qrcodeCreator.qrController_Weixin.e_QRLogoTex = qrcodeCreator.weixinPlatformIcon[(int)QRCodeCreator.QRCodeAPPDownPlatform.Platform_Explorer];
                qrcodeCreator.reportText_Weixin1.text = "浏览器扫一扫下载";
                qrcodeCreator.reportText_Weixin2.text = "星空游戏手柄APP";
                qrcodeCreator.reportText_gamePad1.text = "星空游戏手柄扫一扫";
                qrcodeCreator.reportText_gamePad2.text = "连接游戏";
#endif //UNITY_ANDROID
                //设置本机IP地址格式
                qrcodeCreator.text_gamepad = NetInputDefine.CreateQRCodeString();
            }
            else if (!value && qrcodeCreator != null)
            {
                UnityEngine.Object.DestroyObject(qrcodeCreator.gameObject);
            }
        }
    }

   
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        InputDeviceBase.currentInputDevice = this;
        //尝试创建网络服务
        //首先创建网络服务，这样，网络玩家的位置对象就分配了，这样，后面在设备对象里就能正确绑定了
        OpenInputServer();
        CurrentInputMode = defineInputMode;
        

    }

    protected virtual void OnDestroy()
    {
        CloseInputServer();
    }
    //提供一套基础键位函数需要重写，在类库内使用
    public static InputDeviceBase currentInputDevice = null;
    //刷新队列
    protected List<MonoBehaviourInputSupport> InputUpdateList = new List<MonoBehaviourInputSupport>(128);

    //加入刷新队列
    //排序规则，从小到大排，越晚进队列的，越往后排
    public void AddInputUpdateList(MonoBehaviourInputSupport obj)
    {
        for (int i=0;i<InputUpdateList.Count;i++)
        {
            if ((int)obj.CurrentInputUpdateLevel < (int)InputUpdateList[i].CurrentInputUpdateLevel)
            {
                InputUpdateList.Insert(i, obj);
                return;
            }
        }
        //队列里没元素，或者都比我小
        InputUpdateList.Add(obj);
    }
    //从刷新队列内删除掉
    public void DelInputUpdateList(MonoBehaviourInputSupport obj)
    {
        InputUpdateList.Remove(obj);
    }

    //锁定输入响应
    private bool isInputUpdateLock = false;
    public bool IsInputUpdateLock
    {
        get
        {
            return isInputUpdateLock;
        }
        set
        {
            isInputUpdateLock = value;
        }
    }
    private void InputUpdateUnLock()
    {
        IsInputUpdateLock = false;
    }
    public void InvokeInputUpdateUnLock(float time)
    {
        if (IsInvoking("InputUpdateUnLock"))
            CancelInvoke("InputUpdateUnLock");
        Invoke("InputUpdateUnLock", time);
    }
    //处理刷新对象的刷新
    protected virtual void Update()
    {
        //刷新网络服务的部分
        if (inputServer != null)
        {
            inputServer.Update();
        }
        foreach (FtGameInput.InputPlayer p in inputPlayer)
        {
            p.Update();
        }
        if (!IsInputUpdateLock)
        {
            foreach (MonoBehaviourInputSupport obj in InputUpdateList)
            {
                if (!obj.gameObject.activeSelf ||
                        !obj.enabled)
                    continue;
                if (!obj.OnInputUpdate())
                    return;
            }
        }
            
 #if !UNITY_EDITOR && NETINPUT_DEVELOP && UNITY_ANDROID
        if (InputP1.ButtonMenuDown)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call("ShowTest1Activity");
                }

            }
        }
#endif//NETINPUT_DEVELOP
    }


    protected void OpenInputServer()
    {
        if (!IsSupportNetInput)
            return;
        if (inputServer == null)
        {
            inputServer = new InputServer(netInputPort, supportPlayers,
                netInputGameName, netInputDevice, netInputDeviceFunction);
        }
        inputServer.Start();

    }
    protected void CloseInputServer()
    {
        if (!IsSupportNetInput)
            return;
        if (inputServer != null)
        {
            inputServer.Stop();
        }
    }

    public void JavaJnionStart(string s)
    {
        OpenInputServer();
        //无论如果关闭合理挂起
        NetInputDefine.Rational_Hang_Up = false;
    }

    public void JavaJnionPause(string s)
    {
        if (!NetInputDefine.Rational_Hang_Up)
        {
            CloseInputServer();
        }
        
    }

    public void JavaJnionStop(string s)
    {
        if (!NetInputDefine.Rational_Hang_Up)
        {
            CloseInputServer();
        }
            
    }

    public void JavaJnionDestroy(string s)
    {
        //这里必须关闭
        CloseInputServer();
    }

    public void JavaJnionRestart(string s)
    {
        OpenInputServer();
        //无论如果关闭合理挂起
        NetInputDefine.Rational_Hang_Up = false;
    }

    public void JavaJnionResume(string s)
    {
        OpenInputServer();
        //无论如果关闭合理挂起
        NetInputDefine.Rational_Hang_Up = false;
    }
}
