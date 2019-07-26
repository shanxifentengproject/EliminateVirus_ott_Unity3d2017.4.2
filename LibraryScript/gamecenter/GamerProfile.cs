using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
abstract class GamerProfile
{
     //载入用户档案数据
    public abstract bool Initialization();
    //请求存储一次档案
    public abstract void SaveGamerProfileToServer();
    //用户档案是否载入成功
    public abstract bool IsPlayerDataLoadSucceed{get;}

    public int getPlayerScore()
    {
        return GameCenterEviroment.currentGameCenterEviroment.getPlayerScore();
    }
    public void setPlayerScore(int score)
    {
        GameCenterEviroment.currentGameCenterEviroment.setPlayerScore(score);
    }
    public virtual GameCenterEviroment.PlatformChargeIntensity getPlatformChargeIntensity()
    {
        return GameCenterEviroment.PlatformChargeIntensity.Intensity_High;
    }
}
