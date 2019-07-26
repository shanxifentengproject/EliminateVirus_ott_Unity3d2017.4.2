using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniCamera/UniUIOrthographicCamera3D")]
class UniUIOrthographicCamera3D : MonoBehaviourIgnoreGui
{
    protected Transform myTransform;
    public Camera myCameraLeft;
    public Camera myCameraRight;
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        myTransform = transform;
        myCameraLeft.enabled = false;
        myCameraRight.enabled = false;
        enabled = false;
    }
    public GameObject LoadResource_UIPrefabs(string name, UniGameResources gameResources)
    {
        if (!enabled)
        {
            myCameraLeft.enabled = true;
            myCameraRight.enabled = true;
            enabled = true;
        }
        GameObject obj = gameResources.LoadResource_Prefabs(name);
        obj.transform.parent = myTransform;
        return obj;
    }
    public GameObject LoadLanguageResource_UIPrefabs(string name, UniGameResources gameResources)
    {
        if (!enabled)
        {
            myCameraLeft.enabled = true;
            myCameraRight.enabled = true;
            enabled = true;
        }
        GameObject obj = gameResources.LoadLanguageResource_Prefabs(name);
        obj.transform.parent = myTransform;
        return obj;
    }
    public void PlayFadeScreen(UniGameResources gameResources)
    {
        LoadResource_UIPrefabs("FadeScreen.prefab", gameResources);
    }
    private void Update()
    {
        if (enabled)
        {
            if (myTransform.childCount == 0)
            {
                myCameraLeft.enabled = false;
                myCameraRight.enabled = false;
                enabled = false;
            }
        }
    }
}