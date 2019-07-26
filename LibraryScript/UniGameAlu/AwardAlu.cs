using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class AwardAlu
{
    protected int PlayerPositionCount;
    public AwardAlu(int playes)
    {
        PlayerPositionCount = playes;
    }
    //奖励礼品
    public void GivePlayerAward(InsertCoinsAlu.PlayerPosition p, UniInsertCoinsOptionsFile opfile, UniInsertCoinsRecord recordFile)
    {
        recordFile.PlayerAward(opfile.awardCount);
    }
}
