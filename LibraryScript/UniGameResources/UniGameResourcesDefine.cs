using System;
using System.Collections.Generic;
using System.Text;

/*
 * 支持的宏定义:
 * _Develop 开始模式
 * _Release 发布模式
 * _Ciphertext 加密文本
 * 
 * */


class UniGameResourcesDefine
{
    //本地资源包文件数据
    public const string LocalPackageFileName = "GameAssetBundleInventory.dat";
    //网络资源包清单数据文件名
    //public const string NetPackageFileName = "GameAssetBundleInventory.xml";
    //网络缓冲到本地的资源包清单数据文件名
    //public const string BufferNetPackageFileName = "NetGameAssetBundleInventory.xml";
    //本地缓冲的当前语言设定文件
    public const string LocalLanguageFileName = "LocalLanguage.dat";
    
    //资源包在网络上不同平台的路径定义
    public const string HttpAssetBundle_StandaloneWindows_Path = "StandaloneWindows/";
    public const string HttpAssetBundle_Android_Path = "Android/";
    public const string HttpAssetBundle_iPhone_Path = "iPhone/";
    
    //配置文件包名，这个必须默认，在重建整个资源表的时候会使用
    public const string DefineAssetBundleName_Config = "config.unity3d";
    public const string DefineAssetBundleName_LuaScript = "luascript.unity3d";
    public const string DefineAssetBundleName_PublicConfig = "publicconfig.unity3d";

    //系统启动界面定义
    public const string DefineAssetBundleName_GameBoot = "gameboot.unity3d";
    public const string GameBoot_AssetBundle_Path = "GameBootFace.prefab";
    public const string GameBoot_Resource_Path = "TemplateUI/Prefabs/";


    //产品激活码的文件名
    public const string ProduceActivateIdFileName = "LocalProduceActivateId.dat";

    //系统默认资源加载包。这个包不需要下载，产品打包的时候就一起打包
    //case 0x6D97690D://Resources CRC32HashCode
    public const uint DefineSystemAssetBundleId = 0x6D97690D;
    //资源类型定义
    public const string ResourcesTypeName_XmlFile = "XmlFile";
    public const int ResourcesTypeIndex_XmlFile = 0;

    public const string ResourcesTypeName_ResourceFile = "ResourceFile";
    public const int ResourcesTypeIndex_ResourceFile = 1;

    public const string ResourcesTypeName_LanguageResourceFile = "LanguageResourceFile";
    public const int ResourcesTypeIndex_LanguageResourceFile = 2;

    public const string ResourcesTypeName_LuaScript = "LuaScript";
    public const int ResourcesTypeIndex_LuaScript = 3;

    public const string ResourcesTypeName_String = "String";
    public const int ResourcesTypeIndex_String = 4;

    public const string ResourcesTypeName_Prefabs = "Prefabs";
    public const int ResourcesTypeIndex_Prefabs = 5;

    public const string ResourcesTypeName_PublicXmlFile = "PublicXmlFile";
    public const int ResourcesTypeIndex_PublicXmlFile = 6;

    public const string ResourcesTypeName_Texture = "Texture";
    public const int ResourcesTypeIndex_Texture = 7;

    public const string ResourcesTypeName_AudioClip = "AudioClip";
    public const int ResourcesTypeIndex_AudioClip = 8;

    public const string ResourcesTypeName_TimerDefine = "TimerDefine";
    public const int ResourcesTypeIndex_TimerDefine = 9;

    public const string ResourcesTypeName_Material = "Material";
    public const int ResourcesTypeIndex_Material = 10;

    public const string ResourcesTypeName_PathDefine = "PathDefine";
    public const int ResourcesTypeIndex_PathDefine = 11;

    public const string ResourcesTypeName_CarList = "CarList";
    public const int ResourcesTypeIndex_CarList = 12;

   
    //默认的资源定义
    public const string DefineResourcesName_MainResourcesFile = "MainResourcesInventory.xml";
    public const string DefineResourcesPath_MainResourcesFile = "Resources/MainResourcesInventory.xml";

    //系统的包清单文件名
    public const string GameAssetBundleInventoryXmlFilePath = "GameAssetBundleInventory.xml";
    public const string GameAssetBundleInventoryPackageFilePath = "GameAssetBundleInventory.assetbundle";
    //资源LUA脚本名
    public const string ResourcesLuaScriptName = "LuaResourcesScript.lua";

    //产品版本文件名
    public const string ProduceVersionFileName = "ProduceVersion.xml";


    ////资源包后缀定义
    //public const string InTimePackageFileFilter = "unity3d";
    ////及时下载的时候同时允许下载的任务数
    //public const int InTimeDownloadMaxTask = 4;
    //public const int InTimeAddTiveDownloadLevel = 1;

}
