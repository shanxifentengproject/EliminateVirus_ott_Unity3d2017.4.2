using System;
using System.Collections.Generic;
using UnityEngine;


class AnimationSetting : MonoBehaviourIgnoreGui
{
    public string[] animationList = null;
    public float[] animationPlaySpeed = null;
    public float[] animationPlayStartTime = null;
    protected virtual void Start()
    {
        Animation ani = this.GetComponent<Animation>();
        for (int i = 0; i < animationList.Length; i++)
        {
            ani[animationList[i]].speed = animationPlaySpeed[i];
            ani[animationList[i]].time = animationPlayStartTime[i];
        }
    }
}
