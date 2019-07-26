using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniPerformance/UniCameraPerformance/UniCameraPerformanceCookie")]
class UniCameraPerformanceCookie : MonoBehaviourIgnoreGui
{
    public static int PerformanceCookieComparison(UniCameraPerformanceCookie x,UniCameraPerformanceCookie y)
    {
        return (x == y) ? 0 : 1;
    }
    public static ObjectCollectionT<UniCameraPerformanceCookie> performanceCookieTable = new ObjectCollectionT<UniCameraPerformanceCookie>(PerformanceCookieComparison);
    public static void AddCookie(UniCameraPerformanceCookieType type,UniCameraPerformanceCookie cookie)
    {
        performanceCookieTable.Add((uint)type, cookie);
    }
    public static void RemoveCookie(UniCameraPerformanceCookieType type, UniCameraPerformanceCookie cookie)
    {
        performanceCookieTable.Remove((uint)type, cookie);
    }
    public static List<UniCameraPerformanceCookie> GetCookieList(UniCameraPerformanceCookieType type)
    {
        return performanceCookieTable.GetList((uint)type);
    }

    public UniCameraPerformanceCookieType cookieType = UniCameraPerformanceCookieType.Type_Unknow;
    public Behaviour[] behaviourList = null;
    //光晕灯光性能设置
    public enum PerformanceType
    {
        Performance_Init = 0,         //初模式
        Performance_Hight = 1,         //高性能
        Performance_Lower = 2,         //低性能
        Performance_Stop = 3          //这时候关闭
    }
    private PerformanceType m_PerformanceType = PerformanceType.Performance_Init;
    public virtual PerformanceType performanceType
    {
        get { return m_PerformanceType; }
        set
        {
            if (!enabled)//插件没有启用则不做调整
                return;
            if (m_PerformanceType == value)
                return;
            m_PerformanceType = value;
            if (m_PerformanceType == PerformanceType.Performance_Stop)
            {
                for (int i = 0; i < behaviourList.Length;i++ )
                {
                    behaviourList[i].enabled = false;
                }
            }
            else if (m_PerformanceType == PerformanceType.Performance_Hight ||
                    m_PerformanceType == PerformanceType.Performance_Lower)
            {
                for (int i = 0; i < behaviourList.Length; i++)
                {
                    behaviourList[i].enabled = true;
                }
            }
            OnPerformanceChange();
        }
    }
    public virtual void OnPerformanceChange()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        UniCameraPerformanceCookie.AddCookie(cookieType, this);
    }
    protected virtual void OnDestroy()
    {
        UniCameraPerformanceCookie.RemoveCookie(cookieType, this);
    }

}
