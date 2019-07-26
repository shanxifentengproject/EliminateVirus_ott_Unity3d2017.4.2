using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class InsertCoinsTimesAlu : InsertCoinsAlu
{
    private float[] currentGameTimes;
    public InsertCoinsTimesAlu(int players)
        :base(players)
    {
        currentGameTimes = new float[PlayerPositionCount];
    }
    //游戏开始，扣除游戏币
    public override bool GameIsStartChargeCoins(PlayerPosition p,UniInsertCoinsOptionsFile opfile)
    {
        if (!GameIsStartChargeCoins(p, opfile))
            return false;
        currentGameTimes[(int)p] = opfile.times;
        return true;
    }
    public float GetPlayerGameTimes(PlayerPosition p) { return currentGameTimes[(int)p]; }
    public void SetPlayerGameTimes(PlayerPosition p, float times) { currentGameTimes[(int)p] = times; }
    //游戏是否超时
    public bool IsGameTimeout(PlayerPosition p)
    {
        return (currentGameTimes[(int)p] <= 0.0f);
    }
    //刷新时间
    public void UpdateGameTime()
    {
        for (int i=0;i<PlayerPositionCount;i++)
        {
            currentGameTimes[i] -= Time.deltaTime;
            if (currentGameTimes[i] < 0.0f)
            {
                currentGameTimes[i] = 0.0f;
            }
        }
    }
}
