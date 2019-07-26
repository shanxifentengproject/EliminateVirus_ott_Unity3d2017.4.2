using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UniGui/Extend/GuiSelectFlag")]
class GuiSelectFlag : GuiPlaneAnimationPlayer
{
    public GuiPlaneAnimationCurveRelativePosition lefttop = null;
    public GuiPlaneAnimationCurveRelativePosition leftbottom = null;
    public GuiPlaneAnimationCurveRelativePosition righttop = null;
    public GuiPlaneAnimationCurveRelativePosition rigthbottom = null;
    public Vector3 AnchorPositionOffset = Vector3.zero;
    //移动到这个锚点上
    public void MoveToAnchor(GuiAnchorObject anchor)
    {
        Vector3 anchorPosition = anchor.transform.position;
        //把对象坐标移动到这个坐标上去
        transform.position = anchorPosition + AnchorPositionOffset;
        //需要分别移动4个点的坐标过去
        Vector3 offset = new Vector3(-anchor.buttonSize.x / 2.0f, anchor.buttonSize.y / 2.0f, 0.0f);
        if (lefttop != null)
        {
            lefttop.originalPosition = offset;
            lefttop.gameObject.transform.localPosition = lefttop.originalPosition;
        }
        

        offset = new Vector3(-anchor.buttonSize.x / 2.0f, -anchor.buttonSize.y / 2.0f, 0.0f);
        if (leftbottom != null)
        {
            leftbottom.originalPosition = offset;
            leftbottom.gameObject.transform.localPosition = leftbottom.originalPosition;
        }
        

        offset = new Vector3(anchor.buttonSize.x / 2.0f, anchor.buttonSize.y / 2.0f, 0.0f);
        if (righttop != null)
        {
            righttop.originalPosition = offset;
            righttop.gameObject.transform.localPosition = righttop.originalPosition;
        }
        

        offset = new Vector3(anchor.buttonSize.x / 2.0f, -anchor.buttonSize.y / 2.0f, 0.0f);
        if (rigthbottom != null)
        {
            rigthbottom.originalPosition = offset;
            rigthbottom.gameObject.transform.localPosition = rigthbottom.originalPosition;
        } 
    }
}
