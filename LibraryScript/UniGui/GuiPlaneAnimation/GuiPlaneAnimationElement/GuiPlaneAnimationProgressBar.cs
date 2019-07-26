using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationProgressBar")]
class GuiPlaneAnimationProgressBar : GuiPlaneAnimationElement
{
    //是否正向UV
    public bool IsForwardUV = true;
    //是否支持动画
    public bool IsSupportAnimation = true;
    //当前值
    protected float currentValue;
    //目标值
    protected float targetValue;
    public void SetProgressBar(float targetvalue)
    {
        SetProgressBar(targetvalue, false);
    }
    public void SetProgressBar(float targetvalue, bool isDirect)
    {
        //需要规划到血条区间内
        targetvalue = Mathf.Clamp(targetvalue * 0.5f, 0.0f, 0.5f);
        //在新的血条体系下。0.0f表示满血，0.5表示空血
        if (!IsForwardUV)
        {
            targetvalue = 0.5f - targetvalue;
        }

        if (isDirect || !IsSupportAnimation)
        {
            currentValue = targetvalue;
            //刷新血条
            GetComponent<Renderer>().material.SetVector("_WhiteMaskOffset", new Vector4(targetvalue, 0.0f, 0.0f, 0.0f));
            return;
        }
        Vector4 v = GetComponent<Renderer>().material.GetVector("_WhiteMaskOffset");
        currentValue = v.x;
        targetValue = targetvalue;
        GuiPlaneAnimationPlayer player = this.GetComponent<GuiPlaneAnimationPlayer>();
        player.Stop();
        player.Play();
    }
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        if (IsSupportAnimation)
        {
            float v = Mathf.Lerp(currentValue, targetValue, time);
            myRenderer.material.SetVector("_WhiteMaskOffset", new Vector4(v, 0.0f, 0.0f, 0.0f));
        }
        
    }
}
