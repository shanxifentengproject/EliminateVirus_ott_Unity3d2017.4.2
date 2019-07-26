using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

abstract class UniPlayerPrefs: MonoBehaviourIgnoreGui
{
    private Hashtable dataTable = new Hashtable(32);
    private const string PlayerPrefsName = "PlayerPrefs";
    private const char SplitChar = '#';
    private enum DataType
    {
        Type_Int = 0x01,
        Type_Float = 0x02,
        Type_String = 0x03,
    }
    private bool IsModify = false;

    private void privateDeleteAll()
    {
        dataTable.Clear();
        IsModify = true;
    }
    private void privateDeleteKey(string key)
    {
        dataTable.Remove(key);
        IsModify = true;
    }
    private float privateGetFloat(string key, float defaultValue = 0.0f)
    {
        object ret = dataTable[key];
        if (ret == null)
            return defaultValue;
        if (!(ret is float))
            return defaultValue;
        return (float)ret;
    }
    private int privateGetInt(string key, int defaultValue = 0)
    {
        object ret = dataTable[key];
        if (ret == null)
            return defaultValue;
        if (!(ret is int))
            return defaultValue;
        return (int)ret;
    }
    private string privateGetString(string key, string defaultValue ="")
    {
        object ret = dataTable[key];
        if (ret == null)
            return defaultValue;
        if (!(ret is string))
            return defaultValue;
        return (string)ret;
    }

    private bool privateHasKey(string key)
    {
        return dataTable.ContainsKey(key);
    }

    private void privateSetFloat(string key, float value)
    {
        dataTable.Remove(key);
        dataTable.Add(key, value);
        IsModify = true;
    }
    private void privateSetInt(string key, int value)
    {
        dataTable.Remove(key);
        dataTable.Add(key, value);
        IsModify = true;
    }
    private void privateSetString(string key, string value)
    {
        dataTable.Remove(key);
        dataTable.Add(key, value);
        IsModify = true;
    }

    private void privateSave()
    {
        if (!IsModify)
            return;
        StringBuilder buf = new StringBuilder(256);
        IDictionaryEnumerator list = dataTable.GetEnumerator();
        while(list.MoveNext())
        {
            buf.Append((string)list.Key); buf.Append(SplitChar);
            if (list.Value is int)
            {
                buf.Append((int)DataType.Type_Int); buf.Append(SplitChar);
                buf.Append((int)list.Value); buf.Append(SplitChar);
            }
            else if (list.Value is float)
            {
                buf.Append((int)DataType.Type_Float); buf.Append(SplitChar);
                buf.Append((float)list.Value); buf.Append(SplitChar);
            }
            else if (list.Value is string)
            {
                buf.Append((int)DataType.Type_String); buf.Append(SplitChar);
                buf.Append((string)list.Value); buf.Append(SplitChar);
            }
            else
            {
                throw new Exception("unknown type value!");
            }
        }
        SaveData(PlayerPrefsName,buf.ToString());
        IsModify = false;
    }
    private void privateLoad()
    {
        string data = LoadData(PlayerPrefsName);
        if (data == "")
            return;
        string[] list = data.Split(SplitChar);
        if ((list.Length - 1)%3 != 0)
            return;
        dataTable.Clear();
        for (int i = 0;i<(list.Length - 1);i+=3)
        {
            DataType type = (DataType)Convert.ToInt32(list[i + 1]);
            switch(type)
            {
                case DataType.Type_Int:
                    dataTable.Add(list[i], Convert.ToInt32(list[i + 2]));
                    break;
                case DataType.Type_Float:
                    dataTable.Add(list[i], Convert.ToSingle(list[i + 2]));
                    break;
                case DataType.Type_String:
                    dataTable.Add(list[i], list[i + 2]);
                    break;
            }
        }
    }
    protected abstract void SaveData(string name,string data);
    protected abstract string LoadData(string name);

    private static UniPlayerPrefs myInstance = null;
    public static UniPlayerPrefs Instance
    {
        get
        {
            return myInstance;
        }
        set
        {
            myInstance = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        UniPlayerPrefs.Instance = this;
        UnityEngine.Object.DontDestroyOnLoad(this);
        UniPlayerPrefs.Instance.privateLoad();
    }
    private void Update()
    {
        if (IsModify)
            privateSave();
    }

    public static void DeleteAll()
    {
        UniPlayerPrefs.Instance.privateDeleteAll();
    }
    public static void DeleteKey(string key)
    {
        UniPlayerPrefs.Instance.privateDeleteKey(key);
    }
    public static float GetFloat(string key, float defaultValue = 0.0f)
    {
        return UniPlayerPrefs.Instance.privateGetFloat(key, defaultValue);
    }
    public static int GetInt(string key, int defaultValue = 0)
    {
        return UniPlayerPrefs.Instance.privateGetInt(key, defaultValue);
    }
    public static string GetString(string key, string defaultValue = "")
    {
        return UniPlayerPrefs.Instance.privateGetString(key, defaultValue);
    }
    public static bool HasKey(string key)
    {
        return UniPlayerPrefs.Instance.privateHasKey(key);
    }
    public static void Save()
    {
        UniPlayerPrefs.Instance.privateSave();
    }
    public static void SetFloat(string key, float value)
    {
        UniPlayerPrefs.Instance.privateSetFloat(key,value);
    }
    public static void SetInt(string key, int value)
    {
        UniPlayerPrefs.Instance.privateSetInt(key, value);
    }
    public static void SetString(string key, string value)
    {
        UniPlayerPrefs.Instance.privateSetString(key, value);
    }
    public static void Initialization(Type classType)
    {
        GameObject playerPrefs = new GameObject("PlayerPrefs");
        playerPrefs.AddComponent(classType);
        playerPrefs.hideFlags = HideFlags.HideAndDontSave;

    }


}
