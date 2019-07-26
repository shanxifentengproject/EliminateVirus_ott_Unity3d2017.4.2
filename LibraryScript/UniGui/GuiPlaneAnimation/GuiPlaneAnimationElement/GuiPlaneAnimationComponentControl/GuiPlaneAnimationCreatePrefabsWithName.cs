using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/ComponentControl/GuiPlaneAnimationCreatePrefabsWithName")]
class GuiPlaneAnimationCreatePrefabsWithName : GuiPlaneAnimationElement
{
    [System.Serializable]
    public class PrefabsData
    {
        public string prefabName = "";
        public float time = 0.0f;
        public Vector3 offset = Vector3.zero;
        public bool isLanguage = false;
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
                InstantiatePrefabs(data.prefabName, data.offset,data.isLanguage);
            }
        }
        prveTime = time;
    }
    private void InstantiatePrefabs(string prefabName, Vector3 offset,bool isLanguage)
    {
        GameObject obj;
        if (isLanguage)
        {
            obj = UniGameResources.currentUniGameResources.LoadLanguageResource_Prefabs(prefabName);
        }
        else
        {
            obj = UniGameResources.currentUniGameResources.LoadResource_Prefabs(prefabName);
        }
        obj.transform.parent = rootTransform.parent;
        obj.transform.position = rootTransform.position + offset;
        obj.transform.rotation = rootTransform.rotation;
    }
}
