using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/Extend/GuiUiSceneBase")]
class GuiUiSceneBase : MonoBehaviourInputSupport
{
    public virtual int uiSceneId { get { return 0; } }
    protected override void Start()
    {
        base.Start();
        OnInitializationUI();
    }
    //获取UI摄像机，UI上的控件需要重写此函数
    public virtual GuiUiSceneBase UICamreaPtr
    {
        get
        {
            if (transform.parent == null ||
                    transform.parent.GetComponent<GuiUiSceneBase>() == null)
            {
                return this;
            }
            return transform.parent.GetComponent<GuiUiSceneBase>().UICamreaPtr;
        }
    }
    //获取UI摄像机的正交相机控制组件
    public UniUIOrthographicCamera UICamera
    {
        get
        {
            GuiUiSceneBase ptr = UICamreaPtr;
            if (ptr == null)
                return null;
            return ptr.GetComponent<UniUIOrthographicCamera>();
        }
    }
    public GuiUiSceneManager UIManager
    {
        get
        {
            return (GuiUiSceneManager)UICamreaPtr;
        }
    }
    
    //构造一个选择对象
    public virtual GuiSelectFlag AllocSelectFlag()
    {
        GuiUiSceneBase ptr = UICamreaPtr;
        if (ptr == null)
            return null;
        return ptr.AllocSelectFlag();
    }
    //设置隐藏场景摄像机
    public virtual bool hideSceneCamera
    {
        set
        {
            GuiUiSceneBase ptr = UICamreaPtr;
            if (ptr == null)
                return;
            ptr.hideSceneCamera = value;
        }
    }

    //载入预置成为我的子对象
    public GameObject LoadResource_UIPrefabs(string name)
    {
        GameObject obj = UICamera.LoadResource_UIPrefabs(name, UniGameResources.currentUniGameResources);
        obj.transform.parent = transform;
        return obj;
    }
    public GameObject LoadLanguageResource_UIPrefabs(string name)
    {
        GameObject obj = UICamera.LoadLanguageResource_UIPrefabs(name, UniGameResources.currentUniGameResources);
        obj.transform.parent = transform;
        return obj;
    }
    protected virtual void OnInitializationUI()
    {

    }
    //设置传递的参数
    public virtual void SetTransferParameter(params object[] args)
    {

    }
    //一个子UI删除
    public virtual void OnChildUIRelease(GuiUiSceneBase ui)
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (transform.parent != null)
        {
            GuiUiSceneBase parent = transform.parent.GetComponent<GuiUiSceneBase>();
            if (parent != null)
            {
                parent.OnChildUIRelease(this);
            }
        }
       
    }
}
