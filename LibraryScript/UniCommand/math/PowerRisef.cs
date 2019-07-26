using System;
/*
 * 方程为:a-(a*power(b,x/c))=y
 * 
 * */
struct PowerRisef
{
    public float A;
    public float B;
    public float C;
    public PowerRisef(float a, float b, float c)
    {
        A = a;
        B = b;
        C = c;
    }
    public float GetValue(float x)
    {
        return A - (A * UnityEngine.Mathf.Pow(B, x / C));
    }
}


[System.Serializable]
class SerPowerRisef
{
    public float A;
    public float B;
    public float C;
    public SerPowerRisef(float a, float b, float c)
    {
        A = a;
        B = b;
        C = c;
    }
    public float GetValue(float x)
    {
        return A - (A * UnityEngine.Mathf.Pow(B, x / C));
    }
}