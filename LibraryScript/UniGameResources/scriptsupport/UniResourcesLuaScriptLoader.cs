using System;
using System.IO;
using System.Collections.Generic;
using FTLibrary.Text;
using UniLua;
using UnityEngine;


class UniResourcesLuaScriptLoader : LuaFile
{
    
    public UniResourcesLuaScriptLoader()
        :base()
    {
        LUA_ROOT = UniGameResources.ConnectPath(Application.dataPath, "\\",
                    UniGameResourcesDefine.DefineAssetBundleName_LuaScript, 
                    "\\LuaRoot\\");
    }
    protected override string GetFilePath(string filename)
    {
        return UniGameResources.currentUniGameResources.LoadResource_GetLuaScriptFilePath(filename);
    }
    public override MemoryStream ReadFile(string filename)
    {
        return UniGameResources.currentUniGameResources.LoadResource_LuaScript(GetFilePath(filename));
    }
    public override bool IsExistsFile(string filename)
    {
        return UniGameResources.currentUniGameResources.LoadResource_IsHaveLuaScript(GetFilePath(filename));
    }
}
