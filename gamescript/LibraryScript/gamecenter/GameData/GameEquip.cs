using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTLibrary.XML;

class GameEquip
{
#if _GameType_BaoYue
    private const string fileName = "gameequip_by.xml";
#else
    private const string fileName = "gameequip.xml";
#endif
    //当前支持的装备数量
    public const int EquipMaxCount = 5;
    public void Load()
    {

    }
}
