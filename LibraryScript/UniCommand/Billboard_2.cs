using System;
using UnityEngine;
[AddComponentMenu("UniCommand/Billboard_2")]
class Billboard_2 : MonoBehaviourIgnoreGui
{
    protected virtual void OnWillRenderObject()
    {
        if (!enabled)
            return;
        Camera cam = Camera.current;
        if (!cam || !cam.enabled)
            return;
        Vector3 relativePos = transform.position - cam.transform.position;

        transform.rotation = Quaternion.LookRotation(relativePos);

    }
}