using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class UniSystemSetting
{
    //是否关闭屏幕休眠
    public static bool IsScreenSleep
    {
        get { return Screen.sleepTimeout == SleepTimeout.NeverSleep; }
        set
        {
            if (value)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
        }
    }
}
