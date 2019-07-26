using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using FTLibrary.XML;
using FTLibrary.Encrypt;
using System.IO;
[AddComponentMenu("UniGameResources/UniGameResourceLoad/UniGameResourcesDownLoadWorker")]
class UniGameResourcesDownLoadWorker : MonoBehaviourIgnoreGui
{
    public UniGameResourcesDownLoader AllocResourcesDownLoader_DontDestroyOnLoad()
    {
        UniGameResourcesDownLoader ret = AllocResourcesDownLoader();
        UnityEngine.Object.DontDestroyOnLoad(ret.gameObject);
        return ret;
    }
    public UniGameResourcesDownLoader AllocResourcesDownLoader()
    {
        GameObject go = new GameObject("ResourcesDownLoader", typeof(UniGameResourcesDownLoader));
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        return go.GetComponent<UniGameResourcesDownLoader>();
    }
    public void ReleaseResourcesDownLoader(UniGameResourcesDownLoader loader)
    {
        UnityEngine.Object.DestroyObject(loader.gameObject);
    }
    //加载包清单文件
    public IEnumerator LoadingGameAssetBundleInventoryPackageFile()
    {
        WWW www;
        if (UniGameResources.IsLocalDownloadUrlPath)
        {
            www = WWW.LoadFromCacheOrDownload(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.downloadUrlPath, UniGameResourcesDefine.GameAssetBundleInventoryPackageFilePath),
                            UniGameResources.version_GameAssetBundleInventoryPackageFile);
        }
        else
        {
            www = new WWW(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.downloadUrlPath, UniGameResourcesDefine.GameAssetBundleInventoryPackageFilePath));
        }
        yield return www;
        if (www.error != null)
        {
            GameRoot.Error(string.Format("资源引导文件下载错误:{0}", www.error));
            yield break;
        }
        byte[] data = null;
        try
        {
#if UNITY_4_3 || UNITY_4_6
            TextAsset textAsset = www.assetBundle.Load(UniGameResourcesDefine.GameAssetBundleInventoryXmlFilePath, typeof(TextAsset)) as TextAsset;
#else
            TextAsset textAsset = www.assetBundle.LoadAsset(UniGameResourcesDefine.GameAssetBundleInventoryXmlFilePath, typeof(TextAsset)) as TextAsset;
#endif
            data = textAsset.bytes;
            UniGameResources.ReleaseOneAssets(textAsset);
            www.assetBundle.Unload(false);
            //当获取一次资源对象的时候实际是克隆一个对象
            //UnityEngine.Object.DestroyObject(www.assetBundle);
            www.Dispose();
        }
        catch (System.Exception ex)
        {
            GameRoot.Error(string.Format("资源引导文件读取错误:{0}", ex.ToString()));
            yield break;
        }

        FTEncipher encipher = null;
        try
        {
            //首先从BASE64转换回来
            data = Convert.FromBase64String(Encoding.ASCII.GetString(data));
            //构建解密组件
            encipher = UniGameResources.AllocXmlFileEncipher();
#if _SupportDeviceVerify
            data = encipher.FileDecrypt(data,true);
#else
            data = encipher.FileDecrypt(data);
#endif //_SupportDeviceVerify
            encipher.Dispose();
            encipher = null;
            MemoryStream s = new MemoryStream(data);
            XmlDocument doc = new XmlDocument();
            doc.Load(s);
            s.Close();
            UniGameResources.BuildSystemResourcesPackageTable(doc);
        }
        catch (System.Exception ex)
        {
            GameRoot.Error(string.Format("资源引导文件解析错误:{0}", ex.ToString()));
            if (encipher != null)
            {
                encipher.Dispose();
                encipher = null;
            }
            yield break;
        }
    }
    //加载指定资源包内的指定关卡
    //可以获取加载进度
    protected UniGameResourcesDownLoader SceneLevelDownloader = null;
    public float SceneLevelLoadProgress { get { return SceneLevelDownloader == null ? 0.0f : SceneLevelDownloader.Progress; } }
    public IEnumerator LoadingGameSceneLevel(string packageName,string sceneName)
    {
        if (SceneLevelDownloader != null)
        {
            Debug.LogError("have other scene loader!");
            yield break;
        }
        SceneLevelDownloader = AllocResourcesDownLoader_DontDestroyOnLoad();
        yield return StartCoroutine(SceneLevelDownloader.DownloadPackage(packageName));
        Application.LoadLevel(sceneName);
        ReleaseResourcesDownLoader(SceneLevelDownloader);
        SceneLevelDownloader = null;
    }
    public IEnumerator LoadingGameSceneLevelAdditive(string packageName, string sceneName)
    {
        if (SceneLevelDownloader != null)
        {
            Debug.LogError("have other scene loader!");
            yield break;
        }
        SceneLevelDownloader = AllocResourcesDownLoader_DontDestroyOnLoad();
        yield return StartCoroutine(SceneLevelDownloader.DownloadPackage(packageName));
        Application.LoadLevelAdditive(sceneName);
        ReleaseResourcesDownLoader(SceneLevelDownloader);
        SceneLevelDownloader = null;
    }
    public IEnumerator LoadingGameSceneLevelAsync(string packageName, string sceneName)
    {
        if (SceneLevelDownloader != null)
        {
            Debug.LogError("have other scene loader!");
            yield break;
        }
        SceneLevelDownloader = AllocResourcesDownLoader_DontDestroyOnLoad();
        yield return StartCoroutine(SceneLevelDownloader.DownloadPackage(packageName));
        yield return Application.LoadLevelAsync(sceneName);
        ReleaseResourcesDownLoader(SceneLevelDownloader);
        SceneLevelDownloader = null;
    }
    public IEnumerator LoadingGameSceneLevelAdditiveAsync(string packageName, string sceneName)
    {
        if (SceneLevelDownloader != null)
        {
            Debug.LogError("have other scene loader!");
            yield break;
        }
        SceneLevelDownloader = AllocResourcesDownLoader_DontDestroyOnLoad();
        yield return StartCoroutine(SceneLevelDownloader.DownloadPackage(packageName));
        yield return  Application.LoadLevelAdditiveAsync(sceneName);
        ReleaseResourcesDownLoader(SceneLevelDownloader);
        SceneLevelDownloader = null;
    }

    //在设定资源包内加载一个资源对象
    private uint LoadingAssetObjectCount = 0;
    private Dictionary<uint, UnityEngine.Object> loadingAssetTree = new Dictionary<uint, UnityEngine.Object>(16);
    public uint AllocLoadingAssetObjectGUID() { return LoadingAssetObjectCount++; }
    public UnityEngine.Object PutoffAssetObject(uint guid)
    {
        UnityEngine.Object ret;
        if (!loadingAssetTree.TryGetValue(guid, out ret))
            return null;
        loadingAssetTree.Remove(guid);
        return ret;
    }
    public IEnumerator LoadingAssetObject(string packageName, string assetName, uint assetGuid, Type type, bool isInstantiate)
    {
        UniGameResourcesDownLoader loader = AllocResourcesDownLoader_DontDestroyOnLoad();
        yield return StartCoroutine(loader.DownloadPackage(packageName));
        UniGameResourcesPackage package = UniGameResources.FindSystemResourcesPackageTable(packageName);
        if (package == null)
        {
            Debug.LogError(string.Format("not find package:{0}", packageName));
            yield break;
        }
        UnityEngine.Object obj = null;
        package.LockPackage();
        try
        {
            if (isInstantiate)
            {
#if UNITY_4_3 || UNITY_4_6
                obj = GameObject.Instantiate(package.currentAssetBundle.Load(assetName, type));
                
#else
                obj = GameObject.Instantiate(package.currentAssetBundle.LoadAsset(assetName, type));
#endif
            }
            else
            {
#if UNITY_4_3 || UNITY_4_6
                obj = package.currentAssetBundle.Load(assetName, type);
#else
                obj = package.currentAssetBundle.LoadAsset(assetName, type);
#endif
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        package.UnLockPackage();

        loadingAssetTree.Add(assetGuid, obj);
        ReleaseResourcesDownLoader(loader);
    }
}
