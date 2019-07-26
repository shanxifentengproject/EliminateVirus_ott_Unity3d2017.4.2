using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
class ObjectCollectionT<T> : IDisposable
{
    protected Dictionary<uint, List<T>> objectCollection = new Dictionary<uint, List<T>>(32);
    protected System.Comparison<T> comparisonFun = null;
    public ObjectCollectionT(System.Comparison<T> comFun)
    {
        comparisonFun = comFun;
    }

    public void Add(string keyName,T ele)
    {
        Add(FTLibrary.Command.FTUID.StringGetHashCode(keyName), ele);
    }
    public void Add(uint key,T ele)
    {
        List<T> list;
        try
        {
            if (!objectCollection.TryGetValue(key, out list))
            {
                list = new List<T>(128);
                objectCollection.Add(key, list);
            }
            list.Add(ele);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    public void Remove(string keyName,T ele)
    {
        Remove(FTLibrary.Command.FTUID.StringGetHashCode(keyName), ele);
    }
    public void Remove(uint key,T ele)
    {
        List<T> list;
        try
        {
            if (!objectCollection.TryGetValue(key, out list))
                return;
            for (int i = 0; i < list.Count;i++ )
            {
                if (comparisonFun(list[i],ele) == 0)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    public List<T> GetList(string keyName)
    {
        return GetList(FTLibrary.Command.FTUID.StringGetHashCode(keyName));
    }
    public List<T> GetList(uint key)
    {
        List<T> list;
        try
        {
            if (!objectCollection.TryGetValue(key, out list))
                return null;
            return list;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        return null;
    }
    public void Dispose()
    {
        Clear();
        objectCollection.Clear();
    }
    public void Clear()
    {
        Dictionary<uint, List<T>>.Enumerator list = objectCollection.GetEnumerator();
        while (list.MoveNext())
        {
            list.Current.Value.Clear();
        }
        list.Dispose();
    }
}
