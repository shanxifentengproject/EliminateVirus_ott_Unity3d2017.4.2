using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class MonoBehaviourIgnoreGui : MonoBehaviour
{
    protected virtual void Awake()
    {
        useGUILayout = false;
    }
}
