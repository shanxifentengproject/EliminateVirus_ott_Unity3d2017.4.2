using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Resources;
using FTLibrary.Command;
using FTLibrary.Text;
using FTLibrary.XML;
using UnityEngine;
using FTLibrary.Encrypt;


partial class UniGameResources : GameResources
{
    //静态资源管理对象
    public static UniGameResources currentUniGameResources = null;
    //当前运行平台
    public static RuntimePlatform platform { get { return Application.platform; } }
    //是否编辑器
    public static bool isEditor { get { return Application.isEditor; } }
    //是否播放器
    public static bool isPlaying { get { return Application.isPlaying; } }
    //当前系统语言
    public static SystemLanguage systemLanguage { get { return Application.systemLanguage; } }
    //版本
    public static string unityVersion { get { return Application.unityVersion; } }
    //资源外部定义信息
    public static UniGameResourcesSupport uniGameResourcesSupport = new UniGameResourcesSupport();


    //包是本地或者网络是由外部加载器决定的，本系统只根据资源定义的包明来判断从哪里读取
    internal enum GameResourcesWorkMode
    {
        Mode_AloneSecurity, //独立模式，为街机产品准备的模式，
                            //.xml等文件是以单文件的形式存在，并且是通过dll读取的。
                            //其他资源资源可以定义在包内或者Resources目录下

        Mode_Mobile,        //移动模式，这是为移动产品准备的模式，
                            //xml等文件需要以包的形式存在,其他资源资源可以定义在包内或者Resources目录下。
                            //这种模式必须提供一个包清单文件的包，这个包的主资源就是包清单文件
    }
    //资源模块的第二种工作模式
    internal enum GameResourcesWorkModeTwo
    {
        Mode_NotEffect,         //不起作用
        Mode_OutGameOptions,    //外部的游戏定义模式，对于语言定义，使用的语言在外部设定，资源模块初始化的时候先不加载语言清单，由外部加载
    }
    //当前设定
    private static GameResourcesWorkMode m_GameResourcesWorkMode = GameResourcesWorkMode.Mode_AloneSecurity;
    private static GameResourcesWorkModeTwo m_GameResourcesWorkModeTwo = GameResourcesWorkModeTwo.Mode_OutGameOptions;
    //数据路径
    private static string m_DataPath;
    //持久数据路径
    private static string m_PersistentDataPath;
    //临时数据路径
    private static string m_TemporaryCachePath;
    //本地资源包路径
    private static string m_StreamingAssetsPath;

    //当前工作模式设定
    public static GameResourcesWorkMode gameResourcesWorkMode
    {
        get
        {
            return m_GameResourcesWorkMode;
        }
        set
        {
            m_GameResourcesWorkMode = value;
            m_DataPath = FTLibrary.Text.IStringPath.ConnectPath(Application.dataPath, "/");
            switch(m_GameResourcesWorkMode)
            {
                case GameResourcesWorkMode.Mode_AloneSecurity:
                    {
                        m_PersistentDataPath = FTLibrary.Text.IStringPath.ConnectPath(Application.dataPath, "/../Data/");
                        FTLibrary.Command.IFile.PWritePath(m_PersistentDataPath);
                        m_TemporaryCachePath = FTLibrary.Text.IStringPath.ConnectPath(Application.dataPath, "/../Data/temp/");
                        FTLibrary.Command.IFile.PWritePath(m_TemporaryCachePath);
                    }
                    break;
                case GameResourcesWorkMode.Mode_Mobile:
                    {
                        if (isEditor || platform == RuntimePlatform.WindowsPlayer)
                        {
                            m_PersistentDataPath = FTLibrary.Text.IStringPath.ConnectPath(Application.dataPath, "/../Data/");
                            FTLibrary.Command.IFile.PWritePath(m_PersistentDataPath);
                            m_TemporaryCachePath = FTLibrary.Text.IStringPath.ConnectPath(Application.dataPath, "/../Data/temp/");
                            FTLibrary.Command.IFile.PWritePath(m_TemporaryCachePath);
                        }
                        else
                        {
                            m_PersistentDataPath = FTLibrary.Text.IStringPath.ConnectPath(Application.persistentDataPath, "/");
                            m_TemporaryCachePath = FTLibrary.Text.IStringPath.ConnectPath(Application.temporaryCachePath, "/");
                        }
                        if (isEditor)
                        {
                            //if (platform == RuntimePlatform.WindowsEditor)
                            //{
                            //    m_StreamingAssetsPath = FTLibrary.Text.IStringPath.ConnectPath(Application.streamingAssetsPath, "/");
                            //}
                            //else
                            //{
                                m_StreamingAssetsPath = FTLibrary.Text.IStringPath.ConnectPath("file://", Application.streamingAssetsPath, "/");
                            //}
                        }
                        else
                        {
                            m_StreamingAssetsPath = FTLibrary.Text.IStringPath.ConnectPath(Application.streamingAssetsPath, "/");
                        }
                    }
                    break;
            }
        }
    }
    public static GameResourcesWorkModeTwo gameResourcesWorkModeTwo { get { return m_GameResourcesWorkModeTwo; } set { m_GameResourcesWorkModeTwo = value; } }
    //数据路径
    public static string DataPath { get { return m_DataPath; } }
    //持久数据路径
    public static string PersistentDataPath { get { return m_PersistentDataPath; } }
    //临时数据路径
    public static string TemporaryCachePath { get { return m_TemporaryCachePath; } }
    //本地资源包路径
    public static string StreamingAssetsPath { get { return m_StreamingAssetsPath; } }
    //包下载访问路径
    public static string downloadUrlPath
    {
        get 
        { 
            //如果是在编辑器内则使用本地的包做加载
            //if (isEditor)
            //{
            //    return StreamingAssetsPath;
            //}
            if (uniGameResourcesSupport.DownloadUrlPath == UniGameResourcesSupport.LoaclDownloadUrlPathMode)
            {
                return StreamingAssetsPath;
            }
            else
            {
                switch(platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                        return FTLibrary.Text.IStringPath.ConnectPath(uniGameResourcesSupport.DownloadUrlPath, UniGameResourcesDefine.HttpAssetBundle_StandaloneWindows_Path);
                    case RuntimePlatform.Android:
                        return FTLibrary.Text.IStringPath.ConnectPath(uniGameResourcesSupport.DownloadUrlPath, UniGameResourcesDefine.HttpAssetBundle_Android_Path);
                    case RuntimePlatform.IPhonePlayer:
                        return FTLibrary.Text.IStringPath.ConnectPath(uniGameResourcesSupport.DownloadUrlPath, UniGameResourcesDefine.HttpAssetBundle_iPhone_Path);
                }
                return uniGameResourcesSupport.DownloadUrlPath; 
            }
            
        }
    }
    //是否本地下载路径
    public static bool IsLocalDownloadUrlPath
    {
        get
        {
            ////如果是在编辑器内则使用本地的包做加载
            //if (isEditor)
            //{
            //    return true;
            //}
            if (uniGameResourcesSupport.DownloadUrlPath == UniGameResourcesSupport.LoaclDownloadUrlPathMode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    //是否强制更新所有资源
    public static bool ForceUpdateAllGameAssetBundle = false;
    //资源清单资源包的版本号，资源清单资源包更新后，其他资源更新的版本号才会被改变
    public static int version_GameAssetBundleInventoryPackageFile = 0;



    //打通路径
    public static void PWritePath(string path) { IFile.PWritePath(path); }
    //连接路径
    public static string ConnectPath(params string[] part)
    {
        return IStringPath.ConnectPath(part);
    }
    //删除文件
    public static void RemoveFile(string path){IFile.RemoveFile(path);}
    //是否存在文件
    public static bool IsHaveFile(string path) { return IFile.IsHaveFile(path); }
    //读取文件
    public static byte[] ReadFile(string path)
    {
        try
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] buffer = new byte[file.Length];
            file.Seek(0, SeekOrigin.Begin);
            file.Read(buffer, 0, buffer.Length);
            file.Close();
            return buffer;
        }
        catch //(System.Exception ex)
        {
            return null;
        }
        
    }
    //保存文件
    public static void WriteFile(string path,byte[] data)
    {
        try
        {
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write);
            file.Seek(0, SeekOrigin.Begin);
            file.Write(data, 0, data.Length);
            file.Close();
        }
        catch //(System.Exception ex)
        {
        	
        }
        
    }
    //安全文件操作
    public static void RemoveSafeFile(string path) { ISafeFile.RemoveFile(path); }
    public static byte[] ReadSafeFile(string path) { return ISafeFile.ReadFile(path); }
    public static void WriteSafeFile(string path, byte[] data) { ISafeFile.WriteFile(path,data); }

    //当前系统的资源包表
    //这个表是在外部根据不同的使用方式建立的
    public static Dictionary<uint, UniGameResourcesPackage> systemResourcesPackageTable = new Dictionary<uint, UniGameResourcesPackage>(128);

    public static UniProduceInformation produceInformation = new UniProduceInformation();
    //当前支持的程序版本
    //<!--支持直接升级的程序版本，如果程序不是这个版本则无法直接升级，应该重新安装最新版本的程序-->
    public static string SupportProgramVersion;
    //产品版本
    public static string ProduceVersion;
    //给定一个xml文件用于建立资源包表
    public static void BuildSystemResourcesPackageTable(XmlDocument doc)
    {
        XmlNode root = doc.SelectSingleNode("GameResourcesPackageList");
        XmlNodeList nodelist = root.SelectNodes("Package");
        for (int i = 0; i < nodelist.Count;i++ )
        {
            UniGameResourcesPackage package = new UniGameResourcesPackage(nodelist[i]);
            systemResourcesPackageTable.Add(package.packageId, package);
        }

        produceInformation.Initialization(root.SelectSingleNode("ProduceInformation"));
    }
    public static UniGameResourcesPackage FindSystemResourcesPackageTable(string name)
    {
        uint packageId = UniGameResources.PackageNameToIdPackage(name);
        UniGameResourcesPackage ret;
        if (!systemResourcesPackageTable.TryGetValue(packageId, out ret))
            return ret;
        return ret;
    }
    //存储一次当前资源包的版本信息，做为下一次版本对比之用
    private struct SystemResourcesPackageVerion
    {
        public uint PackageId;
        public int PackageVersion;
    }
    private static void SaveResourcesPackageVersion(List<SystemResourcesPackageVerion> versionList,string fileName)
    {
        MemoryStream s = new MemoryStream(1024);
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        writer.Write(versionList.Count);
        for (int i = 0; i < versionList.Count; i++)
        {
            writer.Write(versionList[i].PackageId);
            writer.Write(versionList[i].PackageVersion);
        }
        writer.Flush();
        byte[] data = s.ToArray();
        writer.Close();

        string path = FTLibrary.Text.IStringPath.ConnectPath(PersistentDataPath, fileName);
        FTLibrary.Command.ISafeFile.WriteFile(path, data);
    }
    private static void ReadResourcesPackageVersion(List<SystemResourcesPackageVerion> versionList,string fileName)
    {
        string path = FTLibrary.Text.IStringPath.ConnectPath(PersistentDataPath, fileName);
        byte[] data = FTLibrary.Command.ISafeFile.ReadFile(path);
        if (data == null)
            return;
        BinaryReader reader = new BinaryReader(new MemoryStream(data));
        int Count = reader.ReadInt32();
        for (int i = 0; i < Count;i++ )
        {
            SystemResourcesPackageVerion version = new SystemResourcesPackageVerion();
            version.PackageId = reader.ReadUInt32();
            version.PackageVersion = reader.ReadInt32();
            versionList.Add(version);
        }
        reader.Close();
    }

    public static void SaveSystemResourcesPackageVersion()
    {
        try
        {
            List<SystemResourcesPackageVerion> versionList = new List<SystemResourcesPackageVerion>(16);
            Dictionary<uint, UniGameResourcesPackage>.Enumerator list = systemResourcesPackageTable.GetEnumerator();
            while (list.MoveNext())
            {
                SystemResourcesPackageVerion version = new SystemResourcesPackageVerion();
                version.PackageId = list.Current.Value.packageId;
                version.PackageVersion = list.Current.Value.assetBundleVersion;
                versionList.Add(version);
            }
            list.Dispose();
            SaveResourcesPackageVersion(versionList, UniGameResourcesDefine.LocalPackageFileName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }  
    }
    //根据本地存储的资源包版本和当前的做对比产生需要下载的资源包列表
    public static List<UniGameResourcesPackage> ComparisonSystemResourcesPackageVersion()
    {
        try
        {
            List<SystemResourcesPackageVerion> versionList = new List<SystemResourcesPackageVerion>(16);
            ReadResourcesPackageVersion(versionList, UniGameResourcesDefine.LocalPackageFileName);
            //在当前包表内查找所有必需下载并且版本是新的
            List<UniGameResourcesPackage> ret = new List<UniGameResourcesPackage>(8);
            Dictionary<uint, UniGameResourcesPackage>.Enumerator list = systemResourcesPackageTable.GetEnumerator();
            while (list.MoveNext())
            {
                if (list.Current.Value.assetBundleExistType == UniGameResourcesPackage.AssetBundleExistType.ExistType_ConstraintDownload)
                {
                    bool IsNew=true;
                    for (int i = 0; i < versionList.Count;i++ )
                    {
                        if (versionList[i].PackageId == list.Current.Value.packageId)
                        {
                            if (versionList[i].PackageVersion >= list.Current.Value.assetBundleVersion)
                            {
                                IsNew = false;
                            }
                            break;
                        }
                    }
                    if (IsNew)
                    {
                        ret.Add(list.Current.Value);
                    }
                }
            }
            list.Dispose();
            return ret;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
            return new List<UniGameResourcesPackage>();
        }
    }

    


    public static UniGameResourcesDownLoadWorker resourcesDownLoadWorker = null;
    public static void AllocResourcesDownLoadWorker()
    {
        if (resourcesDownLoadWorker != null)
            return;
        GameObject go = new GameObject("ResourcesDownLoadWorker", typeof(UniGameResourcesDownLoadWorker));
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        UnityEngine.Object.DontDestroyOnLoad(go);
        resourcesDownLoadWorker = go.GetComponent<UniGameResourcesDownLoadWorker>();
    }


    public static FTEncipher AllocXmlFileEncipher()
    {
        byte[] rgbKey;
        byte[] rgbIV;
        FTEncipher ret = new FTEncipher();
        ret.CreatePublicKeyRSAEncryptProvider(UniGameResources.uniGameResourcesSupport.XmlRsaPublicKey);
        UniGameResources.uniGameResourcesSupport.ResolveXmlDesKey(out rgbKey, out rgbIV);
        ret.CreateDecryptProvider(rgbKey, rgbIV);
        return ret;
    }
    public static FTEncipher AllocLuaScriptFileEncipher()
    {
        byte[] rgbKey;
        byte[] rgbIV;
        FTEncipher ret = new FTEncipher();
        ret.CreatePublicKeyRSAEncryptProvider(UniGameResources.uniGameResourcesSupport.LuaRsaPublicKey);
        UniGameResources.uniGameResourcesSupport.ResolveLuaDesKey(out rgbKey, out rgbIV);
        ret.CreateDecryptProvider(rgbKey, rgbIV);
        return ret;
    }

    static UniGameResources()
    {
        switch(platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                gameResourcesWorkMode = GameResourcesWorkMode.Mode_AloneSecurity;
                gameResourcesWorkModeTwo = GameResourcesWorkModeTwo.Mode_OutGameOptions;
                break;
            case RuntimePlatform.Android:
            case RuntimePlatform.OSXPlayer:
                gameResourcesWorkMode = GameResourcesWorkMode.Mode_Mobile;
                gameResourcesWorkModeTwo = GameResourcesWorkModeTwo.Mode_OutGameOptions;
                break;
        }
        
    }
}

