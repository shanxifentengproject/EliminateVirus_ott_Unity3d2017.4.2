using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class IUniPlayerPrefs: UniPlayerPrefs
{
    protected override void SaveData(string name, string data)
    {
        GameCenterEviroment.currentGameCenterEviroment.setPlayerParam(name, data);
    }
    protected override string LoadData(string name)
    {
        return GameCenterEviroment.currentGameCenterEviroment.getPlayerParam(name);
    }
}
