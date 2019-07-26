using System;

/*
 * 方程表达式:a*Log(x)+b=y
 * 输入量:x
 * 输出量:y
 * */
struct Logarithmf
{
    public float m_a;
    public float m_b;
    public Logarithmf(float a, float b)
    {
        m_a = a;
        m_b = b;
    }
    public float GetValue(float x)
    {
        //x不能为0，并且x必须大于0
        if (UnityEngine.Mathf.Abs(x - 0.0f) <= UnityEngine.Mathf.Epsilon)
            throw new Exception("Logarithm: x cannot be 0");
        if (x < 0.0)
            throw new Exception("Logarithm: x must be positive");

        return m_a * UnityEngine.Mathf.Log(x) + m_b;
    }
}

[System.Serializable]
class SerLogarithmf
{
    public float m_a;
    public float m_b;
    public SerLogarithmf(float a, float b)
    {
        m_a = a;
        m_b = b;
    }
    public float GetValue(float x)
    {
        //x不能为0，并且x必须大于0
        if (UnityEngine.Mathf.Abs(x - 0.0f) <= UnityEngine.Mathf.Epsilon)
            throw new Exception("Logarithm: x cannot be 0");
        if (x < 0.0)
            throw new Exception("Logarithm: x must be positive");

        return m_a * UnityEngine.Mathf.Log(x) + m_b;
    }
}