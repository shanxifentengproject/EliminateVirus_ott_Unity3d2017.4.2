using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class InsertCoinsAlu
{
    public enum PlayerPosition
    {
        Position_P1        = 0,
        Position_P2        = 1,
        Position_P3        = 2,
        Position_P4        = 3,
        Position_P5        = 4,
        Position_P6        = 5,
    }
    protected int PlayerPositionCount;
    //当前存储的投币数
    private int[] currentInsertCoins;
    public InsertCoinsAlu(int players)
    {
        PlayerPositionCount = players;
        currentInsertCoins = new int[PlayerPositionCount];
    }
    //玩家投币触发
    public void PlayerInsertCoins(PlayerPosition p,int conis,UniInsertCoinsRecord recordFile)
    {
        currentInsertCoins[(int)p] += conis;
        recordFile.PlayerInsertCoins(conis);
    }
    public int GetPlayerInsertCoins(PlayerPosition p) { return currentInsertCoins[(int)p]; }
    public void SetPlayerInsertCoins(PlayerPosition p, int coins) { currentInsertCoins[(int)p] = coins; }
    //是否可以开始游戏
    public bool IsCanStartGame(PlayerPosition p,UniInsertCoinsOptionsFile opfile)
    {
        if (opfile.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
            return true;
        return (GetPlayerInsertCoins(p) >= opfile.coins);
    }
    //游戏开始，扣除游戏币
    public virtual bool GameIsStartChargeCoins(PlayerPosition p,UniInsertCoinsOptionsFile opfile)
    {
        if (!IsCanStartGame(p, opfile))
            return false;
        if (opfile.chargeMode == UniInsertCoinsOptionsFile.GameChargeMode.Mode_Free)
            return true;
        SetPlayerInsertCoins(p, GetPlayerInsertCoins(p) - opfile.coins);
        return true;
    }
}
