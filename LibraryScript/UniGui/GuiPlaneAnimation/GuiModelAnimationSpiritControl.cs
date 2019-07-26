using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/GuiModelAnimationSpiritControl")]
public class GuiModelAnimationSpiritControl : MonoBehaviourIgnoreGui
{
    private Animation m_AnimationPlayer = null;
    public Animation animationPlayer { get { return m_AnimationPlayer; } }

    [System.Serializable]
    public class ActionInfo
    {
        public string actionName = "";
        public uint actionId { set; get; }
        public AnimationClip actionObj = null;
        public float speed = 1.0f;
        //精灵的方向动画贴图
        //顺时针计算，从右开始
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
    public int m_CurrentActionDirectionIndex = 0;
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
            Vector3 direction = value;
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

    private Vector3 DirectionTransform(int actionDirection)
    {
        switch(currentActionDirectionIndex)
        {
            case 0:
                return new Vector3(0.0f, 180.0f, 0.0f);
            case 1:
                 return new Vector3(0.0f, 225.0f, 0.0f);
                 //return new Vector3(0.0f, 135.0f, 0.0f);
            case 2:
                return new Vector3(0.0f, 270.0f, 0.0f);
                //return new Vector3(0.0f, 90.0f, 0.0f);
            case 3:
                return new Vector3(0.0f, 315.0f, 0.0f);
                //return new Vector3(0.0f, 45.0f, 0.0f);
            case 4:
                return new Vector3(0.0f, 0.0f, 0.0f);
            case 5:
                return new Vector3(0.0f, 45.0f, 0.0f);
                //return new Vector3(0.0f, 315.0f, 0.0f);
            case 6:
                return new Vector3(0.0f, 90.0f, 0.0f);
                //return new Vector3(0.0f, 270.0f, 0.0f);
            case 7:
                return new Vector3(0.0f, 135.0f, 0.0f);
                //return new Vector3(0.0f, 225.0f, 0.0f);
            }
            return Vector3.zero;
    }
    protected void UpdateActionDirectionToAnimationPlayer(ActionInfo info,int actionDirection)
    {
        gameObject.transform.eulerAngles = DirectionTransform(actionDirection);
    }
    protected void UpdateActionInfoToAnimationPlayer(ActionInfo info,int actionDirection)
    {
        m_AnimationPlayer.CrossFade(info.actionName, 0.1f, PlayMode.StopAll);
        //Debug.Break();
        gameObject.transform.eulerAngles = DirectionTransform(actionDirection);
        //m_AnimationPlayer.TransformAnimation();
    }
    protected virtual void Start()
    {
        m_AnimationPlayer = GetComponent<Animation>();
        m_AnimationPlayer.wrapMode = WrapMode.Loop;
        currentAction = actionList[0];
        for (int i = 0; i < actionList.Length; i++)
        {
            ActionInfo info = actionList[i];
            actionList[i].actionId = FTLibrary.Command.FTUID.StringGetHashCode(actionList[i].actionName);
            m_AnimationPlayer[info.actionName].speed = info.speed;
        }
        m_AnimationPlayer.Play();
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

    //当前动画的播放方式
    public WrapMode aniWrapMode { set { m_AnimationPlayer.wrapMode = value; if (value == WrapMode.Once)IsCallBack = false; } get { return m_AnimationPlayer.wrapMode; } }

    //是否回调
    public bool IsCallBack = false;
    //当动画模式是只调一次后回调
    public delegate void AniPlayOver();
    public AniPlayOver delegateAniPlayOver = null;
    void Update()
    {
        if (aniWrapMode == WrapMode.Once)
        {
            if (m_AnimationPlayer.isPlaying == false && IsCallBack == false)
            {
                if (delegateAniPlayOver != null)
                {
                    delegateAniPlayOver();
                    IsCallBack = true;
                }
            }
        }
    }
}
