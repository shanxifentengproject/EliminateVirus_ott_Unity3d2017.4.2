using System;
using System.Collections.Generic;
using System.Text;
class UniLuaResourcesScript : LuaScriptController
{
    private int SelectCurrentLanguageRef = 0;
    public UniLuaResourcesScript()
        :base()
    {
        InitializationName(UniGameResourcesDefine.ResourcesLuaScriptName,
                0x00000000,
                OnMyStoreMethod);
    }
    private void OnMyStoreMethod()
    {
        SelectCurrentLanguageRef = StoreLuaMethod("SelectCurrentLanguage");
    }
    public UniGameResources.LanguageDefine SelectCurrentLanguage(UniGameResources.LanguageDefine DefineLanguage,
                    UniGameResources.LanguageDefine LoaclLanguage,
                    UniGameResources.LanguageDefine SystemLanguage,
                    Dictionary<uint, UniGameResources.LanguageDefine> LanguageDefineList)
    {
        //首先给LUA建立表
        List<UniGameResources.LanguageDefine> tlist=new List<UniGameResources.LanguageDefine>(32);
        Dictionary<uint, UniGameResources.LanguageDefine>.Enumerator list=LanguageDefineList.GetEnumerator();
        while(list.MoveNext())
        {
            tlist.Add(list.Current.Value);
        }
        list.Dispose();
        object[] parameters = new object[tlist.Count];
        for (int i = 0; i < tlist.Count;i++ )
        {
            parameters[i] = (object)(tlist[i].languageId);
        }
        NewLuaTable("LanguageList", parameters);
        uint selId = CallLuaMethod_UInt32(SelectCurrentLanguageRef, DefineLanguage.languageId, LoaclLanguage.languageId, SystemLanguage.languageId);
        UniGameResources.LanguageDefine ret;
        if (!LanguageDefineList.TryGetValue(selId, out ret))
            return new UniGameResources.LanguageDefine();
        return ret;
    }
}
