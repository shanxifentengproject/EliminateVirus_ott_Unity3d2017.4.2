using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

abstract class GuiUiSceneManager : GuiUiSceneBase
{
    protected GuiUiSceneBase AloneScene = null;
    protected Dictionary<int, GuiUiSceneBase> PoolingSceneList = new Dictionary<int, GuiUiSceneBase>(8);


    public GuiUiSceneBase ActiveAloneUiScene(string prefabsName, params object[] args)
    {
        if (AloneScene != null)
        {
            UnityEngine.Object.DestroyObject(AloneScene.gameObject);
            AloneScene = null;
        }
        if (prefabsName != "")
        {
            AloneScene = LoadResource_UIPrefabs(prefabsName).GetComponent<GuiUiSceneBase>();
            if (AloneScene != null)
            {
                AloneScene.SetTransferParameter(args);
            }
        }
        return AloneScene;
    }
    public int GetCurrentAloneSceneId
    {
        get
        {
            if (AloneScene == null)
                return -1;
            return AloneScene.uiSceneId;
        }
        
    }
    public GuiUiSceneBase getAloneScene { get { return AloneScene; } }
    public GuiUiSceneBase ActivePoolingScene(string prefabsName, params object[] args)
    {
        if (prefabsName == "")
            return null;
        GuiUiSceneBase s = LoadResource_UIPrefabs(prefabsName).GetComponent<GuiUiSceneBase>();
        if (s == null)
            return null;
        if (PoolingSceneList.ContainsKey(s.uiSceneId))
        {
            UnityEngine.Object.DestroyObject(s.gameObject);
            return null;
        }
        PoolingSceneList.Add(s.uiSceneId, s);
        s.SetTransferParameter(args);
        return s;
    }
    public GuiUiSceneBase FindPoolingScene(int id)
    {
        GuiUiSceneBase ret = null;
        if (!PoolingSceneList.TryGetValue(id, out ret))
            return null;
        return ret;
    }
    public void RemovePoolingScene(int id)
    {
        PoolingSceneList.Remove(id);
    }
    public void ReleasePoolingScene(int id)
    {
        GuiUiSceneBase p = FindPoolingScene(id);
        if (p != null)
        {
            PoolingSceneList.Remove(id);
            UnityEngine.Object.DestroyObject(p.gameObject);
        }
    }

}
