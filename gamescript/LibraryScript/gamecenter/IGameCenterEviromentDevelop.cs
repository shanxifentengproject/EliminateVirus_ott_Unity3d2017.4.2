using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class IGameCenterEviromentDevelop : MonoBehaviourInputSupport
{
    private List<string> reportList = new List<string>(8);
    private bool islinkgamecenter = false;
    protected override void Start()
    {
        base.Start();
        reportList.Add("start work!\r\n");
        //请求连接游戏中心
#if !UNITY_EDITOR && UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                int ret = jo.Call<int>("TestFun", 100, 100);
                reportList.Add("TestFun = " + ret + "\r\n");
            }

        }
#endif //UNITY_ANDROID
//#if UNITY_EDITOR || _PlatformCommon

//#else
        
//#endif //UNITY_EDITOR
    }

    protected virtual void Update()
    {
        if (!islinkgamecenter)
        {
            if (GameCenterEviroment.gameCenterLinkStatus == GameCenterEviroment.GameCenterLinkStatus.Status_Succeed)
            {
                reportList.Add("link game center succeed!\r\n");
                islinkgamecenter = true;
            }
            else if (GameCenterEviroment.gameCenterLinkStatus == GameCenterEviroment.GameCenterLinkStatus.Status_Failed)
            {
                reportList.Add("link game center failed!\r\n");
                islinkgamecenter = true;
            }
        }
    }
    public override bool OnInputUpdate()
    {
        if (InputDevice.ButtonOk)
        {
            if (GameCenterEviroment.gameCenterLinkStatus == GameCenterEviroment.GameCenterLinkStatus.Status_Succeed)
            {
                reportList.Add("quest pay money!123\r\n");
                //GameCenterEviroment.currentGameCenterEviroment.OpenPlayerPayMoney((int)IGameCenterEviroment.PlayerPayMoneyItem.PlayerPayMoney_ActiveCharacter, 
                //                        RechargePayCallback);
                return false;
            }
        }
        return true;
    }
    public void RechargePayCallback(int payid, bool issucceed)
    {
        reportList.Add("pay money succeed" + ((IGameCenterEviroment.PlayerPayMoneyItem)payid).ToString() + "\r\n");
    }
    void OnGUI()
    {
        string sMessage = "";
        for (int i = 0; i < reportList.Count; i++)
            sMessage += reportList[i];
        GUI.Label(new Rect(0, 0, 800, 600), sMessage);
    }
}
