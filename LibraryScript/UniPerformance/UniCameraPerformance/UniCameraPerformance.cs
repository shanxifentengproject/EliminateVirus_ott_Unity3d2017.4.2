using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniPerformance/UniCameraPerformance/UniCameraPerformance")]
class UniCameraPerformance : MonoBehaviourIgnoreGui
{
    protected Camera[] myCamera;
    protected Transform[] myCameraTransform;
    protected UniCameraPerformanceKindControl PerformanceControl = new UniCameraPerformanceKindControl();
    protected override void Awake()
    {
        myCamera = new Camera[1];
        myCameraTransform = new Transform[1];
        myCamera[0] = GetComponent<Camera>();
        myCameraTransform[0] = transform;
    }
    protected virtual void OnPreCull()
    {
        PerformanceControl.HandlePerformance(myCamera, myCameraTransform);
    }
}
