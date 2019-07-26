using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Time;
using UnityEngine;
using System.Collections;
class UniProcessModalEvent : FTModalEvent
{
    public UniGameProcessControl processControl { get; set; }
    //过程类型
    public virtual ModalProcessType processType { get { return ModalProcessType.Process_Unknow; } }
    //操作锁，锁定后就不在响应设备输入操作
    public bool InputImmediatelyLocker = false;
    //时间锁
    private FTLibrary.Time.TimeLocker m_InputTimeLocker=new FTLibrary.Time.TimeLocker(0);
    public virtual float InputLockTime { get { return 0.5f; } }
    public bool InputTimeLocker
    {
        get { return m_InputTimeLocker.IsLocked; }
        set
        {
            m_InputTimeLocker = new FTLibrary.Time.TimeLocker((int)(InputLockTime * 1000f));
            m_InputTimeLocker.IsLocked = value;
        }
    }

    public enum ProcessStatus
    {
        Status_WaitWork,
        Status_Initialization,  //初始化
        Status_Working,         //工作
        Status_Dispose,         //释放
        Status_Fadein,          //淡入
        Status_Fadeout          //淡出
    }
    public ProcessStatus processStatus = ProcessStatus.Status_WaitWork;
    public virtual bool IsCanUpdate 
    { 
        get 
        {
            return processStatus == ProcessStatus.Status_Working || processStatus == ProcessStatus.Status_Fadein || processStatus == ProcessStatus.Status_Fadeout;
        } 
    }
    public struct ProcessFadeData
    {
        public bool isEffect;
        public long fadeTime;
        public float fadeA;
        public EventHandler fadeEndEvent;
        public ProcessFadeData(bool effect,long t,float a,EventHandler fun)
        {
            isEffect = effect;
            fadeTime = t;
            fadeA = a;
            fadeEndEvent = fun;
        }
    }
    //淡出时间
    public virtual ProcessFadeData FadeoutData { get { return new ProcessFadeData(); } }
    //淡入时间
    public virtual ProcessFadeData FadeinData { get { return new ProcessFadeData(); } }

    public UniProcessModalEvent()
        :base()
    {
        
    }
    public UniProcessModalEvent(int eventCapacity)
        : base(eventCapacity)
    {

    }
    //初始化函数
    public virtual void Initialization()
    {

    }
    //过程刷新
    public virtual void OnLateUpdate()
    {

    }
    public virtual void OnFixedUpdate()
    {

    }
    //显示
    public virtual void OnShow()
    {

    }
    //隐藏
    public virtual void OnHide()
    {

    }
    //发生异常的处理函数
    public virtual void OnProcessErr()
    {

    }

    //淡出函数
    public virtual void Fadeout()
    {

    }
    //淡入函数
    public virtual void Fadein()
    {

    }
}
