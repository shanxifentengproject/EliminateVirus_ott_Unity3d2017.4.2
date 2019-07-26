//#define _Develop
//#define _Release
using System;
using UnityEngine;
using System.Collections.Generic;
using FTLibrary.Verify;


partial class GameRoot
{
//弹出报错提示框
    public static void Error(string msg)
    {
        Debug.Log(msg);
        
    }
    public static void ErrorClearFirst(string msg)
    {
        Debug.Log(msg);
        
    }
    public static List<string> non_fatal_error_list = null;
    public static void NonFatalErro(string msg)
    {
        if (non_fatal_error_list == null)
            non_fatal_error_list = new List<string>(8);
        non_fatal_error_list.Add(msg);
    }
}