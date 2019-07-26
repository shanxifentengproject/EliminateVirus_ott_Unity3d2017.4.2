using System;
using System.Collections.Generic;

class ILayerManager : LayerManager
{
    //子弹的层
    public int ColliderLayer_Buttle;
    //敌物的层
    public int ColliderLayer_Enemy;
    public ILayerManager()
    {
        ColliderLayer_Buttle = LayerManager.NameToLayer("bullet");
        ColliderLayer_Enemy = LayerManager.NameToLayer("enemy");
    }
}
