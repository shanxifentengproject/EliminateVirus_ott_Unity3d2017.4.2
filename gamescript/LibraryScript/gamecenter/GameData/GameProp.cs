using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GameProp
{
#if _GameType_BaoYue
    private const string fileName = "gameprop_by.xml";
#else
    private const string fileName = "gameprop.xml";
#endif
    public void Load()
    {

    }
}
