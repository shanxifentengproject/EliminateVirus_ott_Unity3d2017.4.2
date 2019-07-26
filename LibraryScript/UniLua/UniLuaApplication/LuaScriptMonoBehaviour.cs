using System;
using System.Collections;
using UnityEngine;
[AddComponentMenu("UniLua/UniLuaApplication/LuaScriptMonoBehaviour")]
class LuaScriptMonoBehaviour : MonoBehaviourIgnoreGui
{
    protected LuaScriptController Lua = null;
    
    //获取加载形式
    protected virtual LuaScriptType luaScriptType { get { return LuaScriptType.Type_Name; } }
    //获取脚本名称
    protected virtual string LuaScriptName { get { return ""; } }
    //获得脚本文件路径
    protected virtual string LuaScriptFile { get { return ""; } }
    //获得脚本字符串
    protected virtual string LuaScriptString { get { return ""; } }
    //获得脚本字节码
    protected virtual byte[] LuaScriptBytes { get { return null; } }
    protected virtual string LuaScriptBytesName { get { return ""; } }
    //获得绑定函数掩码
    protected virtual uint LuaStoreMethodMask { get { return 0xFFFFFFFF; } }
    //获得绑定函数回掉
    protected virtual LuaScriptController.OnStoreMethod LuaStoreMethod { get { return null; } }
    //初始化LUA
    protected virtual void InitializationLua()
    {
        try
        {
            Lua = new LuaScriptController();
            switch (luaScriptType)
            {
                case LuaScriptType.Type_Name:
                    Lua.InitializationName(LuaScriptName, LuaStoreMethodMask, LuaStoreMethod);
                    break;
                case LuaScriptType.Type_File:
                    Lua.InitializationFile(LuaScriptFile, LuaStoreMethodMask, LuaStoreMethod);
                    break;
                case LuaScriptType.Type_String:
                    Lua.Initialization(LuaScriptString, LuaStoreMethodMask, LuaStoreMethod);
                    break;
                case LuaScriptType.Type_Bytes:
                    Lua.Initialization(LuaScriptBytes, LuaScriptBytesName, LuaStoreMethodMask, LuaStoreMethod);
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
}