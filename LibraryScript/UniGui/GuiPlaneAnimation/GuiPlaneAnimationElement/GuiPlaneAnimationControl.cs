using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationControl")]
class GuiPlaneAnimationControl : MonoBehaviourIgnoreGui
{
    //当前对象的网格渲染器
    protected MeshRenderer myRenderer = null;
    protected Transform myTransform = null;
    protected GuiPlaneAnimationElement[] elementList = null;
    [HideInInspector]
    public bool overturn = false;
    protected override void Awake()
    {
        base.Awake();
        myRenderer = GetComponent<Renderer>() as MeshRenderer;
        myTransform = transform;
        //分析当前对象的所有组件
        Component[] list = GetComponents<Component>();
        //统计数量
        int count = 0;
        for(int i=0;i<list.Length;i++)
        {
            if (list[i] is GuiPlaneAnimationElement && list[i] != this)
                count += 1;
        }
        elementList = new GuiPlaneAnimationElement[count];
        int index = 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] is GuiPlaneAnimationElement)
            {
                elementList[index] = list[i] as GuiPlaneAnimationElement;
                index += 1;
            }
        }
    }
    public virtual void TransformAnimation(float time)
    {
        if (elementList != null)
        {
            if (overturn)
            {
                time = 1.0f - time;
            }
            for (int i = 0; i < elementList.Length;i++ )
            {
                elementList[i].TransformAnimation(time, myRenderer, myTransform);
            }
        }
    }
}
