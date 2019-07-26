using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniCamera/UniUIOrthographicCamera")]
class UniUIOrthographicCamera : MonoBehaviourIgnoreGui
{
    protected Transform myTransform;
    protected Camera myCamera;
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        myTransform = transform;
        myCamera = GetComponent<Camera>();
        myCamera.enabled = false;
        enabled = false;
    }
    public GameObject LoadResource_UIPrefabs(string name,UniGameResources gameResources)
    {
        if (!myCamera.enabled)
        {
            myCamera.enabled = true;
            enabled = true;
        }
        GameObject obj = gameResources.LoadResource_Prefabs(name);
        obj.transform.parent = myTransform;
        return obj;
    }
    public GameObject LoadLanguageResource_UIPrefabs(string name, UniGameResources gameResources)
    {
        if (!myCamera.enabled)
        {
            myCamera.enabled = true;
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
        if (myCamera.enabled)
        {
            if (myTransform.childCount == 0)
            {
                myCamera.enabled = false;
                enabled = false;
            }
        }
    }
}
