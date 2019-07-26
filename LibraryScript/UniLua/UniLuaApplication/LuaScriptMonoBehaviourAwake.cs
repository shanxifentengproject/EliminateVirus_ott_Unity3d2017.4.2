using System;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("UniLua/UniLuaApplication/LuaScriptMonoBehaviourAwake")]
class LuaScriptMonoBehaviourAwake : LuaScriptMonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        InitializationLua();
    }
}

