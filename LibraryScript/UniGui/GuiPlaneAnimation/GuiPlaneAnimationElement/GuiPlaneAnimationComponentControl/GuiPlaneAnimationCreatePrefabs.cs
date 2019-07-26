using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/ComponentControl/GuiPlaneAnimationCreatePrefabs")]
class GuiPlaneAnimationCreatePrefabs : GuiPlaneAnimationElement
{
    [System.Serializable]
    public class PrefabsData
    {
        public GameObject prefabs = null;
        public float time = 0.0f;
        public Vector3 offset = Vector3.zero;
    }
    public Transform rootTransform = null;
    public PrefabsData[] list = null;
    private float prveTime = 0.0f;
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        if (list == null || list.Length == 0)
            return;
        for (int i = 0; i < list.Length; i++)
        {
            PrefabsData data = list[i];
            if (data.time > prveTime && data.time <= time)
            {
                InstantiatePrefabs(data.prefabs, data.offset);
            }
        }
        prveTime = time;
    }
    private void InstantiatePrefabs(GameObject prefabs,Vector3 offset)
    {
        GameObject obj = (GameObject)Instantiate((GameObject)prefabs);
        obj.transform.parent = rootTransform.parent;
        obj.transform.position = rootTransform.position + offset;
        obj.transform.rotation = rootTransform.rotation;
    }
}
