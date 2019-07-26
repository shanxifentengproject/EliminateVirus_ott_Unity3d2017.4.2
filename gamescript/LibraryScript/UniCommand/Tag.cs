using System;
using System.Collections.Generic;
using UnityEngine;

public class Tag : MonoBehaviourIgnoreGui
{
    public enum TagType
    {
        Type_Player = 0,
        Type_Enemy = 1,
        Type_Count = 2,
    }
    public TagType tag = TagType.Type_Enemy;
    public delegate void GameObjectDead(GameObject obj);
    public GameObjectDead theGameObjectDeadFun = null;
    private class TagData
    {
        public List<GameObject> list = new List<GameObject>(32);
        public GameObject[] gameobjectList = null;
        
    }
    private static TagData[] tagList = new TagData[(int)TagType.Type_Count];
    private static GameObject[] emptyGameObject = new GameObject[0];

    public static void Add(Tag tag)
    {
        TagData data = tagList[(int)tag.tag];
        if (data != null)
        {
            data.list.Add(tag.gameObject);
            data.gameobjectList = null;
        }
        else
        {
            data = new TagData();
            data.list.Add(tag.gameObject);
            data.gameobjectList = null;
            tagList[(int)tag.tag] = data;
        }
    }
    public static void Red(Tag tag)
    {
        TagData data = tagList[(int)tag.tag];
        if (data != null)
        {
            data.list.Remove(tag.gameObject);
            data.gameobjectList = null;
        }
    }
    public static GameObject[] FindGameObjectsWithTag(Tag.TagType tagtype)
    {
        TagData data = tagList[(int)tagtype];
        if (data != null)
        {
            if (data.gameobjectList != null)
                return data.gameobjectList;
            data.gameobjectList = data.list.ToArray();
            return data.gameobjectList;
        }
        return emptyGameObject;
    }

    public static void CallTagGameObjectDeadFun(Tag.TagType tagtype,GameObject obj)
    {
        TagData data = tagList[(int)tagtype];
        if (data != null)
        {
            Tag t;
            foreach (GameObject o in data.list)
            {
                t = o.GetComponent<Tag>();
                if (t.theGameObjectDeadFun != null)
                {
                    t.theGameObjectDeadFun(obj);
                }
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        Tag.Add(this);
    }
    protected void OnDestroy()
    {
        Tag.Red(this);
    }
}
