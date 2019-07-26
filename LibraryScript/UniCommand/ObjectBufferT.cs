using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FTLibrary.Command;
class ObjectBufferT<T> : IDisposable
{
    protected Dictionary<uint, List<T>> objectBuffer = new Dictionary<uint, List<T>>(32);
    //托管一个游戏对象
    public void TrusteeshipObject(string keyName, T obj)
    {
        List<T> objList;
        uint key = FTUID.StringGetHashCode(keyName);
        try
        {
            if (!objectBuffer.TryGetValue(key, out objList))
            {
                objList = new List<T>(128);
                objectBuffer.Add(key, objList);
            }
            objList.Add(obj);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    //分配一个游戏对象
    public bool AllocObject(string keyName, out T obj)
    {
        obj = default(T);
        List<T> objList;
        uint key = FTUID.StringGetHashCode(keyName);
        try
        {
            if (!objectBuffer.TryGetValue(key, out objList))
            {
                return false;
            }
            if (objList.Count == 0)
                return false;
            obj = objList[0];
            objList.RemoveAt(0);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        return false;
    }
    public List<T> GetObjectBuffer(string keyName)
    {
        List<T> objList;
        uint key = FTUID.StringGetHashCode(keyName);
        try
        {
            if (!objectBuffer.TryGetValue(key, out objList))
            {
                return null;
            }
            return objList;
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
        objectBuffer.Clear();
    }
    public void Clear()
    {
        Dictionary<uint, List<T>>.Enumerator list = objectBuffer.GetEnumerator();
        while (list.MoveNext())
        {
            list.Current.Value.Clear();
        }
        list.Dispose();
    }
}


