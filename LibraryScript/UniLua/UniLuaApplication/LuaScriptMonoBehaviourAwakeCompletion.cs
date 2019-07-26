using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniLua/UniLuaApplication/LuaScriptMonoBehaviourAwakeCompletion")]
class LuaScriptMonoBehaviourAwakeCompletion : LuaScriptMonoBehaviourAwake
{
    public string linkLuaScriptName = "";
    public string LuaMethodMask = "0xFFFFFFFF";
    //获取加载形式
    protected override LuaScriptType luaScriptType { get { return LuaScriptType.Type_Name; } }
    //获取脚本名称
    protected override string LuaScriptName { get { return linkLuaScriptName; } }
    //获得绑定函数掩码
    protected override uint LuaStoreMethodMask { get { return (uint)FTLibrary.Command.FTConvert.AutoToInt32(LuaMethodMask); } }
    protected override void Awake()
    {
        base.Awake();
        Lua.CallAwake();
    }
    protected virtual void Start()
    {
        Lua.CallStart();
    }
    protected virtual void Update()
    {
        Lua.CallUpdate();
    }
    protected virtual void FixedUpdate()
    {
        Lua.CallFixedUpdate();
    }
    protected virtual void LetUpdate()
    {
        Lua.CallLateUpdate();
    }
}
