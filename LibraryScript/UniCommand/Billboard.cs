using System;
using UnityEngine;
[AddComponentMenu("UniCommand/Billboard")]
class Billboard : MonoBehaviourIgnoreGui
{
    
    private Quaternion direction;
    protected virtual void Start()
    {
        Vector3 Normal = Vector3.zero;
        direction = Quaternion.FromToRotation(new Vector3(0, 0, 1), Normal);
    }
    
    protected virtual void OnWillRenderObject()
    {
        if (!enabled)
            return;
        Camera cam = Camera.current;
        if (!cam || !cam.enabled)
            return;
        transform.rotation = cam.transform.rotation * direction;
    }
}