using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/GuiPlaneAnimationSpiritControl")]
class GuiPlaneAnimationSpiritControl : GuiPlaneAnimationUVAnimation
{
    private MeshRenderer animationMeshRenderer = null;
    private GuiPlaneAnimationPlayer m_AnimationPlayer = null;
    public GuiPlaneAnimationPlayer animationPlayer { get { return m_AnimationPlayer; } }

    [System.Serializable]
    public class ActionInfo
    {
        public string actionName = "";
        public uint actionId { set; get; }
        public float UVAnimationXTile = 1.0f;
        public float UVAnimationYTile = 1.0f;
        public float Frames = 1.0f;
        public float PlayTime = 5.0f;
        //精灵的方向动画贴图
        //顺时针计算，从右开始
        public Texture[] spiritTexture = null;
    }
    //当前的动作定义
    public ActionInfo[] actionList = null;
    //当前正在使用的动作定义
    protected ActionInfo currentAction = null;
    //当前动作的名称
    public string currentActionName 
    { 
        get 
        { 
            return currentAction.actionName; 
        }
        set
        {
            currentActionId = FTLibrary.Command.FTUID.StringGetHashCode(value);
        }
    }
    //当前动作的编号
    public uint currentActionId
    {
        get { return currentAction.actionId; }
        set
        {
            if (currentAction.actionId == value)
                return;
            for (int i = 0; i < actionList.Length;i++ )
            {
                if (actionList[i].actionId == value)
                {
                    currentAction = actionList[i];
                    UpdateActionInfoToAnimationPlayer(currentAction, currentActionDirectionIndex);
                    return;
                }
            }
        }
    }
    //当前动作的方向编号
    private int m_CurrentActionDirectionIndex = 0;
    public int currentActionDirectionIndex
    {
        get { return m_CurrentActionDirectionIndex; }
        set
        {
            if (m_CurrentActionDirectionIndex == value)
                return;
            m_CurrentActionDirectionIndex = value;
            UpdateActionDirectionToAnimationPlayer(currentAction,m_CurrentActionDirectionIndex);
        }
    }
    public Vector3 currentActionDirection
    {
        get 
        {
            switch(currentActionDirectionIndex)
            {
                case 0:
                    return new Vector3(1.0f, 0.0f, 0.0f);
                case 1:
                    return new Vector3(1.0f, 0.0f, 1.0f);
                case 2:
                    return new Vector3(0.0f, 0.0f, 1.0f);
                case 3:
                    return new Vector3(-1.0f, 0.0f, 1.0f);
                case 4:
                    return new Vector3(-1.0f, 0.0f, 0.0f);
                case 5:
                    return new Vector3(-1.0f, 0.0f, -1.0f);
                case 6:
                    return new Vector3(0.0f, 0.0f, -1.0f);
                case 7:
                    return new Vector3(1.0f, 0.0f, -1.0f);
            }
            return Vector3.zero;
        }
        set
        {
            Vector3 direction = Vector3.Normalize(value);
            if (direction.x == 1.0f)
            {
                if (direction.z == 0.0f)
                    currentActionDirectionIndex = 0;
                else if (direction.z == 1.0f)
                    currentActionDirectionIndex = 1;
                else if (direction.z == -1.0f)
                    currentActionDirectionIndex = 7;
            }
            else if (direction.x == 0.0f)
            {
                if (direction.z == 1.0f)
                    currentActionDirectionIndex = 2;
                else if (direction.z == -1.0f)
                    currentActionDirectionIndex = 6;
            }
            else if (direction.x == -1.0f)
            {
                if (direction.z == 0.0f)
                    currentActionDirectionIndex = 4;
                else if (direction.z == 1.0f)
                    currentActionDirectionIndex = 3;
                else if (direction.z == -1.0f)
                    currentActionDirectionIndex = 5;
            }
        }
    }

    protected void UpdateActionDirectionToAnimationPlayer(ActionInfo info,int actionDirection)
    {
        animationMeshRenderer.material.mainTexture = info.spiritTexture[actionDirection];
        m_AnimationPlayer.TransformAnimation();
    }
    protected void UpdateActionInfoToAnimationPlayer(ActionInfo info,int actionDirection)
    {
        animationMeshRenderer.material.mainTexture = info.spiritTexture[actionDirection];
        uvAnimationXTile = info.UVAnimationXTile;
        uvAnimationYTile = info.UVAnimationYTile;
        Frames = info.Frames;
        m_AnimationPlayer.playTime = info.PlayTime;
        m_AnimationPlayer.playProgress = 0.0f;
        m_AnimationPlayer.TransformAnimation();
    }
    protected override void Awake()
    {
        base.Awake();
        animationMeshRenderer = this.GetComponent<Renderer>() as MeshRenderer;
        m_AnimationPlayer = GetComponent<GuiPlaneAnimationPlayer>();
        m_AnimationPlayer.playMode = GuiPlaneAnimationPlayer.PlayMode.Mode_PlayLoop;
        m_AnimationPlayer.IsAutoPlay = true;
        m_AnimationPlayer.IsAutoDel = false;

    }
/// <summary>
/// 独立场景时使用，及不切换场景的
/// </summary>
#if Independent_Scene
 
#else
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (animationMeshRenderer != null)
        {
            for (int i = 0; i < animationMeshRenderer.materials.Length; i++)
            {
            	if(animationMeshRenderer.materials[i] != null)
            	{
            		UnityEngine.GameObject.Destroy(animationMeshRenderer.materials[i]);
            	}
                animationMeshRenderer.materials[i] = null;
            }
            UnityEngine.GameObject.Destroy(animationMeshRenderer);
            animationMeshRenderer = null;
        }
    }
#endif    
    protected virtual void Start()
    {
        currentAction = actionList[0];
        UpdateActionInfoToAnimationPlayer(currentAction, currentActionDirectionIndex);
    }
    //获取指定动作的数据
    public ActionInfo GetActionInfo(string actionName)
    {
        return GetActionInfo(FTLibrary.Command.FTUID.StringGetHashCode(actionName));
    }
    public ActionInfo GetActionInfo(uint actionId)
    {
        for (int i = 0; i < actionList.Length; i++)
        {
            if (actionList[i].actionId == actionId)
            {
                return actionList[i];
            }
        }
        return null;
    }
}
