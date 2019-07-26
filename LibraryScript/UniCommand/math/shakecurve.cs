using System;
//振动函数，使用sin函数 当前振动位移=振幅*Sin((当前周期内时间/(1000/周期))*2*PI)
struct ShakeSinCurve
{
    //振幅，单位：米
    public float m_Swing;
    //周期时间，单位秒，每周期需要的时间
    public float m_CycleTime;
    public ShakeSinCurve(float swing, float cycletime)
    {
        m_Swing = swing;
        m_CycleTime = cycletime;
    }
    public float GetValue(float time)
    {
        return m_Swing * UnityEngine.Mathf.Sin((time % m_CycleTime) * 2.0f * UnityEngine.Mathf.PI);
    }
}

[System.Serializable]
class SerShakeSinCurve
{
    //振幅，单位：米
    public float m_Swing;
    //周期时间，单位秒，每周期需要的时间
    public float m_CycleTime;
    public SerShakeSinCurve(float swing, float cycletime)
    {
        m_Swing = swing;
        m_CycleTime = cycletime;
    }
    public float GetValue(float time)
    {
        return m_Swing * UnityEngine.Mathf.Sin((time % m_CycleTime) * 2.0f * UnityEngine.Mathf.PI);
    }
}