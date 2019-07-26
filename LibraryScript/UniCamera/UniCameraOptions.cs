using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("UniCamera/UniCameraOptions")]
class UniCameraOptions : MonoBehaviourIgnoreGui
{
    private const int LAYERMASKCOUNT = 32;
    //没有明确设置的默认层的距离
    public float DefineLayerDistance = 100.0f;
    [System.Serializable]
    public class LayerDistance
    {
        public LayerMask layerMask = 0;
        public float Distance = 0.0f;
    }
    //没有设置的层使用默认距离
    //如果设置为零的表示使用摄像机的远裁剪平面
    public LayerDistance[] layerDistance = null;

    //是否球形裁剪
    public bool layerCullSpherical = false;
    //透明排序模式
    public TransparencySortMode transparencySortMode = TransparencySortMode.Default;
    //是否使用遮挡剔除
    //如果是正交相机需要关闭遮罩剔除
    public bool useOcclusionCulling = true;

    //事件控制层,使用这个层用来控制摄像机渲染到的物体响应鼠标消息。
    //一般如果不响应鼠标这个层都设置为零。
    public LayerMask eventLayerMask = 0;
    private void SetLayerDistance(Camera myCamera)
    {
        //首先处理摄像机的层设定
        try
        {
            float[] setLayerDistance = new float[LAYERMASKCOUNT];
            for (int i = 0; i < LAYERMASKCOUNT; i++)
            {
                setLayerDistance[i] = DefineLayerDistance;
            }
            if (layerDistance != null)
            {
                for (int i = 0; i < layerDistance.Length; i++)
                {
                    int value = LayerMaskChangeIndex(layerDistance[i].layerMask);
                    if (value <= 0 || value >= LAYERMASKCOUNT)
                        throw new Exception("Layer Distance Set Err!");
                    setLayerDistance[value] = layerDistance[i].Distance;
                }
            }
            myCamera.layerCullDistances = setLayerDistance;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
        }
        
    }

    public static int LayerMaskChangeIndex(LayerMask layerMask)
    {
        int nlayerMask = layerMask.value;
        int nIndex = -1;
        if (nlayerMask < 0)
        {
            return -1;
        }
        while (nlayerMask > 0)
        {
            nlayerMask = nlayerMask >> 1;
            nIndex++;
        }
        return nIndex;
    }

    //是否重设相机长宽比
    public bool IsSetAspect = false;
    //设置的长宽比,默认 1280、720;
    public float Aspect = 1.78f;
    protected override void Awake()
    {
        base.Awake();
        Camera myCamera = GetComponent<Camera>();
        SetLayerDistance(myCamera);
        myCamera.layerCullSpherical = layerCullSpherical;
        myCamera.transparencySortMode = transparencySortMode;
        myCamera.useOcclusionCulling = useOcclusionCulling;
        //4.3才支持
        myCamera.eventMask = eventLayerMask.value;
        if (IsSetAspect)
        {
            myCamera.aspect = Aspect;
            myCamera.ResetAspect();
        }
        
    }
}
