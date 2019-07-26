using System;
using System.Collections;
using UniLua;
using UnityEngine;
public class LuaScriptController
{
    private ILuaState Lua = null;
    private int AwakeRef = 0;
    private int StartRef = 0;
    private int UpdateRef = 0;
    private int LateUpdateRef = 0;
    private int FixedUpdateRef = 0;

    public delegate void OnStoreMethod();
    public static void LogError(string msg)
    {
        Debug.LogError(msg);
    }
    // StoreMethod 和 CallMethod 的实现
    private int StoreMethod(string name)
    {
        Lua.GetField(-1, name);
        if (!Lua.IsFunction(-1))
        {
            throw new Exception(string.Format(
                "method {0} not found!", name));
        }
        return Lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
    }
    private void CallMethod(int funcRef)
    {
        Lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

        // insert `traceback' function
        int b = Lua.GetTop();
        Lua.PushCSharpFunction(Traceback);
        Lua.Insert(b);

        var status = Lua.PCall(0, 0, b);
        if (status != ThreadStatus.LUA_OK)
        {
            LuaScriptController.LogError(Lua.ToString(-1));
        }

        // remove `traceback' function
        Lua.Remove(b);
    }
    private static int Traceback(ILuaState lua)
    {
        string msg = lua.ToString(1);
        if (msg != null)
        {
            lua.L_Traceback(lua, msg, 1);
        }
        // is there an error object?
        else if (!lua.IsNoneOrNil(1))
        {
            // try its `tostring' metamethod
            if (!lua.L_CallMeta(1, "__tostring"))
            {
                lua.PushString("(no error message)");
            }
        }
        return 1;
    }

    public void InitializationName(string luaScriptName)
    {
        InitializationName(luaScriptName, 0xFFFFFFFF, null);
    }
    public void InitializationName(string luaScriptName, uint storeMethodMask)
    {
        InitializationName(luaScriptName, storeMethodMask, null);
    }
    public void InitializationName(string luaScriptName, OnStoreMethod storeMethod)
    {
        InitializationName(luaScriptName, 0xFFFFFFFF, storeMethod);
    }
    public void InitializationName(string luaScriptName, uint storeMethodMask, OnStoreMethod storeMethod)
    {
        try
        {
            if (UniGameResources.currentUniGameResources == null)
                throw new Exception("UniGameResources.currentUniGameResources == null!");
            FTLibrary.Resources.GameResourcesNode data = new FTLibrary.Resources.GameResourcesNode();
            if (!UniGameResources.currentUniGameResources.FindResources(UniGameResourcesDefine.ResourcesTypeIndex_LuaScript,
                            luaScriptName, ref data))
                throw new Exception(string.Format("cant find lua script!name={0}", luaScriptName));

            if (UniGameResources.gameResourcesWorkMode == UniGameResources.GameResourcesWorkMode.Mode_AloneSecurity)
            {
                try
                {
                    InitializationFile(data.path, storeMethodMask, storeMethod);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
            else if (UniGameResources.gameResourcesWorkMode == UniGameResources.GameResourcesWorkMode.Mode_Mobile)
            {
                UniGameResourcesPackage package = (UniGameResourcesPackage)data.package;
                package.LockPackage();
                try
                {
                    InitializationFile(data.path, storeMethodMask, storeMethod);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
                package.UnLockPackage();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        
    }


    public void InitializationFile(string luaScriptFile)
    {
        InitializationFile(luaScriptFile,0xFFFFFFFF, null);
    }
    public void InitializationFile(string luaScriptFile, uint storeMethodMask)
    {
        InitializationFile(luaScriptFile, storeMethodMask, null);
    }
    public void InitializationFile(string luaScriptFile, OnStoreMethod storeMethod)
    {
        InitializationFile(luaScriptFile, 0xFFFFFFFF, storeMethod);
    }
    public void InitializationFile(string luaScriptFile, uint storeMethodMask, OnStoreMethod storeMethod)
    {
        if (Lua != null)
            return;

        // 创建 Lua 虚拟机
        Lua = LuaAPI.NewState();
        // 加载基本库
        Lua.L_OpenLibs();
        // 加载 Lua 脚本文件
        ThreadStatus status = Lua.L_DoFile(luaScriptFile);
        // 捕获错误
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(Lua.ToString(-1));
        }
        // 确保 framework/main.lua 执行结果是一个 Lua Table
        if (!Lua.IsTable(-1))
        {
            throw new Exception("framework main's return value is not a table");
        }
        // 从 framework/main.lua 返回的 table 中读取 awake 字段指向的函数
        // 并保存到 AwakeRef 中 (可以将 AwakeRef 视为这个函数的句柄)
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Awake) == LuaScriptDefine.StoreMethodMask_Awake)
            AwakeRef = StoreMethod("awake");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Start) == LuaScriptDefine.StoreMethodMask_Start)
            StartRef = StoreMethod("start");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Update) == LuaScriptDefine.StoreMethodMask_Update)
            UpdateRef = StoreMethod("update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_LateUpdate) == LuaScriptDefine.StoreMethodMask_LateUpdate)
            LateUpdateRef = StoreMethod("late_update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_FixedUpdate) == LuaScriptDefine.StoreMethodMask_FixedUpdate)
            FixedUpdateRef = StoreMethod("fixed_update");
        if (storeMethod != null)
        {
            storeMethod();
        }
        // 不再需要 framework/main.lua 返回的 table 了，将其从栈上弹出
        Lua.Pop(1);
    }


    public void Initialization(string luaScriptString)
    {
        Initialization(luaScriptString, 0xFFFFFFFF, null);
    }
    public void Initialization(string luaScriptString, uint storeMethodMask)
    {
        Initialization(luaScriptString, storeMethodMask, null);
    }
    public void Initialization(string luaScriptString, OnStoreMethod storeMethod)
    {
        Initialization(luaScriptString, 0xFFFFFFFF, storeMethod);
    }
    public void Initialization(string luaScriptString, uint storeMethodMask, OnStoreMethod storeMethod)
    {
        if (Lua != null)
            return;
        // 创建 Lua 虚拟机
        Lua = LuaAPI.NewState();
        // 加载基本库
        Lua.L_OpenLibs();
        // 加载 Lua 脚本文件
        ThreadStatus status = Lua.L_DoString(luaScriptString);
        // 捕获错误
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(Lua.ToString(-1));
        }
        // 确保 framework/main.lua 执行结果是一个 Lua Table
        if (!Lua.IsTable(-1))
        {
            throw new Exception("framework main's return value is not a table");
        }
        // 从 framework/main.lua 返回的 table 中读取 awake 字段指向的函数
        // 并保存到 AwakeRef 中 (可以将 AwakeRef 视为这个函数的句柄)
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Awake) == LuaScriptDefine.StoreMethodMask_Awake)
            AwakeRef = StoreMethod("awake");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Start) == LuaScriptDefine.StoreMethodMask_Start)
            StartRef = StoreMethod("start");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Update) == LuaScriptDefine.StoreMethodMask_Update)
            UpdateRef = StoreMethod("update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_LateUpdate) == LuaScriptDefine.StoreMethodMask_LateUpdate)
            LateUpdateRef = StoreMethod("late_update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_FixedUpdate) == LuaScriptDefine.StoreMethodMask_FixedUpdate)
            FixedUpdateRef = StoreMethod("fixed_update");
        if (storeMethod != null)
        {
            storeMethod();
        }
        // 不再需要 framework/main.lua 返回的 table 了，将其从栈上弹出
        Lua.Pop(1);
    }


    public void Initialization(byte[] scriptBytes, string scriptName)
    {
        Initialization(scriptBytes, scriptName, 0xFFFFFFFF, null);
    }
    public void Initialization(byte[] scriptBytes, string scriptName, uint storeMethodMask)
    {
        Initialization(scriptBytes, scriptName, storeMethodMask, null);
    }
    public void Initialization(byte[] scriptBytes, string scriptName, OnStoreMethod storeMethod)
    {
        Initialization(scriptBytes, scriptName, 0xFFFFFFFF, storeMethod);
    }
    public void Initialization(byte[] scriptBytes, string scriptName,uint storeMethodMask, OnStoreMethod storeMethod)
    {
        if (Lua != null)
            return;
        // 创建 Lua 虚拟机
        Lua = LuaAPI.NewState();
        // 加载基本库
        Lua.L_OpenLibs();
        // 加载 Lua 脚本文件
        ThreadStatus status = Lua.L_LoadBytes(scriptBytes, scriptName);
        // 捕获错误
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(Lua.ToString(-1));
        }
        // 确保 framework/main.lua 执行结果是一个 Lua Table
        if (!Lua.IsTable(-1))
        {
            throw new Exception("framework main's return value is not a table");
        }
        // 从 framework/main.lua 返回的 table 中读取 awake 字段指向的函数
        // 并保存到 AwakeRef 中 (可以将 AwakeRef 视为这个函数的句柄)
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Awake) == LuaScriptDefine.StoreMethodMask_Awake)
            AwakeRef = StoreMethod("awake");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Start) == LuaScriptDefine.StoreMethodMask_Start)
            StartRef = StoreMethod("start");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_Update) == LuaScriptDefine.StoreMethodMask_Update)
            UpdateRef = StoreMethod("update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_LateUpdate) == LuaScriptDefine.StoreMethodMask_LateUpdate)
            LateUpdateRef = StoreMethod("late_update");
        if ((storeMethodMask & LuaScriptDefine.StoreMethodMask_FixedUpdate) == LuaScriptDefine.StoreMethodMask_FixedUpdate)
            FixedUpdateRef = StoreMethod("fixed_update");
        if (storeMethod != null)
        {
            storeMethod();
        }
        // 不再需要 framework/main.lua 返回的 table 了，将其从栈上弹出
        Lua.Pop(1);
    }


    public void CallAwake()
    {
        if (AwakeRef != 0)
        {
            CallLuaMethod(AwakeRef);
        }
    }
    public void CallStart()
    {
        if (StartRef != 0)
        {
            CallLuaMethod(StartRef);
        }
    }
    public void CallUpdate()
    {
        if (UpdateRef != 0)
        {
            CallLuaMethod(UpdateRef);
        }
    }
    public void CallLateUpdate()
    {
        if (LateUpdateRef != 0)
        {
            CallLuaMethod(LateUpdateRef);
        }
    }
    public void CallFixedUpdate()
    {
        if (FixedUpdateRef != 0)
        {
            CallLuaMethod(FixedUpdateRef);
        }
    }
    //绑定函数
    public int StoreLuaMethod(string funname)
    {
        return StoreMethod(funname);
    }


    private void CallLuaGlobalFuntion_Command(string funName, object[] parameters, int returnVars)
    {
        Lua.GetGlobal(funName); // 加载 lua 中定义的一个名叫 foo 的全局函数到堆栈
        // 确保加载成功了, 此时栈顶是函数 foo
        if (!Lua.IsFunction(-1))
        {
            throw new Exception(string.Format(
                "method {0} not found!", funName));
        }
        //foreach (object o in parameters)
        for (int i = 0; i < parameters.Length;i++ )
        {
            object o = parameters[i];
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    Lua.PushBoolean((Boolean)o);
                    break;
                case TypeCode.Double:
                    Lua.PushNumber((Double)o);
                    break;
                case TypeCode.Single:
                    Lua.PushNumber((float)o);
                    break;
                case TypeCode.Int32:
                    Lua.PushInteger((Int32)o);
                    break;
                case TypeCode.UInt32:
                    Lua.PushUnsigned((UInt32)o);
                    break;
                case TypeCode.String:
                    Lua.PushString((String)o);
                    break;
                case TypeCode.UInt64:
                    Lua.PushUInt64((UInt64)o);
                    break;
                case TypeCode.Object:
                    {
                        if (o is CSharpFunctionDelegate)
                        {
                            Lua.PushCSharpFunction((CSharpFunctionDelegate)o);
                        }
                        else
                        {
                            throw new Exception("not support type!");
                        }
                    }
                    break;
                default:
                    throw new Exception("not support type!");

            }
        }
        Lua.Call(parameters.Length, returnVars); // 调用函数 foo, 指明有2个参数，没有返回值
        // 上面的代码相当于 lua 里一个这样的调用 foo("test", 42)
    }
    public void CallLuaGlobalFuntion(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 0);
    }
    public object[] CallLuaGlobalFuntion(string funName, TypeCode[] returnTypeList, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, returnTypeList.Length);
        object[] ret=new object[returnTypeList.Length];
        for (int i=0;i<ret.Length;i++)
        {
            int index=i-ret.Length;
            switch (returnTypeList[i])
            {
                case TypeCode.Double:
                    ret[i] = Lua.L_CheckNumber(index);
                    break;
                case TypeCode.Single:
                    ret[i] = (float)Lua.L_CheckNumber(index);
                    break;
                case TypeCode.Int32:
                    ret[i] = Lua.L_CheckInteger(index);
                    break;
                case TypeCode.UInt32:
                    ret[i] = Lua.L_CheckUnsigned(index);
                    break;
                case TypeCode.String:
                    ret[i] = Lua.L_CheckString(index);
                    break;
                case TypeCode.UInt64:
                    ret[i] = Lua.L_CheckUInt64(index);
                    break;
                default:
                    throw new Exception("not support type!");

            }
        }
        Lua.Pop(ret.Length);
        return ret;
    }
    public int CallLuaGlobalFuntion_Int32(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        int ret = Lua.L_CheckInteger(-1);
        Lua.Pop(1);
        return ret;
    }
    public UInt64 CallLuaGlobalFuntion_UInt64(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        UInt64 ret = Lua.L_CheckUInt64(-1);
        Lua.Pop(1);
        return ret;
    }
    public UInt32 CallLuaGlobalFuntion_UInt32(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        UInt32 ret = Lua.L_CheckUnsigned(-1);
        Lua.Pop(1);
        return ret;
    }
    public double CallLuaGlobalFuntion_Double(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        double ret = Lua.L_CheckNumber(-1);
        Lua.Pop(1);
        return ret;
    }
    public float CallLuaGlobalFuntion_Single(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        float ret = (float)Lua.L_CheckNumber(-1);
        Lua.Pop(1);
        return ret;
    }
    public string CallLuaGlobalFuntion_String(string funName, params object[] parameters)
    {
        CallLuaGlobalFuntion_Command(funName, parameters, 1);
        string ret = Lua.L_CheckString(-1);
        Lua.Pop(1);
        return ret;
    }


    //调用lua函数
    public void CallLuaMethod(int funcRef)
    {
        CallMethod(funcRef);
    }
    private void CallLuaMethod_Command(int funcRef, object[] parameters, int returnVars)
    {
        Lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        // insert `traceback' function
        int b = Lua.GetTop();
        Lua.PushCSharpFunction(Traceback);
        Lua.Insert(b);

        //foreach (object o in parameters)
        //{
        for (int i = 0; i < parameters.Length; i++)
        {
            object o = parameters[i];
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    Lua.PushBoolean((Boolean)o);
                    break;
                case TypeCode.Double:
                    Lua.PushNumber((Double)o);
                    break;
                case TypeCode.Single:
                    Lua.PushNumber((float)o);
                    break;
                case TypeCode.Int32:
                    Lua.PushInteger((Int32)o);
                    break;
                case TypeCode.UInt32:
                    Lua.PushUnsigned((UInt32)o);
                    break;
                case TypeCode.String:
                    Lua.PushString((String)o);
                    break;
                case TypeCode.UInt64:
                    Lua.PushUInt64((UInt64)o);
                    break;
                case TypeCode.Object:
                    {
                        if (o is CSharpFunctionDelegate)
                        {
                            Lua.PushCSharpFunction((CSharpFunctionDelegate)o);
                        }
                        else
                        {
                            throw new Exception("not support type!");
                        }
                    }
                    break;
                default:
                    throw new Exception("not support type!");

            }
        }

        var status = Lua.PCall(parameters.Length, returnVars, b);
        if (status != ThreadStatus.LUA_OK)
        {
            LuaScriptController.LogError(Lua.ToString(-1));
        }

        // remove `traceback' function
        Lua.Remove(b);
    }
    public object[] CallLuaMethod(int funcRef, TypeCode[] returnTypeList, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, returnTypeList.Length);
        object[] ret = new object[returnTypeList.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            int index = i - ret.Length;
            switch (returnTypeList[i])
            {
                case TypeCode.Double:
                    ret[i] = Lua.L_CheckNumber(index);
                    break;
                case TypeCode.Single:
                    ret[i] = (float)Lua.L_CheckNumber(index);
                    break;
                case TypeCode.Int32:
                    ret[i] = Lua.L_CheckInteger(index);
                    break;
                case TypeCode.UInt32:
                    ret[i] = Lua.L_CheckUnsigned(index);
                    break;
                case TypeCode.String:
                    ret[i] = Lua.L_CheckString(index);
                    break;
                case TypeCode.UInt64:
                    ret[i] = Lua.L_CheckUInt64(index);
                    break;
                default:
                    throw new Exception("not support type!");

            }
        }
        Lua.Pop(ret.Length);
        return ret;
    }
    public int CallLuaMethod_Int32(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        int ret = Lua.L_CheckInteger(-1);
        Lua.Pop(1);
        return ret;
    }
    public UInt64 CallLuaMethod_UInt64(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        UInt64 ret = Lua.L_CheckUInt64(-1);
        Lua.Pop(1);
        return ret;
    }
    public UInt32 CallLuaMethod_UInt32(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        UInt32 ret = Lua.L_CheckUnsigned(-1);
        Lua.Pop(1);
        return ret;
    }
    public double CallLuaMethod_Double(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        double ret = Lua.L_CheckNumber(-1);
        Lua.Pop(1);
        return ret;
    }
    public float CallLuaMethod_Single(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        float ret = (float)Lua.L_CheckNumber(-1);
        Lua.Pop(1);
        return ret;
    }
    public string CallLuaMethod_String(int funcRef, params object[] parameters)
    {
        CallLuaMethod_Command(funcRef, parameters, 1);
        string ret = Lua.L_CheckString(-1);
        Lua.Pop(1);
        return ret;
    }

    //创建一张全局表
    //这个函数不会处理元表
    public void NewLuaTable(string tableName, params object[] element)
    {
        //在栈中创建一个新表
        Lua.NewTable();
        //让所有元素压入栈成为表元素
        //首先是元素所有入栈
        //然后是元素值入栈
        for (int i = 0; i < element.Length;i++ )
        {
            Lua.PushInteger(i);
            object o = element[i];
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    Lua.PushBoolean((Boolean)o);
                    break;
                case TypeCode.Double:
                    Lua.PushNumber((Double)o);
                    break;
                case TypeCode.Single:
                    Lua.PushNumber((float)o);
                    break;
                case TypeCode.Int32:
                    Lua.PushInteger((Int32)o);
                    break;
                case TypeCode.UInt32:
                    Lua.PushUnsigned((UInt32)o);
                    break;
                case TypeCode.String:
                    Lua.PushString((String)o);
                    break;
                case TypeCode.UInt64:
                    Lua.PushUInt64((UInt64)o);
                    break;
                case TypeCode.Object:
                    {
                        if (o is CSharpFunctionDelegate)
                        {
                            Lua.PushCSharpFunction((CSharpFunctionDelegate)o);
                        }
                        else
                        {
                            throw new Exception("not support type!");
                        }
                    }
                    break;
                default:
                    throw new Exception("not support type!");

            }
            Lua.RawSet(-3);
        }
        /*
         * // 调用lua的函数，都是通过压栈出栈来完成的  
71.    // 为表执行一个t[k]=v的操作,则需要先将k压栈，再将v压栈，再调用操作函数  
72.    // 这个操作函数会使用栈上的元素，并“可能”将弹出元素和压入元素  
73.    // lua_rawset直接赋值（不触发metamethods方法）。   
74.      
75.    // lua_rawset/lua_settable使用:  
76.    // 它从栈中获取参数。以table在栈中的索引作为参数，  
77.    // 并将栈中的key和value出栈。  
78.    // lua_pushnumber函数调用之前，  
79.    // table是在栈顶位置(索引为-1)。index和value入栈之后，  
80.    // table索引变为-3。  
81.    lua_pushnumber( state, 1 );  
82.    lua_pushnumber( state, 45 );  
83.    lua_rawset( state, -3 );  
         * 
         * */
        /*
         * // set the name of the array that the script will access  
            98.    // Pops a value from the stack and sets it as the new value of global name.  
            99.    // 从栈顶弹出一个值，并将其设置全局变量"arg"的新值。  
            100.    lua_setglobal( state, "arg" );  
         * 
         * */
        Lua.SetGlobal(tableName);
    }
}









//public class LuaScriptController : MonoBehaviour
//{
//    public string LuaScriptFile = "framework/main.lua";

//    private ILuaState Lua;
//    private int AwakeRef;
//    private int StartRef;
//    private int UpdateRef;
//    private int LateUpdateRef;
//    private int FixedUpdateRef;

//    void Awake()
//    {
//        Debug.Log("LuaScriptController Awake");

//        if (Lua == null)
//        {
//            Lua = LuaAPI.NewState();
//            Lua.L_OpenLibs();

//            var status = Lua.L_DoFile(LuaScriptFile);
//            if (status != ThreadStatus.LUA_OK)
//            {
//                throw new Exception(Lua.ToString(-1));
//            }

//            if (!Lua.IsTable(-1))
//            {
//                throw new Exception(
//                    "framework main's return value is not a table");
//            }

//            AwakeRef = StoreMethod("awake");
//            StartRef = StoreMethod("start");
//            UpdateRef = StoreMethod("update");
//            LateUpdateRef = StoreMethod("late_update");
//            FixedUpdateRef = StoreMethod("fixed_update");

//            Lua.Pop(1);
//            Debug.Log("Lua Init Done");
//        }

//        CallMethod(AwakeRef);
//    }

//    IEnumerator Start()
//    {
//        CallMethod(StartRef);

//        // -- sample code for loading binary Asset Bundles --------------------
//        String s = "file:///" + Application.streamingAssetsPath + "/testx.unity3d";
//        WWW www = new WWW(s);
//        yield return www;
//        if (www.assetBundle.mainAsset != null)
//        {
//            TextAsset cc = (TextAsset)www.assetBundle.mainAsset;
//            var status = Lua.L_LoadBytes(cc.bytes, "test");
//            if (status != ThreadStatus.LUA_OK)
//            {
//                throw new Exception(Lua.ToString(-1));
//            }
//            status = Lua.PCall(0, 0, 0);
//            if (status != ThreadStatus.LUA_OK)
//            {
//                throw new Exception(Lua.ToString(-1));
//            }
//            Debug.Log("---- call done ----");
//        }
//    }

//    void Update()
//    {
//        CallMethod(UpdateRef);
//    }

//    void LateUpdate()
//    {
//        CallMethod(LateUpdateRef);
//    }

//    void FixedUpdate()
//    {
//        CallMethod(FixedUpdateRef);
//    }

//    private int StoreMethod(string name)
//    {
//        Lua.GetField(-1, name);
//        if (!Lua.IsFunction(-1))
//        {
//            throw new Exception(string.Format(
//                "method {0} not found!", name));
//        }
//        return Lua.L_Ref(LuaDef.LUA_REGISTRYINDEX);
//    }

//    private void CallMethod(int funcRef)
//    {
//        Lua.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);

//        // insert `traceback' function
//        var b = Lua.GetTop();
//        Lua.PushCSharpFunction(Traceback);
//        Lua.Insert(b);

//        var status = Lua.PCall(0, 0, b);
//        if (status != ThreadStatus.LUA_OK)
//        {
//            Debug.LogError(Lua.ToString(-1));
//        }

//        // remove `traceback' function
//        Lua.Remove(b);
//    }

//    private static int Traceback(ILuaState lua)
//    {
//        var msg = lua.ToString(1);
//        if (msg != null)
//        {
//            lua.L_Traceback(lua, msg, 1);
//        }
//        // is there an error object?
//        else if (!lua.IsNoneOrNil(1))
//        {
//            // try its `tostring' metamethod
//            if (!lua.L_CallMeta(1, "__tostring"))
//            {
//                lua.PushString("(no error message)");
//            }
//        }
//        return 1;
//    }
//}
