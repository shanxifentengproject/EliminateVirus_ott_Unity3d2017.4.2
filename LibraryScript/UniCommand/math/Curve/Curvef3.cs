using System;
struct Curvef3
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public float m_h2;
    public Curvef3(float h2,float h1, float h0)
    {
        m_h2 = h2;
        m_h1 = h1;
        m_h0 = h0;
        
    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x + m_h2 * x * x;
    }
}


[System.Serializable]
class SerCurvef3
{
    //这几个系数可以取任何值
    public float m_h0;
    public float m_h1;
    public float m_h2;
    public SerCurvef3(float h2, float h1, float h0)
    {
        m_h2 = h2;
        m_h1 = h1;
        m_h0 = h0;

    }
    //x可以取任意值
    public float GetValue(float x)
    {
        return m_h0 + m_h1 * x + m_h2 * x * x;
    }
}