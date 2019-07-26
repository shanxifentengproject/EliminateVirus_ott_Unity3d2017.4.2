using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
abstract class GuiPlaneAnimationElement : MonoBehaviourIgnoreGui
{
    public virtual void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {

    }

    public virtual void OnDestroy()
    {

    }
}
