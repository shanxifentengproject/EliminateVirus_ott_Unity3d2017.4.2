using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationCurveRelativePosition")]

class GuiPlaneAnimationCurveRelativePosition : GuiPlaneAnimationElement
{
    //原始坐标
    public Vector3 originalPosition;
    //原始坐标是否使用自身坐标
    public bool isSelfPosition = false;
    public AnimationCurve xCurve = new AnimationCurve();
    public AnimationCurve yCurve = new AnimationCurve();
    public AnimationCurve zCurve = new AnimationCurve();
    protected void Start()
    {
        if (isSelfPosition)
        {
            originalPosition = transform.localPosition;
        }
    }
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        Vector3 localPosition = originalPosition;
        if (xCurve.length != 0)
        {
            localPosition.x += xCurve.Evaluate(time);
        }
        if (yCurve.length != 0)
        {
            localPosition.y += yCurve.Evaluate(time);
        }
        if (zCurve.length != 0)
        {
            localPosition.z += zCurve.Evaluate(time);
        }
        myTransform.localPosition = localPosition;
    }
}
