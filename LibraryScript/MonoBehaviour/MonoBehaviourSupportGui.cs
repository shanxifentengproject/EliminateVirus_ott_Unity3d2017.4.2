using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class MonoBehaviourSupportGui : MonoBehaviour
{
    protected virtual void Awake()
    {
        useGUILayout = true;
    }
}
