using System;
using System.Collections.Generic;
using UnityEngine;

//这是坦克世界内处理投影器性能的一个例子。
//如果是单独摄像机一般都在OnPreCull处理
//如果是多摄像机则需要像本代码的处理办法

//[AddComponentMenu("UniPerformance/UniCameraPerformance/ProjectionCameraPerformance")]
//class ProjectionCameraPerformance : UniCameraPerformance
//{
//    [System.Serializable]
//    public class PerformaceData
//    {
//        public UniCameraPerformanceCookieType type;
//        //检测区域
//        public float checkRange;
//        //摄像机前检测区域
//        public float forntRange;
//        //摄像机后检测区域
//        public float backRange;

//        public float checktime;
//    }

//    public PerformaceData[] performacedata = null;

//    public Camera[] checkCameraMode1P;
//    public Transform[] checkCameraTransformMode1P;

//    public Camera[] checkCameraMode2P;
//    public Transform[] checkCameraTransformMode2P;
//    protected override void Awake()
//    {
//        base.Awake();
//        if (performacedata != null)
//        {
//            for (int i = 0;i< performacedata.Length;i++)
//            {
//                UniCameraPerformanceKindControl.PerformanceCheckData data = new UniCameraPerformanceKindControl.PerformanceCheckData(
//                        performacedata[i].type,
//                        PerformanceControl.HandlePerformanceCommonProc,
//                        performacedata[i].checkRange,
//                        performacedata[i].forntRange,
//                        performacedata[i].backRange,
//                        performacedata[i].checktime);
//                PerformanceControl.AddPerformanceCheckData(ref data);
//            }
//        } 
//    }
//    protected override void OnPreCull()
//    {
//        //不用这里检测
//        //if (isEffect)
//        //{
//        //    base.OnPreCull();
//        //}
//    }
//    private void LateUpdate()
//    {
//        if (InputDevice.Instance.CurrentInputMode == InputDeviceBase.InputPlayerMode.Mode_1P)
//        {
//            PerformanceControl.HandlePerformance(checkCameraMode1P, checkCameraTransformMode1P);
//        }
//        else if (InputDevice.Instance.CurrentInputMode == InputDeviceBase.InputPlayerMode.Mode_2P)
//        {
//            PerformanceControl.HandlePerformance(checkCameraMode2P, checkCameraTransformMode2P);
//        }
//    }
//}
