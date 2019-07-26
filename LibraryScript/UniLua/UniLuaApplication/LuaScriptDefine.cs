using System;
using System.Collections.Generic;
using System.Text;

class LuaScriptDefine
{
    public const uint StoreMethodMask_Awake = 0x00000001;
    public const uint StoreMethodMask_Start = 0x00000002;
    public const uint StoreMethodMask_Update = 0x00000004;
    public const uint StoreMethodMask_LateUpdate = 0x00000008;
    public const uint StoreMethodMask_FixedUpdate = 0x00000010;
}
enum LuaScriptType
{
    Type_Name,
    Type_File,  //文件形式
    Type_String,//字符串形式
    Type_Bytes //字节码
}
