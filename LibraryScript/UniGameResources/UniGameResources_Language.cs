using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Resources;
using FTLibrary.XML;
using FTLibrary.Text;
using UnityEngine;

partial class UniGameResources : GameResources
{
    internal struct LanguageDefine
    {
        public enum CodeType
        {
            Type_Sys,
            Type_UTF8,
            Type_Unicode,
        }
        
        public uint languageId;
        public string languageName;
        public int languageIndex;
        public SystemLanguage languageSystemId;
        public List<string> LanguageResourcesInventoryFileList;
        public CodeType codeType;
        public static uint LanguageNameToLanguageId(string name)
        {
            return FTLibrary.Command.FTUID.StringGetHashCode(name);
        }
        public static CodeType CodeNameToType(string name)
        {
            switch (name)
            {
                case "def":
                    return CodeType.Type_Sys;
                case "utf8":
                    return CodeType.Type_UTF8;
                case "unicode":
                    return CodeType.Type_Unicode;
            }
            return CodeType.Type_Sys;
        }
        public static Encoding GetCode(CodeType type)
        {
            switch (type)
            {
                case CodeType.Type_Sys:
                    return Encoding.Default;
                case CodeType.Type_UTF8:
                    return Encoding.UTF8;
                case CodeType.Type_Unicode:
                    return Encoding.Unicode;
            }
            return Encoding.Default;
        }
    }
    internal enum LanguageType
    {
        Type_SimplifiedChinese,
        Type_English,
    }
    public static uint LanguageTypeToLanguageId(LanguageType t)
    {
        switch(t)
        {
            case LanguageType.Type_SimplifiedChinese:
                return LanguageDefine.LanguageNameToLanguageId("SimplifiedChinese");
            case LanguageType.Type_English:
                return LanguageDefine.LanguageNameToLanguageId("English");
        }
        return 0;
    }


    //语言列表
    protected Dictionary<uint, LanguageDefine> LanguageDefineList = new Dictionary<uint, LanguageDefine>(32);
    public Dictionary<uint, LanguageDefine>.Enumerator EnumeratorLanguageDefineList
    {
        get
        {
            return LanguageDefineList.GetEnumerator();
        }
    }
    public int LanguageDefineCount { get { return LanguageDefineList.Count; } }
    //要求的默认语言
    public LanguageDefine DefineLanguage;
    //本地设置的语言定义
    public LanguageDefine LoaclLanguage
    {
        get
        {
            byte[] data = ReadSafeFile(UniGameResources.ConnectPath(PersistentDataPath, UniGameResourcesDefine.LocalLanguageFileName));
            if (data == null)
                return SystemLanguage;
            string languageName = Encoding.ASCII.GetString(data);
            uint languageId = FTLibrary.Command.FTUID.StringGetHashCode(languageName);
            LanguageDefine languageDefine;
            if (!LanguageDefineList.TryGetValue(languageId,out languageDefine))
            {
                return new LanguageDefine();
            }
            return languageDefine;
        }
        set
        {
            byte[] data = Encoding.ASCII.GetBytes(value.languageName);
            WriteSafeFile(UniGameResources.ConnectPath(PersistentDataPath, UniGameResourcesDefine.LocalLanguageFileName), data);
        }
    }
    //当前系统语言
    public LanguageDefine SystemLanguage
    {
        get
        {
            Dictionary<uint, LanguageDefine>.Enumerator list = LanguageDefineList.GetEnumerator();
            while(list.MoveNext())
            {
                if (systemLanguage == list.Current.Value.languageSystemId)
                {
                    LanguageDefine ret = list.Current.Value;
                    list.Dispose();
                    return ret;
                }
            }
            list.Dispose();
            return new LanguageDefine();
        }
    }
    //当前使用的语言定义
    public LanguageDefine currentLanguageDefine;
    private void LoadLanguageDefine()
    {
        XmlDocument doc = LoadResource_XmlFile("LanguageDefine.xml");
        if (doc == null)
            throw new Exception("not define language!");
        XmlNode root = doc.SelectSingleNode("LanguageDefine");
        XmlNodeList nodelist = root.SelectNodes("Language");
        //foreach (XmlNode n in nodelist)
        for (int i = 0; i < nodelist.Count; i++)
        {
            XmlNode n = nodelist[i];
            LanguageDefine def = new LanguageDefine();
            def.languageName = n.Attribute("name");
            def.languageId = LanguageDefine.LanguageNameToLanguageId(def.languageName);
            def.languageIndex = Convert.ToInt32(n.Attribute("typeindex"));
            def.languageSystemId = (SystemLanguage)Convert.ToInt32(n.Attribute("SystemId"));
            def.codeType = LanguageDefine.CodeNameToType(n.Attribute("code"));
            XmlNodeList lnlist = n.SelectNodes("LanguageResourcesInventory");
            def.LanguageResourcesInventoryFileList = new List<string>(32);
            //foreach (XmlNode ln in lnlist)
            for (int j = 0; j < lnlist.Count;j++ )
            {
                def.LanguageResourcesInventoryFileList.Add(lnlist[j].Attribute("filename"));
            }
            LanguageDefineList.Add(def.languageId, def);
        }
        //获得默认语言定义
        uint defineLanguageId = (uint)FTLibrary.Command.FTUID.StringGetHashCode(root.Attribute("DefineLanguage"));
        if (!LanguageDefineList.TryGetValue(defineLanguageId, out DefineLanguage))
            throw new Exception("not find define language!");
    }
    //选择当前使用的语言,这是默认函数可重载
    //默认使用LUA脚本控制语言的选择
    public void SelectCurrentLanguage()
    {
        //如果当前语言是中文就用简体中文，否则只用英文
        if (uniLuaResourcesScript == null)
            throw new Exception("not have lua script object!");
        //首先需要建立传输表。把语言定义传到脚本里
        LanguageDefine selLanguage = uniLuaResourcesScript.SelectCurrentLanguage(DefineLanguage, LoaclLanguage, SystemLanguage, LanguageDefineList);
        //没有选到.使用默认的，一般默认的都是英文
        if (selLanguage.languageId == 0)
        {
            currentLanguageDefine = DefineLanguage;
        }
        else
        {
            currentLanguageDefine = selLanguage;
        }
        //保存一次选择的语言
        LoaclLanguage = currentLanguageDefine;
    }
    //重建语言资源列表
    public void LoadLanguageResourcesInventory()
    {
        //开始加载语言定义的资源清单
        for (int i = 0; i < currentLanguageDefine.LanguageResourcesInventoryFileList.Count; i++)
        {
            XmlDocument ldoc = LoadResource_LanguageResourceXmlFile(currentLanguageDefine.LanguageResourcesInventoryFileList[i],
                                    LanguageDefine.GetCode(currentLanguageDefine.codeType));
            if (ldoc == null)
                throw new Exception("load Language xml Inventory file err!");
            BuildLanguageResourcesTable(ldoc);
        }
    }

    public bool FindLanguageDefine(LanguageType t,out LanguageDefine def)
    {
        return FindLanguageDefine(LanguageTypeToLanguageId(t), out def);
    }
    public bool FindLanguageDefine(uint languageId,out LanguageDefine def)
    {
        return LanguageDefineList.TryGetValue(languageId, out def);
    }
    public LanguageDefine GetDefineLanguage() { return DefineLanguage; }
}
