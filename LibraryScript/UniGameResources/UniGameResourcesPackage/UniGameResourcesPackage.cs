using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Resources;
using FTLibrary.Text;
using FTLibrary.XML;
using UnityEngine;
class UniGameResourcesPackage : GameResourcesPackage
{
    internal enum AssetBundleType
    {
        Type_Local      =0,//本地的包
        Type_Net        =1,//网络上的包
    }
    internal enum AssetBundleExistType
    {
        ExistType_ConstraintInstall = 0,  //强制装载,并且装载后不再卸载
        ExistType_ConstraintDownload = 1, //强制下载，要求游戏一启动的时候就需要下载
        ExistType_UseingInstall = 2,      //使用时装载,这是在需要的时候由其他功能进行装载的，使用完可以卸载
    }

    public AssetBundle currentAssetBundle = null;
    public AssetBundleType assetBundleType = AssetBundleType.Type_Local;
    public int assetBundleVersion { get { return packageRealVersion; } }
    public AssetBundleExistType assetBundleExistType
    {
        get
        {
            switch(existType)
            {
                case GameResourcesPackage.ResourcesPackageExistType.Type_ConstraintNew:
                    return AssetBundleExistType.ExistType_ConstraintInstall;
                case GameResourcesPackage.ResourcesPackageExistType.Type_UseNew:
                    return AssetBundleExistType.ExistType_ConstraintDownload;
                case GameResourcesPackage.ResourcesPackageExistType.Type_UnessentialNew:
                    return AssetBundleExistType.ExistType_UseingInstall;
            }
            return AssetBundleExistType.ExistType_UseingInstall;
        }
    }
    public UniGameResourcesPackage(XmlNode node)
        :base()
    {
        packageName = node.Attribute("name");
        packageId = UniGameResources.PackageNameToIdPackage(packageName);
        packagePath = node.Attribute("path");
        packageLocalVersion = 0;
        packageRealVersion = Convert.ToInt32(node.Attribute("version"));
        packageLocalSize = 0;
        packageRealSize = Convert.ToInt64(node.Attribute("size"));
        existType = (ResourcesPackageExistType)Convert.ToInt32(node.Attribute("existtype"));
        assetBundleType = (AssetBundleType)Convert.ToInt32(node.Attribute("type"));
        inventoryFileName = node.Attribute("inventoryFileName");
    }
    protected void ReleaseAssetBundle()
    {
        if (currentAssetBundle != null)
        {
            currentAssetBundle.Unload(false);
            UnityEngine.Object.DestroyObject(currentAssetBundle);
            currentAssetBundle = null;
        }
    }
    //卸载资源包
    public override void ReleaseResourcesPackage()
    {
        ReleaseAssetBundle();
    }
    //释放
    public override void Release()
    {
        ReleaseAssetBundle();
    }
}
