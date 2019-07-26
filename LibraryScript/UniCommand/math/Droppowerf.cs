using System;
struct Droppowerf
{
    //可以取任何值
    public float m_a;
    //可以取-∞到∞，不能为1
    public float m_b;
    // 默认为1.0，不能为0
    public float m_c;
    //限制最小量
    public float m_d;
    public Droppowerf(float a, float b, float c, float d)
    {
        m_a = a;
        m_b = b;
        m_c = c;
        m_d = d;
    }
    public float GetValue(float x)
    {
        return m_a * UnityEngine.Mathf.Pow(m_b, x - m_c) + (m_d - m_a);
    }
}


[System.Serializable]
class SerDroppowerf
{
    //可以取任何值
    public float m_a;
    //可以取-∞到∞，不能为1
    public float m_b;
    // 默认为1.0，不能为0
    public float m_c;
    //限制最小量
    public float m_d;
    public SerDroppowerf(float a, float b, float c, float d)
    {
        m_a = a;
        m_b = b;
        m_c = c;
        m_d = d;
    }
    public float GetValue(float x)
    {
        return m_a * UnityEngine.Mathf.Pow(m_b, x - m_c) + (m_d - m_a);
    }
}
