using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationCurvePosition")]
class GuiPlaneAnimationCurvePosition : GuiPlaneAnimationElement
{
    public AnimationCurve xCurve = new AnimationCurve();
    public AnimationCurve yCurve = new AnimationCurve();
    public AnimationCurve zCurve = new AnimationCurve();
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        Vector3 localPosition = myTransform.localPosition;
        if (xCurve.length != 0)
        {
            localPosition.x = xCurve.Evaluate(time);
        }
        if (yCurve.length != 0)
        {
            localPosition.y = yCurve.Evaluate(time);
        }
        if (zCurve.length != 0)
        {
            localPosition.z = zCurve.Evaluate(time);
        }
        myTransform.localPosition = localPosition;
    }
}
