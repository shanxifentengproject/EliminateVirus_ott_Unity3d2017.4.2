using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/ComponentControl/GuiPlaneAnimationParticleSystemControl")]
class GuiPlaneAnimationParticleSystemControl : GuiPlaneAnimationElement
{
    [System.Serializable]
    public class ParticleSystemData
    {
        public ParticleSystem particle = null;
        public float startTime = 0.0f;
        public float endTime = 0.0f;
    }
    public ParticleSystemData[] list = null;
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        if (list == null || list.Length == 0)
            return;
        for (int i = 0; i < list.Length; i++)
        {
            ParticleSystemData data = list[i];
            if (time >= data.startTime && time <= data.endTime)
            {
                if (!IsParticleSystemActive(data.particle))
                {
                    SetParticleSystemActive(data.particle,true);
                }
            }
            else if (IsParticleSystemActive(data.particle))
            {
                SetParticleSystemActive(data.particle,false);
            }
        }
    }
    private bool IsParticleSystemActive(ParticleSystem p)
    {
        //return p.enableEmission;
        return false;
    }
    private void SetParticleSystemActive(ParticleSystem p,bool active)
    {
        //p.enableEmission = active;
    }
}
