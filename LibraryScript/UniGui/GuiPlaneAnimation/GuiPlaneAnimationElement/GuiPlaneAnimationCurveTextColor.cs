﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationCurveTextColor")]
class GuiPlaneAnimationCurveTextColor : GuiPlaneAnimationElement
{
    public AnimationCurve maincolorRGBCurve = new AnimationCurve();
    public AnimationCurve maincolorACurve = new AnimationCurve();
    public GuiPlaneAnimationText animationText = null;
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        Color c = animationText.TextColor;
        if (maincolorRGBCurve.length != 0)
        {
            float v = maincolorRGBCurve.Evaluate(time);
            c.r = v; c.g = v; c.b = v;
        }
        if (maincolorACurve.length != 0)
        {
            c.a = maincolorACurve.Evaluate(time);
        }
        animationText.TextColor=c;
        if (myRender == null)
        {
            myRender = myRenderer;
        }
    }

    protected MeshRenderer myRender = null;
/// <summary>
/// 独立场景时使用，及不切换场景的
/// </summary>
#if Independent_Scene
 
#else
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (myRender != null)
        {
        	if(myRender.material != null)
        	{
        		UnityEngine.GameObject.Destroy(myRender.material);
        	}
            myRender.material = null;
            UnityEngine.GameObject.Destroy(myRender);
            myRender = null;
        }
    }
#endif
}
