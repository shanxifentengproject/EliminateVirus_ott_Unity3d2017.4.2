using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
abstract class UniCamera3DAdjust : MonoBehaviourIgnoreGui
{
    public Camera myCameraLeft;
    public Camera myCameraRight;
    //中间量
    public float centerValue = 0.0f;
    //增加量
    public float increaseValue = 0.01f;
    //当前量
    public float currentValue = 0.5f;
    //初始量
    private float initvalue = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        initvalue = currentValue;
        UpdateCameraValue();
    }
    private void UpdateCameraValue()
    {
        Vector3 v = myCameraLeft.transform.localPosition;
        v.x = (centerValue - currentValue);
        myCameraLeft.transform.localPosition = v;

        v = myCameraRight.transform.localPosition;
        v.x = (centerValue + currentValue);
        myCameraRight.transform.localPosition = v;
    }
    protected abstract bool IsIncreaseValue{get;}
    protected abstract bool IsDecreaseValue{get;}
    protected abstract bool IsShowGUI{get;}
    protected abstract bool IsReset{get;}
    protected virtual void Update()
    {
        if (IsIncreaseValue)
        {
            currentValue += increaseValue;
            UpdateCameraValue();
        }
        else if (IsDecreaseValue)
        {
            currentValue -= increaseValue;
            UpdateCameraValue();
        }
        else if (IsReset)
        {
            currentValue = initvalue;
            UpdateCameraValue();
        }
    }
    protected virtual void OnGUI()
    {

        if (IsShowGUI)
        {
            GUI.Label(new Rect(10, 10, 100, 20), currentValue.ToString());
        }
        
    }
}
