using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/*
 * 如果不需要在工程里设置字符
 * 不需要在工程里定义颜色，请直接使用GuiPlaneAnimationText
 * */
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationTextAdvanced")]
class GuiPlaneAnimationTextAdvanced : GuiPlaneAnimationText
{
    public string useText = "";
    public Color useColor = Color.white;
    protected override void Awake()
    {
        base.Awake();
        Text = useText;
        TextColor = useColor;
    }
}
