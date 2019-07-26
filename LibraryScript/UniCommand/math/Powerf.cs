using System;

/*
 * 曲线方程为：y=a*x^b
 * 输入量为：x
 * 输出量为: y
 * 
 * */
struct Powerf
{
    public float m_a;
    public float m_b;
    public Powerf(float a, float b)
    {
        m_a = a;
        m_b = b;
    }
    public float GetValue(float x)
    {
        //x不能为0，并且x必须大于0
        if (UnityEngine.Mathf.Abs(x - 1.0f) <= UnityEngine.Mathf.Epsilon)
            throw new Exception("Power: x cannot be 1.0");
        if (x < 0.0f)
            throw new Exception("Power: x must be positive");

        return m_a * UnityEngine.Mathf.Pow(x, m_b);
    }
}


[System.Serializable]
class SerPowerf
{
    public float m_a;
    public float m_b;
    public SerPowerf(float a, float b)
    {
        m_a = a;
        m_b = b;
    }
    public float GetValue(float x)
    {
        //x不能为0，并且x必须大于0
        if (UnityEngine.Mathf.Abs(x - 1.0f) <= UnityEngine.Mathf.Epsilon)
            throw new Exception("Power: x cannot be 1.0");
        if (x < 0.0f)
            throw new Exception("Power: x must be positive");

        return m_a * UnityEngine.Mathf.Pow(x, m_b);
    }
}