using UnityEngine;
using System;

[System.Serializable]
class MaterialMainTextureOffset_Beat
{
    //目标材质球
    public Material targetMaterial = null;
    //节拍时间
    private float beatCountTime = 0.0f;
    public float beatTime = 1.0f;
    //每一个节拍的改变量
    public Vector2 beatStart = new Vector2(0.0f, 0.0f);
    public Vector2 beatEnd = new Vector2(0.1f, 0.1f);
    public Vector2 beatOffset = new Vector2(1.0f, 1.0f);
    public void Update()
    {
        beatCountTime += Time.deltaTime;
        if (beatCountTime >= beatTime)
        {
            beatCountTime = 0.0f;
            Vector2 v = targetMaterial.mainTextureOffset;
            v += beatOffset;
            if (beatOffset.x > 0.0f)
            {
                if (v.x > beatEnd.x)
                {
                    v.x = beatStart.x;
                }
            }
            else if (beatOffset.x < 0.0f)
            {
                if (v.x < beatEnd.x)
                {
                    v.x = beatStart.x;
                }
            }

            if (beatOffset.y > 0.0f)
            {
                if (v.y > beatEnd.y)
                {
                    v.y = beatEnd.y;
                }
            }
            else if (beatOffset.y < 0.0f)
            {
                if (v.y < beatEnd.y)
                {
                    v.y = beatEnd.y;
                }
            }
            targetMaterial.mainTextureOffset=v;
        }
    }
}
[AddComponentMenu("UniGui/Animation/TextureMaterialsAnimation")]
class TextureMaterialsAnimation : MonoBehaviourIgnoreGui
{
    //节拍型纹理偏移动画
    public MaterialMainTextureOffset_Beat[] beatMainTextureOffset = null;
    void Update()
    {
        if (beatMainTextureOffset != null)
        {
            for (int i = 0; i < beatMainTextureOffset.Length; i++ )
            {
                beatMainTextureOffset[i].Update();
            }
        }
    }
}
