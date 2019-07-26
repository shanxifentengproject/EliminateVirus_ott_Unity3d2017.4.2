using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/ComponentControl/GuiPlaneAnimationGameObjectControl")]
class GuiPlaneAnimationGameObjectControl : GuiPlaneAnimationElement
{
    [System.Serializable]
    public class GameObjectData
    {
        public Transform transform = null;
        public float startTime = 0.0f;
        public float endTime = 0.0f;
    }
    public GameObjectData[] list = null;
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        if (list == null || list.Length == 0)
            return;
        for (int i = 0; i < list.Length; i++)
        {
            GameObjectData data = list[i];
            if (time >= data.startTime && time <= data.endTime)
            {
                if (!IsGameObjectActive(data.transform))
                {
                    SetGameObjectActive(data.transform, true);
                }
            }
            else if (IsGameObjectActive(data.transform))
            {
                SetGameObjectActive(data.transform, false);
            }
        }
    }
    private bool IsGameObjectActive(Transform transform)
    {
        return transform.gameObject.activeSelf;
    }
    private void SetGameObjectActive(Transform transform, bool active)
    {
        transform.gameObject.SetActive(active);
    }
}
