using System;
using System.Collections.Generic;
using UnityEngine;
/*
 * 当前版本支持数字滚动动画，需要挂载GuiPlaneAnimationPlayer用来控制动画
 * */
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationTextRoll")]
class GuiPlaneAnimationTextRoll : GuiPlaneAnimationText
{
    public enum RollNumberType
    {
        Type_Nothing,
        Type_Integer,
        Type_Float,
    }
    private enum NumberIndex
    {
        Index_CurrentNumber = 0,
        Index_TargetNumber = 1,
    }

    private RollNumberType currentRollNumberType = RollNumberType.Type_Nothing;

    private int[] currentIntegerValue = new int[2];
    private float[] currentFloatValue = new float[2];

    protected virtual void Start()
    {
        GuiPlaneAnimationPlayer player = this.GetComponent<GuiPlaneAnimationPlayer>();
        player.DelegateOnPlayEndEvent += OnPlayEventEnd;
    }

    public void OnPlayEventEnd()
    {
        if (currentRollNumberType == RollNumberType.Type_Integer)
        {
            Text = currentIntegerValue[(int)NumberIndex.Index_TargetNumber].ToString();
        }
        else if (currentRollNumberType == RollNumberType.Type_Float)
        {
            Text = string.Format("{0:0.00}", currentFloatValue[(int)NumberIndex.Index_TargetNumber]);
        }
        currentRollNumberType = RollNumberType.Type_Nothing;
    }

    public void SetIntegerRollValue(int targetValue)
    {
        SetIntegerRollValue(targetValue, false);
    }
    public void SetIntegerRollValue(int targetValue,bool isDirect)
    {
        if (isDirect)
        {
            Text = targetValue.ToString();
            return;
        }
        currentRollNumberType = RollNumberType.Type_Integer;
        currentIntegerValue[(int)NumberIndex.Index_CurrentNumber] = Convert.ToInt32(Text);
        currentIntegerValue[(int)NumberIndex.Index_TargetNumber] = targetValue;
        GuiPlaneAnimationPlayer player = this.GetComponent<GuiPlaneAnimationPlayer>();
        player.Stop();
        player.Play();
    }

    public void SetFloatRollValue(float targetValue)
    {
        SetFloatRollValue(targetValue,false);
    }
    public void SetFloatRollValue(float targetValue,bool isDirect)
    {
        if (isDirect)
        {
            Text = string.Format("{0:0.00}", targetValue);
            return;
        }
        currentRollNumberType = RollNumberType.Type_Float;
        currentFloatValue[(int)NumberIndex.Index_CurrentNumber] = Convert.ToSingle(Text);
        currentFloatValue[(int)NumberIndex.Index_TargetNumber] = targetValue;
        GuiPlaneAnimationPlayer player = this.GetComponent<GuiPlaneAnimationPlayer>();
        player.Stop();
        player.Play();
    }
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        if (currentRollNumberType == RollNumberType.Type_Nothing)
        {
            return;
        }
        else if (currentRollNumberType == RollNumberType.Type_Integer)
        {
            int value = (int)Mathf.Lerp((float)currentIntegerValue[(int)NumberIndex.Index_CurrentNumber],
                                (float)currentIntegerValue[(int)NumberIndex.Index_TargetNumber],
                                time);
            Text = value.ToString();
        }
        else if (currentRollNumberType == RollNumberType.Type_Float)
        {
            float value = Mathf.Lerp((float)currentFloatValue[(int)NumberIndex.Index_CurrentNumber],
                                (float)currentFloatValue[(int)NumberIndex.Index_TargetNumber],
                                time);
            Text = string.Format("{0:0.00}", value);
        }
    }
}
