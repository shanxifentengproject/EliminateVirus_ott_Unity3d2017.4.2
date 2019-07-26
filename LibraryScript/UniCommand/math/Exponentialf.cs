using System;
/*
 * 曲线方程为：y=a*b^(x/c)
 * 输入量为：x
 * 输出量为: y
 * 
 * */
struct Exponentialf
{
    //可以取任何值
    public float m_a;
    //可以取-∞到∞，不能为1
    public float m_b;
    // 默认为1.0，不能为0
    public float m_c;
    public Exponentialf(float a, float b, float c)
    {
        m_a = a;
        //b不能为1
        if (UnityEngine.Mathf.Abs(b - 1.0f) > UnityEngine.Mathf.Epsilon)
            m_b = b;
        else
            throw new Exception("Exponential: b cannot equal to 1.0");
        //c不能为0
        if (UnityEngine.Mathf.Abs(c - 0.0f) > UnityEngine.Mathf.Epsilon)
            m_c = c;
        else
            throw new Exception("Exponential: c cannot equal to 0");
    }

    //x可以取任意值
    public float GetValue(float x)
    {
        return m_a * UnityEngine.Mathf.Pow(m_b, x / m_c);
    }
}


[System.Serializable]
class SerExponentialf
{
    //可以取任何值
    public float m_a;
    //可以取-∞到∞，不能为1
    public float m_b;
    // 默认为1.0，不能为0
    public float m_c;
    public SerExponentialf(float a, float b, float c)
    {
        m_a = a;
        //b不能为1
        if (UnityEngine.Mathf.Abs(b - 1.0f) > UnityEngine.Mathf.Epsilon)
            m_b = b;
        else
            throw new Exception("Exponential: b cannot equal to 1.0");
        //c不能为0
        if (UnityEngine.Mathf.Abs(c - 0.0f) > UnityEngine.Mathf.Epsilon)
            m_c = c;
        else
            throw new Exception("Exponential: c cannot equal to 0");
    }

    //x可以取任意值
    public float GetValue(float x)
    {
        return m_a * UnityEngine.Mathf.Pow(m_b, x / m_c);
    }
}