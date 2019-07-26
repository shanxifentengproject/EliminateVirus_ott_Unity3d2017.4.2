using System;
using System.Collections.Generic;
using UnityEngine;
class LayerManager
{
    //获取层的掩码值
    public static int getLayerMask(int layer) { return (1 << layer); }
    //获取层的值
    public static int NameToLayer(string name) { return LayerMask.NameToLayer(name); }
}
