using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationCurveScale")]
class GuiPlaneAnimationCurveScale : GuiPlaneAnimationElement
{
    public AnimationCurve xCurve = new AnimationCurve();
    public AnimationCurve yCurve = new AnimationCurve();
    public AnimationCurve zCurve = new AnimationCurve();
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        Vector3 localScale = myTransform.localScale;
        if (xCurve.length != 0)
        {
            localScale.x = xCurve.Evaluate(time);
        }
        if (yCurve.length != 0)
        {
            localScale.y = yCurve.Evaluate(time);
        }
        if (zCurve.length != 0)
        {
            localScale.z = zCurve.Evaluate(time);
        }
        myTransform.localScale = localScale;
    }
}
