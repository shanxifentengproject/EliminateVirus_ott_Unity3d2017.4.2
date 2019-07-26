using System;
using System.Collections.Generic;
using System.Text;
class SingleProcessControl : IDisposable
{
    private UniGameProcessControl gameProcessControl = null;
    public UniProcessModalEvent currentProcess = null;
    public SingleProcessControl(UniGameProcessControl gameprocesscontrol)
    {
        gameProcessControl = gameprocesscontrol;
    }
    private void TimerProcProcessFadeOutComplete(object[] parameters)
    {
        try
        {
            if (currentProcess == null || currentProcess.processStatus != UniProcessModalEvent.ProcessStatus.Status_Fadeout)
                throw new Exception("TimerProcCurrentProcessFadeOutComplete call err!");
            currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Dispose;
            currentProcess.Dispose();
        }
        catch (System.Exception ex)
        {
            gameProcessControl.ShowExceptionError(ex);
        }
        SetCurrentProcess(parameters[0] as UniProcessModalEvent);
    }
    private void TimerProcProcessFadeInComplete(object[] parameters)
    {
        try
        {
            if (currentProcess == null || currentProcess.processStatus != UniProcessModalEvent.ProcessStatus.Status_Fadein)
                throw new Exception("TimerProcCurrentProcessFadeInComplete call err!");
            currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Working;
        }
        catch (System.Exception ex)
        {
            gameProcessControl.ShowExceptionError(ex);
        }

    }
    public void SetCurrentProcess(UniProcessModalEvent process)
    {
        if (currentProcess != null)
        {
            try
            {
                UniProcessModalEvent.ProcessFadeData fadeData = currentProcess.FadeoutData;
                if (fadeData.isEffect)
                {
                    //需要淡出
                    currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Fadeout;
                    gameProcessControl.TimerCall(TimerProcProcessFadeOutComplete, fadeData.fadeTime, false, process);
                    //调用淡出函数
                    currentProcess.Fadeout();
                    return;
                }
                currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Dispose;
                currentProcess.Dispose();
            }
            catch (System.Exception ex)
            {
                gameProcessControl.ShowExceptionError(ex);
            }

        }
        currentProcess = process;
        if (currentProcess != null)
        {
            try
            {
                currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Initialization;
                currentProcess.processControl = gameProcessControl;
                currentProcess.Initialization();
                UniProcessModalEvent.ProcessFadeData fadeData = currentProcess.FadeinData;
                if (fadeData.isEffect)
                {
                    //需要淡入
                    currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Fadein;
                    gameProcessControl.TimerCall(TimerProcProcessFadeInComplete, fadeData.fadeTime, false, null);
                    //调用淡出函数
                    currentProcess.Fadein();
                    return;
                }
                currentProcess.processStatus = UniProcessModalEvent.ProcessStatus.Status_Working;
            }
            catch (System.Exception ex)
            {
                gameProcessControl.ShowExceptionError(ex);
                currentProcess.OnProcessErr();
            }
        }
    }
    public void ActivateProcess(Type type)
    {
        UniProcessModalEvent process = null;
        try
        {
            process = Activator.CreateInstance(type) as UniProcessModalEvent;
        }
        catch (System.Exception ex)
        {
            gameProcessControl.ShowExceptionError(ex);
            return;
        }
        SetCurrentProcess(process);
    }
    public void Dispose()
    {
        gameProcessControl = null;
        if (currentProcess != null)
        {
            currentProcess.Dispose();
            currentProcess = null;
        }
    }
}
