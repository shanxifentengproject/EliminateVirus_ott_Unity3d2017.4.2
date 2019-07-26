using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationCurveRotation")]
class GuiPlaneAnimationCurveRotation : GuiPlaneAnimationElement
{
    public AnimationCurve xAngleCurve = new AnimationCurve();
    public AnimationCurve yAngleCurve = new AnimationCurve();
    public AnimationCurve zAngleCurve = new AnimationCurve();
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        Vector3 localAngle = myTransform.localRotation.eulerAngles;
        if (xAngleCurve.length != 0)
        {
            localAngle.x = xAngleCurve.Evaluate(time);
        }
        if (yAngleCurve.length != 0)
        {
            localAngle.y = yAngleCurve.Evaluate(time);
        }
        if (zAngleCurve.length != 0)
        {
            localAngle.z = zAngleCurve.Evaluate(time);
        }
        myTransform.localRotation = Quaternion.Euler(localAngle);
    }
}
