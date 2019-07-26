using System;
struct Curvef4
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public float m_h2;
    public float m_h3;
    public Curvef4(float h3,float h2, float h1, float h0)
    {
        m_h3 = h3;
        m_h2 = h2;
        m_h1 = h1;
        m_h0 = h0;

    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x + m_h2 * x * x + m_h3 * x * x * x;
    }
}


[System.Serializable]
class SerCurvef4
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public float m_h2;
    public float m_h3;
    public SerCurvef4(float h3, float h2, float h1, float h0)
    {
        m_h3 = h3;
        m_h2 = h2;
        m_h1 = h1;
        m_h0 = h0;

    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x + m_h2 * x * x + m_h3 * x * x * x;
    }
}