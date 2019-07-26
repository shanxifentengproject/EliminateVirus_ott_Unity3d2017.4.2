using System;
using System.Collections;
using UniLua;
using UnityEngine;
using FTLibrary.Text;

class LuaScriptFile : LuaFile
{
    public LuaScriptFile()
        :base()
    {
        LUA_ROOT = UniGameResources.ConnectPath(Application.dataPath, "\\LuaScript\\", "LuaRoot\\");
    }
}
