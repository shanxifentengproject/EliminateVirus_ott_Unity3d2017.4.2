using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using FTLibrary.Time;
class UniGameProcessControl : MonoBehaviourIgnoreGui
{
    //自身时间过程
    private FTModalEvent processControlEventModal = new FTModalEvent(16);
    public void ShowExceptionError(System.Exception ex)
    {
        if (Application.isEditor)
        {
            Debug.LogError("异常处理 ===  "+ ex.ToString());
        }
    }
    protected override void Awake()
    {
        base.Awake();
        UnityEngine.Object.DontDestroyOnLoad(this);
        currentProcessControl = new SingleProcessControl(this);
        currentSceneProcessControl = new SingleProcessControl(this);
    }
    public FTTimeCallProc TimerCall(FTEventCallbackHandler fun, long delayTime)
    {
        return processControlEventModal.TimerCall(fun, delayTime);
    }
    public FTTimeCallProc TimerCall(FTEventCallbackHandler fun, long delayTime, bool loop)
    {
        return processControlEventModal.TimerCall(fun, delayTime, loop);
    }
    public FTTimeCallProc TimerCall(FTEventCallbackHandler fun, long delayTime, bool loop, params object[] patameters)
    {
        return processControlEventModal.TimerCall(fun, delayTime, loop, patameters);
    }

    public int AllocEvent(FTEventHandler cFun)
    {
        return processControlEventModal.AllocEvent(cFun);
    }
    public int AllocEvent(FTEventHandler cFun, FTEventCallbackHandler tFun, long timeout)
    {
        return processControlEventModal.AllocEvent(cFun, tFun, timeout);
    }



    //当前的过程
    protected SingleProcessControl currentProcessControl = null;
    public UniProcessModalEvent currentProcess { get { return currentProcessControl.currentProcess; } }
    public virtual void SetCurrentProcess(UniProcessModalEvent process)
    {
        currentProcessControl.SetCurrentProcess(process);
    }
    public virtual void ActivateProcess(Type type)
    {
        currentProcessControl.ActivateProcess(type);
    }
   



    //当前场景过程
    protected SingleProcessControl currentSceneProcessControl = null;
    public UniProcessModalEvent currentSceneProcess { get { return currentSceneProcessControl.currentProcess; } }
    public virtual void SetCurrentSceneProcess(UniProcessModalEvent process)
    {
        currentSceneProcessControl.SetCurrentProcess(process);
    }
    public virtual void ActivateSceneProcess(Type type)
    {
        currentSceneProcessControl.ActivateProcess(type);
    }
    public virtual void CloseSceneProcess()
    {
        SetCurrentSceneProcess(null);
    }

    //独立过程
    //独立过程不支持淡入淡出
    protected Dictionary<int, UniProcessModalEvent> aloneProcessList = new Dictionary<int, UniProcessModalEvent>(16);
    protected List<UniProcessModalEvent> updateAloneProcessList = new List<UniProcessModalEvent>(16);
    protected bool aloneProcessListModify=false;
    protected List<int> aloneProcessRemoveList = new List<int>(16);
    public virtual void ActivateAloneProcess(Type type)
    {
        UniProcessModalEvent process = null;
        try
        {
            process = Activator.CreateInstance(type) as UniProcessModalEvent;
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
            return;
        }
        try
        {
            UniProcessModalEvent oldprocess;
            if (aloneProcessList.TryGetValue((int)process.processType, out oldprocess))
            {
                try
                {
                    oldprocess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Dispose;
                    oldprocess.Dispose();
                }
                catch (System.Exception ex)
                {
                    ShowExceptionError(ex);
                }
                aloneProcessList.Remove((int)process.processType);
            }
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
        }
        try
        {
            aloneProcessList.Add((int)process.processType, process);
            aloneProcessListModify = true;
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
        }
        try
        {
            process.processStatus = UniProcessModalEvent.ProcessStatus.Status_Initialization;
            process.processControl=this;
            process.Initialization();
            process.processStatus = UniProcessModalEvent.ProcessStatus.Status_Working;
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
            process.OnProcessErr();
        }
    }
    public virtual void CloseAloneProcess(ModalProcessType type)
    {
        try
        {
            UniProcessModalEvent oldprocess;
            if (aloneProcessList.TryGetValue((int)type, out oldprocess))
            {
                try
                {
                    oldprocess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Dispose;
                    oldprocess.Dispose();
                }
                catch (System.Exception ex)
                {
                    ShowExceptionError(ex);
                }
                aloneProcessRemoveList.Add((int)type);
            }
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
        }

    }
    public virtual UniProcessModalEvent GetAloneProcess(ModalProcessType type)
    {
        try
        {
            UniProcessModalEvent oldprocess;
            if (aloneProcessList.TryGetValue((int)type, out oldprocess))
            {
                return oldprocess;
            }
        }
        catch (System.Exception ex)
        {
            ShowExceptionError(ex);
        }

        return null;
    }
    private void UpdateUpdateAloneProcessList()
    {
        if (!aloneProcessListModify)
            return;
        updateAloneProcessList.Clear();
        Dictionary<int, UniProcessModalEvent>.Enumerator list = aloneProcessList.GetEnumerator();
        while (list.MoveNext())
        {
            updateAloneProcessList.Add(list.Current.Value);
        }
        list.Dispose();
        aloneProcessListModify = false;
    }
    private void AloneProcessRemoveProc()
    {
        if (aloneProcessRemoveList.Count == 0)
            return;
        int nCount = aloneProcessRemoveList.Count;
        for (int i = 0; i < nCount; i++)
        {
            aloneProcessList.Remove(aloneProcessRemoveList[i]);
        }
        aloneProcessRemoveList.Clear();
        if (aloneProcessList.Count != 0)
        {
            aloneProcessListModify = true;
            return;
        }
        aloneProcessListModify = false;
        updateAloneProcessList.Clear();
    }


    protected virtual void Update()
    {
        if (currentProcess != null && currentProcess.IsCanUpdate)
        {
            try
            {
                currentProcess.OnUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentProcess.OnProcessErr();
            }
        }
        if (currentSceneProcess != null && currentSceneProcess.IsCanUpdate)
        {
            try
            {
                currentSceneProcess.OnUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentSceneProcess.OnProcessErr();
            }

        }
        
        if (aloneProcessList.Count > 0)
        {
            //Dictionary<int, UniProcessModalEvent>.Enumerator list = aloneProcessList.GetEnumerator();
            //while (list.MoveNext())
            //{
            //    if (list.Current.Value.IsCanUpdate)
            //    {
            //        try
            //        {
            //            list.Current.Value.OnUpdate();
            //        }
            //        catch (System.Exception ex)
            //        {
            //            ShowExceptionError(ex);
            //            list.Current.Value.OnProcessErr();
            //        }
            //    }
            //}
            //list.Dispose();
            UpdateUpdateAloneProcessList();
            for (int i = 0; i < updateAloneProcessList.Count;i++ )
            {
                if (updateAloneProcessList[i].IsCanUpdate)
                {
                    try
                    {
                        updateAloneProcessList[i].OnUpdate();
                    }
                    catch (System.Exception ex)
                    {
                        ShowExceptionError(ex);
                        updateAloneProcessList[i].OnProcessErr();
                    }
                }
            }
            AloneProcessRemoveProc();
        }
        
    }
    protected virtual void LateUpdate()
    {
        if (currentProcess != null && currentProcess.IsCanUpdate)
        {
            try
            {
                currentProcess.OnLateUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentProcess.OnProcessErr();
            }
            
        }
        if (currentSceneProcess != null && currentSceneProcess.IsCanUpdate)
        {
            try
            {
                currentSceneProcess.OnLateUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentSceneProcess.OnProcessErr();
            }
            
        }
        if (aloneProcessList.Count > 0)
        {
            //Dictionary<int, UniProcessModalEvent>.Enumerator list = aloneProcessList.GetEnumerator();
            //while (list.MoveNext())
            //{
            //    if (list.Current.Value.IsCanUpdate)
            //    {
            //        try
            //        {
            //            list.Current.Value.OnLateUpdate();
            //        }
            //        catch (System.Exception ex)
            //        {
            //            ShowExceptionError(ex);
            //            list.Current.Value.OnProcessErr();
            //        }
            //    }
            //}
            //list.Dispose();
            UpdateUpdateAloneProcessList();
            for (int i = 0; i < updateAloneProcessList.Count; i++)
            {
                if (updateAloneProcessList[i].IsCanUpdate)
                {
                    try
                    {
                        updateAloneProcessList[i].OnLateUpdate();
                    }
                    catch (System.Exception ex)
                    {
                        ShowExceptionError(ex);
                        updateAloneProcessList[i].OnProcessErr();
                    }
                }
            }
            AloneProcessRemoveProc();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (currentProcess != null && currentProcess.IsCanUpdate)
        {
            try
            {
                currentProcess.OnFixedUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentProcess.OnProcessErr();
            }
            
        }
        if (currentSceneProcess != null && currentSceneProcess.IsCanUpdate)
        {
            try
            {
                currentSceneProcess.OnFixedUpdate();
            }
            catch (System.Exception ex)
            {
                ShowExceptionError(ex);
                currentSceneProcess.OnProcessErr();
            }
            
        }
        if (aloneProcessList.Count > 0)
        {
            //Dictionary<int, UniProcessModalEvent>.Enumerator list = aloneProcessList.GetEnumerator();
            //while (list.MoveNext())
            //{
            //    if (list.Current.Value.IsCanUpdate)
            //    {
            //        try
            //        {
            //            list.Current.Value.OnFixedUpdate();
            //        }
            //        catch (System.Exception ex)
            //        {
            //            ShowExceptionError(ex);
            //            list.Current.Value.OnProcessErr();
            //        }
            //    }
            //}
            //list.Dispose();
            UpdateUpdateAloneProcessList();
            for (int i = 0; i < updateAloneProcessList.Count; i++)
            {
                if (updateAloneProcessList[i].IsCanUpdate)
                {
                    try
                    {
                        updateAloneProcessList[i].OnFixedUpdate();
                    }
                    catch (System.Exception ex)
                    {
                        ShowExceptionError(ex);
                        updateAloneProcessList[i].OnProcessErr();
                    }
                }
            }
            AloneProcessRemoveProc();
        }
    }
    protected virtual void OnDestroy()
    {
        if (currentProcessControl != null)
        {
            currentProcessControl.Dispose();
            currentProcessControl = null;
        }
        if (currentSceneProcessControl != null)
        {
            currentSceneProcessControl.Dispose();
            currentSceneProcessControl = null;
        }
        if (aloneProcessList.Count > 0)
        {
            Dictionary<int, UniProcessModalEvent>.Enumerator list = aloneProcessList.GetEnumerator();
            while (list.MoveNext())
            {
                list.Current.Value.Dispose();
            }
            list.Dispose();
            aloneProcessList.Clear();
            updateAloneProcessList.Clear();
            aloneProcessRemoveList.Clear();
        }
    }
    
    


    
}
