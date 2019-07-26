using System;
using UnityEngine;
struct SamplingColor
{
    //颜色采样点
    public Color[] colorSamplingPoint;
    private const float from = 0.0f;
    private const float to = 1.0f;
    public SamplingColor(params Color[] colorlist)
    {
        colorSamplingPoint = colorlist;
    }
    /// <summary>
    /// 根据一个值来获得采样值
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Color GetValue(float v)
    {
        if (colorSamplingPoint.Length == 0)
            return Color.red;
        else if (v <= from || colorSamplingPoint.Length == 1)
            return colorSamplingPoint[0];
        else if (v >= to)
            return colorSamplingPoint[colorSamplingPoint.Length - 1];
        float idx = (colorSamplingPoint.Length - 1) * (v - from) / (to - from);
        return Color.Lerp(colorSamplingPoint[(int)idx], colorSamplingPoint[(int)idx + 1], idx - (int)idx);
    }
}






//处理颜色采样的类
//使用Color的插值函数
[System.Serializable]
class SerSamplingColor
{
    //颜色采样点
    public Color[] colorSamplingPoint = null;
    private const float from = 0.0f;
    private const float to = 1.0f;
    public SerSamplingColor(params Color[] colorlist)
    {
        colorSamplingPoint = colorlist;
    }
    /// <summary>
    /// 根据一个值来获得采样值
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Color GetValue(float v)
    {
        if (colorSamplingPoint.Length == 0)
            return Color.red;
        else if (v <= from || colorSamplingPoint.Length == 1)
            return colorSamplingPoint[0];
        else if (v >= to)
            return colorSamplingPoint[colorSamplingPoint.Length - 1];
        float idx = (colorSamplingPoint.Length - 1) * (v - from) / (to - from);
        return Color.Lerp(colorSamplingPoint[(int)idx], colorSamplingPoint[(int)idx + 1], idx - (int)idx);
    }
}