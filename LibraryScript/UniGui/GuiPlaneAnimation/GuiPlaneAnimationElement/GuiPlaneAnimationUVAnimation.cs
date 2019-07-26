//#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3 && !UNITY_5_4)
#define UNITY_5_5_AND_GREATER
//#endif

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/Element/GuiPlaneAnimationUVAnimation")]
class GuiPlaneAnimationUVAnimation : GuiPlaneAnimationElement
{
    private MeshFilter meshfilter = null;
    //UV拆分
    public float UVAnimationXTile = 1.0f;
    private int UVAnimationXTileInt = 1;
    public float UVAnimationYTile = 1.0f;
    private float FrameUVWidth;
    private float FrameUVHeight;
    public float uvAnimationXTile
    {
        get { return UVAnimationXTile; }
        set 
        {
            if (value <= 0.0f || UVAnimationXTile == value)
                return;
            UVAnimationXTile = value;
            UVAnimationXTileInt = (int)UVAnimationXTile;
            FrameUVWidth = 1.0f / UVAnimationXTile;
        }
    }
    public float uvAnimationYTile
    {
        get { return UVAnimationYTile; }
        set
        {
            if (value <= 0.0f || UVAnimationYTile == value)
                return;
            UVAnimationYTile = value;
            FrameUVHeight = 1.0f / UVAnimationYTile;
        }
    }
    //UV缓冲
    private static Vector2[] uvBuffer = new Vector2[4];
    //最大帧数
    public float Frames = 1.0f;
    //当前帧数
    private float m_Frame = -1.0f;
    public float frame
    {
        get { return m_Frame; }
        set
        {
            if (m_Frame == value || value < 0.0f || value >= Frames)
                return;
            m_Frame = value;
            
             //帧数的排列方法是从左到右，从上到下
            //分别计算4个顶点的uv
            Vector2 LeftTopUv = AccountFrameUV((int)m_Frame);
            Vector2 LeftBottomUv = new Vector2(LeftTopUv.x, LeftTopUv.y + FrameUVHeight);
            Vector2 RightTopUv = new Vector2(LeftTopUv.x + FrameUVWidth, LeftTopUv.y);
            Vector2 RightBottomUv = new Vector2(LeftTopUv.x + FrameUVWidth, LeftTopUv.y + FrameUVHeight);
            //重新计算模型的UV分布
            //Debug.Log("0:" + uv[0].ToString());
            //Debug.Log("1:" + uv[1].ToString());
            //Debug.Log("2:" + uv[2].ToString());
            //Debug.Log("3:" + uv[3].ToString());
            
            //if (Application.unityVersion.StartsWith("4."))
            //{//unity==4.x版本
            //    uvBuffer[0] = CorrectionFrameUV(LeftBottomUv);
            //    uvBuffer[1] = CorrectionFrameUV(RightTopUv);
            //    uvBuffer[2] = CorrectionFrameUV(RightBottomUv);
            //    uvBuffer[3] = CorrectionFrameUV(LeftTopUv);
            //}
            //else
            //{//unity==5.x版本
            //    uvBuffer[0] = CorrectionFrameUV(LeftBottomUv);
            //    uvBuffer[1] = CorrectionFrameUV(LeftTopUv);
            //    uvBuffer[2] = CorrectionFrameUV(RightTopUv);
            //    uvBuffer[3] = CorrectionFrameUV(RightBottomUv);
            //}
#if UNITY_5_5_AND_GREATER
            uvBuffer[0] = LeftTopUv;
            uvBuffer[1] = LeftBottomUv;
            uvBuffer[2] = RightBottomUv;
            uvBuffer[3] = RightTopUv;
#else
            uvBuffer[0] = CorrectionFrameUV(LeftBottomUv);
            uvBuffer[1] = CorrectionFrameUV(RightTopUv);
            uvBuffer[2] = CorrectionFrameUV(RightBottomUv);
            uvBuffer[3] = CorrectionFrameUV(LeftTopUv);
#endif


            //
            if (meshfilter == null)
                meshfilter = (MeshFilter)GetComponent(typeof(MeshFilter));
            meshfilter.mesh.uv = uvBuffer;
        }
    }

/// <summary>
/// 独立场景时使用，及不切换场景的
/// </summary>
#if Independent_Scene
 
#else
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (meshfilter != null)
        {
            if (meshfilter.mesh != null)
            {
                UnityEngine.GameObject.Destroy(meshfilter.mesh);
            }
            meshfilter.mesh = null;
            UnityEngine.GameObject.Destroy(meshfilter);
            meshfilter = null;
        }
    }
#endif

    //重新计算一次帧拆分值
    public void AccountFrameUVRect()
    {
        FrameUVWidth = 1.0f / UVAnimationXTile;
        UVAnimationXTileInt = (int)UVAnimationXTile;
        FrameUVHeight = 1.0f / UVAnimationYTile;
    }
    //根据帧数计算这一帧的左上角顶点uv值
    //这里计算出来实际是反的
    private Vector2 AccountFrameUV(int frame)
    {
        //return new Vector2((float)(frame % XTile) * (1.0f / (float)XTile),
        //                (float)(frame / XTile) * (1.0f / (float)YTile));
        return new Vector2((float)(frame % UVAnimationXTileInt) * FrameUVWidth,
                        (float)(frame / UVAnimationXTileInt) * FrameUVHeight);
    }
    //修正UV
    private Vector2 CorrectionFrameUV(Vector2 uv)
    {
        return new Vector2(uv.x, 1.0f - uv.y);
    }
    protected override void Awake()
    {
        base.Awake();
        AccountFrameUVRect();
        meshfilter = (MeshFilter)GetComponent(typeof(MeshFilter));
        frame = 0.0f;
    }
    public override void TransformAnimation(float time, MeshRenderer myRenderer, Transform myTransform)
    {
        frame = Mathf.Lerp(0.0f, Frames, time);
    }
}
