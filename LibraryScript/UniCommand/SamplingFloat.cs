using System;
using UnityEngine;
//处理采样的类
[System.Serializable]
struct SamplingFloat
{
    //强度采样点，默认为全局的采样点
    public float[] FloatSamplingPoint;
    private const float from = 0.0f;
    private const float to = 1.0f;
    public SamplingFloat(params float[] pointlist)
    {
        FloatSamplingPoint = pointlist;
    }
    /// <summary>
    /// 根据一个值来获得采样值
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public float GetValue(float v)
    {
        if (FloatSamplingPoint.Length == 0)
            return 0.0f;
        else if (v <= from || FloatSamplingPoint.Length == 1)
            return FloatSamplingPoint[0];
        else if (v >= to)
            return FloatSamplingPoint[FloatSamplingPoint.Length - 1];
        float idx = (FloatSamplingPoint.Length - 1) * (v - from) / (to - from);
        return Mathf.Lerp(FloatSamplingPoint[(int)idx], FloatSamplingPoint[(int)idx + 1], idx - (int)idx);
    }
}
//处理采样的类
[System.Serializable]
class SerSamplingFloat
{
    //强度采样点，默认为全局的采样点
    public float[] FloatSamplingPoint;
    private const float from = 0.0f;
    private const float to = 1.0f;
    public SerSamplingFloat(params float[] pointlist)
    {
        FloatSamplingPoint = pointlist;
    }
    /// <summary>
    /// 根据一个值来获得采样值
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public float GetValue(float v)
    {
        if (FloatSamplingPoint.Length == 0)
            return 0.0f;
        else if (v <= from || FloatSamplingPoint.Length == 1)
            return FloatSamplingPoint[0];
        else if (v >= to)
            return FloatSamplingPoint[FloatSamplingPoint.Length - 1];
        float idx = (FloatSamplingPoint.Length - 1) * (v - from) / (to - from);
        return Mathf.Lerp(FloatSamplingPoint[(int)idx], FloatSamplingPoint[(int)idx + 1], idx - (int)idx);
    }
}