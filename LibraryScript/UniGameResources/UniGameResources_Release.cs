using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Resources;
using UnityEngine;

partial class UniGameResources: GameResources
{
    public static void ReleaseOneAssets(UnityEngine.Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }
    public static UniGameResourcesReleaseUnusedAssets releaseUnusedAssetsObject = null;
    public static bool AllocUnloadUnusedAssetsGameObject()
    {
        if (releaseUnusedAssetsObject != null)
            return false;
        GameObject go = new GameObject("UniGameResourcesReleaseUnusedAssets", typeof(UniGameResourcesReleaseUnusedAssets));
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        UnityEngine.Object.DontDestroyOnLoad(go);
        releaseUnusedAssetsObject = go.GetComponent<UniGameResourcesReleaseUnusedAssets>();
        return true;
    }
    public static void UnloadUnusedAssets()
    {
        if (releaseUnusedAssetsObject == null)
            return;
        releaseUnusedAssetsObject.CallUnloadUnusedAssets();
    }
}
