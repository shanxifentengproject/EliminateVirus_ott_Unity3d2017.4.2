using System;
struct Curvef2
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public Curvef2(float h1, float h0)
    {
        m_h1 = h1;
        m_h0 = h0;
    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x;
    }
}

[System.Serializable]
class SerCurvef2
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public SerCurvef2(float h1, float h0)
    {
        m_h1 = h1;
        m_h0 = h0;
    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x;
    }
}