using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Resources;
using UnityEngine;
using FTLibrary.XML;
using FTLibrary.Text;
using FTLibrary.Encrypt;
/************************************************************************/
/* 资源包下载的实现方法如下：
 *由于在开发过程中有需求随时从任何一个场景进入，所以实际下载过程需要放到单独的过程中
 *1，首先所有启动在GameRoot里做环境初始化的时候需要重建资源，这个过程不处理资源的更新，即便资源需要更新。
 *2，在Root场景，也就是启动场景里的场景控制Start函数里首先创建一个资源对象，建立资源包表。然后进行更新
 *3,更新完成后在进行下面的过程。
 *
 * 备注，所有包的装载和卸载过程都是在外部完成的，整个资源系统已经假设包是存在的
 * 
*/
/************************************************************************/
partial class UniGameResources : GameResources
{
    public UniGameResources()
        :base()
    {
        UniGameResources.currentUniGameResources = this;
    }
    //public override GameResourcesPackage AllocGameResourcesPackage()
    //{
    //    return new UniGameResourcesPackage();
    //}
    //LUA脚本读取器
    protected UniResourcesLuaScriptLoader uniResourcesLuaScriptLoader = null;
    //资源的LUA脚本控制对象
    protected UniLuaResourcesScript uniLuaResourcesScript = null;
    //xml文件解码器
    public FTEncipher xmlFileEncipher = null;
    public FTEncipher luaScriptFileEncipher = null;
    //xml文件读取器
    private FTLibrary.XML.XmlReaderSecurity xmlReaderSecurity = null;
    private FTLibrary.XML.XmlPublicReaderSecurity xmlPublicReaderSecurity = null;
    //lua脚本读取器
    private UniGameResourcesPackage luaScriptPackage;
    protected FTLibrary.XML.LuaScriptReaderSecurity luaScriptReaderSecurity = null;
    //当前系统版本信息
    public FTLibrary.Produce.ProduceVersionInformation produceVersion = new FTLibrary.Produce.ProduceVersionInformation();
    //是否支持及时下载模式
    public void Initialization()
    {
        //移动资源包模式需要加载文件解密器
        //移动模式必须提供资源包列表
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            xmlFileEncipher = UniGameResources.AllocXmlFileEncipher();
            luaScriptFileEncipher = UniGameResources.AllocLuaScriptFileEncipher();
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            xmlReaderSecurity = new FTLibrary.XML.XmlReaderSecurity();
            xmlReaderSecurity.Initialization(UniGameResources.ConnectPath(UniGameResources.DataPath, UniGameResourcesDefine.DefineAssetBundleName_Config, "\\"));
            xmlPublicReaderSecurity = new FTLibrary.XML.XmlPublicReaderSecurity();
            luaScriptReaderSecurity = new FTLibrary.XML.LuaScriptReaderSecurity();
            luaScriptReaderSecurity.Initialization(UniGameResources.ConnectPath(UniGameResources.DataPath, UniGameResourcesDefine.DefineAssetBundleName_LuaScript, "\\"));

            //需要载入包清单来构建包表
            //这里虚构一份资源进行加载
            GameResourcesNode data = new GameResourcesNode();
            data.packageName = UniGameResourcesDefine.DefineAssetBundleName_Config;
            data.path = UniGameResourcesDefine.GameAssetBundleInventoryXmlFilePath;
            //载入系统包清单
            UniGameResources.BuildSystemResourcesPackageTable(LoadResource_XmlDocument(ref data,Encoding.Default));
        }

        //定于到资源对象的表内以方便所有资源进行索引
        Dictionary<uint, UniGameResourcesPackage>.Enumerator list = UniGameResources.systemResourcesPackageTable.GetEnumerator();
        while (list.MoveNext())
        {
            resourcesPackageTable.Add(list.Current.Value.packageId, list.Current.Value);
        }
        list.Dispose();


        //开始重建资源表
        //注册所有资源类型
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_XmlFile, UniGameResourcesDefine.ResourcesTypeIndex_XmlFile);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_ResourceFile, UniGameResourcesDefine.ResourcesTypeIndex_ResourceFile);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_LanguageResourceFile, UniGameResourcesDefine.ResourcesTypeIndex_LanguageResourceFile);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_LuaScript, UniGameResourcesDefine.ResourcesTypeIndex_LuaScript);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_String, UniGameResourcesDefine.ResourcesTypeIndex_String);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_Prefabs, UniGameResourcesDefine.ResourcesTypeIndex_Prefabs);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_PublicXmlFile, UniGameResourcesDefine.ResourcesTypeIndex_PublicXmlFile);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_Texture, UniGameResourcesDefine.ResourcesTypeIndex_Texture);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_AudioClip, UniGameResourcesDefine.ResourcesTypeIndex_AudioClip);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_TimerDefine, UniGameResourcesDefine.ResourcesTypeIndex_TimerDefine);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_Material, UniGameResourcesDefine.ResourcesTypeIndex_Material);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_PathDefine, UniGameResourcesDefine.ResourcesTypeIndex_PathDefine);
        RegisterResourcesType(UniGameResourcesDefine.ResourcesTypeName_CarList, UniGameResourcesDefine.ResourcesTypeIndex_CarList);
        
        //一些索引资源需要手动注册以方便读取
        uint configPackageId = 0;
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            GameResourcesPackage configPackage = LockPackage(UniGameResourcesDefine.DefineAssetBundleName_Config);
            configPackageId = configPackage.packageId;
            luaScriptPackage = LockPackage(UniGameResourcesDefine.DefineAssetBundleName_LuaScript) as UniGameResourcesPackage;
            LockPackage(UniGameResourcesDefine.DefineAssetBundleName_PublicConfig);
            //这里锁定就不释放了，永远驻留内存
        }
        //注册LUA脚本读取器为从资源中加载脚本
        uniResourcesLuaScriptLoader = new UniResourcesLuaScriptLoader();
        //植入主清单文件
        AddResourcesToTable(UniGameResourcesDefine.ResourcesTypeIndex_XmlFile,
                                UniGameResourcesDefine.DefineResourcesName_MainResourcesFile,
                                configPackageId,
                                UniGameResourcesDefine.DefineAssetBundleName_Config,
                                UniGameResourcesDefine.DefineResourcesPath_MainResourcesFile, -1);
        //加载这个主清单文件
        XmlDocument mainXmlDoc = LoadResource_XmlFile(UniGameResourcesDefine.DefineResourcesName_MainResourcesFile);
        if (mainXmlDoc == null)
            throw new Exception("load main xml Inventory file err!");
        //加载这个资源清单
        BuildResourcesTable(mainXmlDoc);
        //加载其他资源清单
        Dictionary<uint, GameResourcesNode>.Enumerator resourceFileList = GetResourceEnumerator(UniGameResourcesDefine.ResourcesTypeIndex_ResourceFile);
        while(resourceFileList.MoveNext())
        {
            XmlDocument doc = LoadResource_ResourceXmlFile(resourceFileList.Current.Value.name);
            if (doc == null)
                throw new Exception("load xml Inventory file err!");
            //加载这份清单
            BuildResourcesTable(doc);
        }
        resourceFileList.Dispose();
        //资源清单已经加载完成了
        //加载语言定义
        LoadLanguageDefine();
        //外部定义模式不在这里加载语言定义
        if (UniGameResources.gameResourcesWorkModeTwo != GameResourcesWorkModeTwo.Mode_OutGameOptions)
        {
            //加载资源模块使用的LUA脚本
            uniLuaResourcesScript = new UniLuaResourcesScript();
            //选择当前使用的语言
            SelectCurrentLanguage();
            //加载语言资源清单
            LoadLanguageResourcesInventory();
        }
        //加载当前版本信息
        produceVersion.Initialization(LoadResource_PublicXmlFile(UniGameResourcesDefine.ProduceVersionFileName));
        //加载资源卸载器
        UniGameResources.AllocUnloadUnusedAssetsGameObject();
        //加载资源下载器
        UniGameResources.AllocResourcesDownLoadWorker();
    }
    //加载Xml文件
    private XmlDocument LoadResource_XmlDocument(ref GameResourcesNode data,Encoding code)
    {
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
#if _Develop
                XmlDocument doc = new XmlDocument();
                doc.Load(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path));
                return doc;
#else
                return xmlReaderSecurity.LoadXml(data.path);
#endif//_Develop
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            UniGameResourcesPackage package = (UniGameResourcesPackage)data.package;
            package.LockPackage();
#if UNITY_4_3 || UNITY_4_6
            TextAsset textAsset = package.currentAssetBundle.Load(data.path, typeof(TextAsset)) as TextAsset;
#else
            TextAsset textAsset = package.currentAssetBundle.LoadAsset(data.path, typeof(TextAsset)) as TextAsset;
#endif
            XmlDocument doc = null;
            try
            {
                byte[] xmldata = textAsset.bytes;
                xmldata = Convert.FromBase64String(Encoding.ASCII.GetString(xmldata));
                //解密数据
#if _SupportDeviceVerify
                xmldata = xmlFileEncipher.FileDecrypt(xmldata,true);
#else
                xmldata = xmlFileEncipher.FileDecrypt(xmldata);
#endif //_SupportDeviceVerify
                //MemoryStream s = new MemoryStream(xmldata);
                doc = new XmlDocument();
                doc.LoadXml(code.GetString(xmldata));
                //s.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                doc = null;
            }
            UniGameResources.ReleaseOneAssets(textAsset);
            package.UnLockPackage();
            return doc;
        }
        return null;
    }
    private XmlDocument LoadResource_PublicXmlDocument(ref GameResourcesNode data, Encoding code)
    {
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
#if _Develop
                XmlDocument doc = new XmlDocument();
                doc.Load(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path));
                return doc;
#else
                return xmlPublicReaderSecurity.LoadXml(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path));
#endif//_Develop
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            UniGameResourcesPackage package = (UniGameResourcesPackage)data.package;
            package.LockPackage();
#if UNITY_4_3 || UNITY_4_6
            TextAsset textAsset = package.currentAssetBundle.Load(data.path, typeof(TextAsset)) as TextAsset;
#else
            TextAsset textAsset = package.currentAssetBundle.LoadAsset(data.path, typeof(TextAsset)) as TextAsset;
#endif
            XmlDocument doc = null;
            try
            {
                byte[] xmldata = textAsset.bytes;
                xmldata = Convert.FromBase64String(Encoding.ASCII.GetString(xmldata));
                //解密数据
#if _SupportDeviceVerify
                xmldata = xmlFileEncipher.FileDecrypt(xmldata,true);
#else
                xmldata = xmlFileEncipher.FileDecrypt(xmldata);
#endif //_SupportDeviceVerify
                //MemoryStream s = new MemoryStream(xmldata);
                doc = new XmlDocument();
                doc.LoadXml(code.GetString(xmldata));
                //s.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                doc = null;
            }
            UniGameResources.ReleaseOneAssets(textAsset);
            package.UnLockPackage();
            return doc;
        }
        return null;
    }
    public XmlDocument LoadResource_XmlFile(string name)
    {
        return LoadResource_XmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadResource_XmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_XmlFile, name, ref data))
            return null;
        return LoadResource_XmlDocument(ref data, code);
    }
    public XmlDocument LoadResource_PublicXmlFile(string name)
    {
        return LoadResource_PublicXmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadResource_PublicXmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_PublicXmlFile, name, ref data))
            return null;
        return LoadResource_PublicXmlDocument(ref data, code);
    }
    public XmlDocument LoadLanguageResource_XmlFile(string name)
    {
        return LoadLanguageResource_XmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadLanguageResource_XmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_XmlFile, name, ref data))
            return null;
        return LoadResource_XmlDocument(ref data, code);
    }
    public XmlDocument LoadLanguageResource_PublicXmlFile(string name)
    {
        return LoadLanguageResource_PublicXmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadLanguageResource_PublicXmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_PublicXmlFile, name, ref data))
            return null;
        return LoadResource_PublicXmlDocument(ref data, code);
    }

    public XmlDocument LoadResource_ResourceXmlFile(string name)
    {
        return LoadResource_ResourceXmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadResource_ResourceXmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_ResourceFile, name, ref data))
            return null;
        return LoadResource_XmlDocument(ref data, code);
    }
    public XmlDocument LoadResource_LanguageResourceXmlFile(string name)
    {
        return LoadResource_LanguageResourceXmlFile(name, Encoding.Default);
    }
    public XmlDocument LoadResource_LanguageResourceXmlFile(string name, Encoding code)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_LanguageResourceFile, name, ref data))
            return null;
        return LoadResource_XmlDocument(ref data, code);
    }


    private byte[] LoadResource_ByteFile(ref GameResourcesNode data)
    {
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
                string path = UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path);
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096);
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0,buffer.Length);
                file.Close();
                return buffer;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            UniGameResourcesPackage package = (UniGameResourcesPackage)data.package;
            package.LockPackage();
#if UNITY_4_3 || UNITY_4_6
            TextAsset textAsset = package.currentAssetBundle.Load(data.path, typeof(TextAsset)) as TextAsset;
#else
            TextAsset textAsset = package.currentAssetBundle.LoadAsset(data.path, typeof(TextAsset)) as TextAsset;
#endif
            byte[] buffer = textAsset.bytes;
            buffer = Convert.FromBase64String(Encoding.ASCII.GetString(buffer));
            //解密数据
#if _SupportDeviceVerify
            buffer = xmlFileEncipher.FileDecrypt(buffer,true);
#else
            buffer = xmlFileEncipher.FileDecrypt(buffer);
#endif //_SupportDeviceVerify
            UniGameResources.ReleaseOneAssets(textAsset);
            package.UnLockPackage();
            return buffer;
        }
        return null;

    }
    public byte[] LoadResource_ByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_XmlFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }
    public byte[] LoadResource_PublicByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_PublicXmlFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }
    public byte[] LoadLanguageResource_ByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_XmlFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }
    public byte[] LoadLanguageResource_PublicByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_PublicXmlFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }

    public byte[] LoadResource_ResourceByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_ResourceFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }
    public byte[] LoadResource_LanguageResourceByteFile(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_LanguageResourceFile, name, ref data))
            return null;
        return LoadResource_ByteFile(ref data);
    }



    public string LoadResource_TextFile(string name,Encoding code)
    {
        byte[] buffer = LoadResource_ByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }
    public string LoadResource_PublicTextFile(string name, Encoding code)
    {
        byte[] buffer = LoadResource_PublicByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }
    public string LoadLanguageResource_TextFile(string name, Encoding code)
    {
        byte[] buffer = LoadLanguageResource_ByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }
    public string LoadLanguageResource_PublicTextFile(string name, Encoding code)
    {
        byte[] buffer = LoadLanguageResource_PublicByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }

    public string LoadResource_ResourceTextFile(string name, Encoding code)
    {
        byte[] buffer = LoadResource_ResourceByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }
    public string LoadResource_LanguageResourceTextFile(string name, Encoding code)
    {
        byte[] buffer = LoadResource_LanguageResourceByteFile(name);
        if (buffer == null)
            return "";
        return code.GetString(buffer);
    }






    //加载lua脚本
    public string LoadResource_GetLuaScriptFilePath(string filename)
    {
        //这个模式需要从资源包内载入文件路径需要给从资源包开始定义的路径
        return UniGameResources.ConnectPath("LuaRoot/", filename);
    }
    public MemoryStream LoadResource_LuaScript(string filePath)
    {
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
#if _Develop
                FileStream file = new FileStream(UniGameResources.ConnectPath(UniGameResources.DataPath, UniGameResourcesDefine.DefineAssetBundleName_LuaScript, "/", filePath), FileMode.Open, FileAccess.Read, FileShare.Read);
                file.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, buffer.Length);
                file.Close();
                return new MemoryStream(buffer);
#else
                return luaScriptReaderSecurity.Load(filePath);
#endif//_Develop
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            if (luaScriptPackage == null)
                throw new Exception("cant find lua script package!");
            luaScriptPackage.LockPackage();
#if UNITY_4_3 || UNITY_4_6
            TextAsset textAsset = luaScriptPackage.currentAssetBundle.Load(filePath, typeof(TextAsset)) as TextAsset;
#else
            TextAsset textAsset = luaScriptPackage.currentAssetBundle.LoadAsset(filePath, typeof(TextAsset)) as TextAsset;
#endif
            MemoryStream ret = null;
            try
            {
                byte[] luadata = textAsset.bytes;
                luadata = Convert.FromBase64String(Encoding.ASCII.GetString(luadata));
                //解密数据
#if _SupportDeviceVerify
                luadata = luaScriptFileEncipher.FileDecrypt(luadata,true);
#else
                luadata = luaScriptFileEncipher.FileDecrypt(luadata);
#endif //_SupportDeviceVerify
                ret = new MemoryStream(luadata);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                ret = null;
            }
            UniGameResources.ReleaseOneAssets(textAsset);
            luaScriptPackage.UnLockPackage();
            return ret;
        }
        return null;
    }
    public bool LoadResource_IsHaveLuaScript(string filePath)
    {
        if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
#if _Develop
                return File.Exists(UniGameResources.ConnectPath(UniGameResources.DataPath, UniGameResourcesDefine.DefineAssetBundleName_LuaScript, "/", filePath));
#else
                MemoryStream s = luaScriptReaderSecurity.Load(filePath);
                bool ret = (s != null);
                s.Close();
                return ret;
#endif//_Develop
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            if (luaScriptPackage == null)
                throw new Exception("cant find lua script package!");
            luaScriptPackage.LockPackage();
            bool ret = luaScriptPackage.currentAssetBundle.Contains(filePath);
            luaScriptPackage.UnLockPackage();
            return ret;
        }
        return false;
    }


    //通用加载函数
    private UnityEngine.Object PrivateLoadResource_Resource(ref GameResourcesNode data, Type type, bool isInstantiate)
    {
        try
        {
            if (isInstantiate)
            {
                return GameObject.Instantiate(Resources.Load(data.path, type));
            }
            else
            {
                return Resources.Load(data.path, type);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        return null;
    }
    private UnityEngine.Object PrivateLoadResource(ref GameResourcesNode data, Type type, bool isInstantiate)
    {
        //如果是系统资源包就换个函数加载
        if (data.packageId == UniGameResourcesDefine.DefineSystemAssetBundleId)
        {
            return PrivateLoadResource_Resource(ref data, type, isInstantiate);
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_AloneSecurity)
        {
            try
            {
                if (isInstantiate)
                {
#if UNITY_4_3 || UNITY_4_6
                    return GameObject.Instantiate(Resources.LoadAssetAtPath(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path), type));
#else
                    return GameObject.Instantiate(Resources.Load(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path), type));
#endif
                }
                else
                {
#if UNITY_4_3 || UNITY_4_6
                    return Resources.LoadAssetAtPath(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path), type);
#else
                    return Resources.Load(UniGameResources.ConnectPath(UniGameResources.DataPath, data.packageName, "/", data.path), type);
#endif
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }
        else if (UniGameResources.gameResourcesWorkMode == GameResourcesWorkMode.Mode_Mobile)
        {
            UnityEngine.Object ret = null;
            UniGameResourcesPackage package = (UniGameResourcesPackage)data.package;
            package.LockPackage();
            try
            {
                if (isInstantiate)
                {
#if UNITY_4_3 || UNITY_4_6
                    ret = GameObject.Instantiate(package.currentAssetBundle.Load(data.path, type));
#else
                    ret = GameObject.Instantiate(package.currentAssetBundle.LoadAsset(data.path, type));
#endif
                }
                else
                {
#if UNITY_4_3 || UNITY_4_6
                    ret = package.currentAssetBundle.Load(data.path, type);
#else
                    ret = package.currentAssetBundle.LoadAsset(data.path, type);
#endif
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            package.UnLockPackage();
            return ret;
        }
        return null;
    }
    //加载纹理
    public Texture LoadResource_Texture(string name, Type type)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_Texture, name, ref data))
            return null;
        return PrivateLoadResource(ref data, type, false) as Texture;
    }
    public Texture LoadLanguageResource_Texture(string name, Type type)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_Texture, name, ref data))
            return null;
        return PrivateLoadResource(ref data, type, false) as Texture;
    }

    //加载音频
    public AudioClip LoadResource_AudioClip(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_AudioClip, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(AudioClip), false) as AudioClip;
    }
    public AudioClip LoadLanguageResource_AudioClip(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_AudioClip, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(AudioClip), false) as AudioClip;
    }
    //获取材质资源
    public Material LoadResource_Material(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_Material, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(Material), false) as Material;
    }
    public Material LoadLanguageResource_Material(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_Material, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(Material), false) as Material;
    }
    //加载预置
    public GameObject LoadResource_Prefabs(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_Prefabs, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(GameObject), true) as GameObject;
    }
    public GameObject LoadLanguageResource_Prefabs(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_Prefabs, name, ref data))
            return null;
        return PrivateLoadResource(ref data, typeof(GameObject), true) as GameObject;
    }
    //归并化预置
    public static GameObject NormalizePrefabs(GameObject prefabs,Transform parent)
    {
        if (parent != null)
        {
            prefabs.transform.parent = parent;
        }
        prefabs.transform.localPosition = Vector3.zero;
        prefabs.transform.localRotation = Quaternion.identity;
        return prefabs;
    }
    public static Component NormalizePrefabs(GameObject prefabs,Transform parent,Type type)
    {
        return NormalizePrefabs(prefabs, parent).GetComponent(type);
    }

    //加载字符串
    public string LoadResource_Text(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_String, name, ref data))
            return "";
        return data.path;
    }
    public string LoadLanguageResource_Text(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_String, name, ref data))
            return "";
        return data.path;
    }
    
    
    //获取时间资源
    public float LoadResource_Time(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_TimerDefine, name, ref data))
            return 0.0f;
        return Convert.ToSingle(data.path);
    }
    public float LoadLanguageResource_Time(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_TimerDefine, name, ref data))
            return 0.0f;
        return Convert.ToSingle(data.path);
    }
    //获取路径资源
    public string LoadResource_PathDefine(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindResources(UniGameResourcesDefine.ResourcesTypeIndex_PathDefine, name, ref data))
            return "";
        return data.path;
    }
    public string LoadLanguageResource_PathDefine(string name)
    {
        GameResourcesNode data = new GameResourcesNode();
        if (!FindLanguageResources(UniGameResourcesDefine.ResourcesTypeIndex_PathDefine, name, ref data))
            return "";
        return data.path;
    }
}
