using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
[AddComponentMenu("UniGui/GuiPlaneAnimation/GuiPlaneAnimationPlayer")]
class GuiPlaneAnimationPlayer : MonoBehaviourIgnoreGui
{
    protected GuiPlaneAnimationControl[] animationControlList { get; set; }
    //播放时间
    public float playTime = 1.0f;
    private float m_CurrentPlayTime = 0.0f;
    private float m_PlayProgress = 0.0f;
    public float currentPlayTime
    
    {
        get { return m_CurrentPlayTime; }
        set
        {
            m_CurrentPlayTime = value;
            m_PlayProgress = Mathf.Clamp(m_CurrentPlayTime / playTime, 0.0f, 1.0f);
        }
    }
    public float playProgress
    {
        get { return m_PlayProgress; }
        set
        {
            m_PlayProgress = value;
            m_CurrentPlayTime = playTime * m_PlayProgress;
        }
    }
    //播放类型
    public enum PlayMode
    {
        Mode_PlayOnec,
        Mode_PlayLoop
    }
    public PlayMode playMode = PlayMode.Mode_PlayOnec;
    //是否自动播放
    public bool IsAutoPlay = false;
    //播放完是否自动删除
    public bool IsAutoDel = false;

    //播放状态
    public enum PlayStatus
    {
        Status_Stop,
        Status_Pause,
        Status_Play
    }
    public PlayStatus playStatus { get; set; }

    protected override void Awake()
    {
        base.Awake();
        animationControlList = GetComponentsInChildren<GuiPlaneAnimationControl>();
    }
    protected virtual void Start()
    {
        //这里如果程序构造出来对象后直接调用播放
        //会先于这个函数调用，这样如果没有勾选自动播放
        //就出现这里停止播放的问题
        //甚至重复调用函数
        //所以这里判断是否当前正在播放则这个函数不做处理
        if (playStatus != PlayStatus.Status_Play)
        {
            Stop();
            if (IsAutoPlay)
            {
                Play();
            }
        }
    }
    public void TransformAnimation()
    {
        for (int i=0;i<animationControlList.Length;i++)
        {
            animationControlList[i].TransformAnimation(playProgress);
        }
    }
    public void Play()
    {
        playStatus = PlayStatus.Status_Play;
        OnPlay();
        if (DelegateOnPlayEvent != null)
            DelegateOnPlayEvent();
        if (DelegateOnPlayEventNew != null)
            DelegateOnPlayEventNew(this);
    }
    public void Stop()
    {
        playStatus = PlayStatus.Status_Stop;
        playProgress = 0.0f;
        TransformAnimation();
        OnStop();
        if (DelegateOnStopEvent != null)
            DelegateOnStopEvent();
        if (DelegateOnStopEventNew != null)
            DelegateOnStopEventNew(this);
    }
    public void Pause()
    {
        playStatus = PlayStatus.Status_Pause;
        OnPause();
        if (DelegateOnPauseEvent != null)
            DelegateOnPauseEvent();
        if (DelegateOnPauseEventNew != null)
            DelegateOnPauseEventNew(this);
    }
    protected virtual void Update()
    {
        if (playStatus == PlayStatus.Status_Stop ||
                playStatus == PlayStatus.Status_Pause)
        {
            return;
        }
        else if (playStatus == PlayStatus.Status_Play)
        {
            currentPlayTime += Time.deltaTime;
            TransformAnimation();
            if (playProgress == 1.0f)
            {
                switch (playMode)
                {
                    case PlayMode.Mode_PlayOnec:
                        {
                            Pause();
                            OnPlayEnd();
                            if (DelegateOnPlayEndEvent != null)
                                DelegateOnPlayEndEvent();
                            if (DelegateOnPlayEndEventNew != null)
                                DelegateOnPlayEndEventNew(this);
                            if (IsAutoDel)
                            {
                                UnityEngine.GameObject.DestroyObject(gameObject);
                            }
                        }
                        break;
                    case PlayMode.Mode_PlayLoop:
                        {
                            playProgress = 0.0f;
                            TransformAnimation();
                        }
                        break;
                }
            }
        }
    }

    public delegate void OnPlayDelegateEvent();
    public OnPlayDelegateEvent DelegateOnPlayEvent = null;
    public OnPlayDelegateEvent DelegateOnStopEvent = null;
    public OnPlayDelegateEvent DelegateOnPauseEvent = null;
    public OnPlayDelegateEvent DelegateOnPlayEndEvent = null;
    public delegate void OnPlayDelegateEventNew(GuiPlaneAnimationPlayer ani);
    public OnPlayDelegateEventNew DelegateOnPlayEventNew = null;
    public OnPlayDelegateEventNew DelegateOnStopEventNew = null;
    public OnPlayDelegateEventNew DelegateOnPauseEventNew = null;
    public OnPlayDelegateEventNew DelegateOnPlayEndEventNew = null;


    public virtual void OnPlay()
    {

    }
    public virtual void OnStop()
    {

    }
    public virtual void OnPause()
    {

    }
    public virtual void OnPlayEnd()
    {

    }
}
