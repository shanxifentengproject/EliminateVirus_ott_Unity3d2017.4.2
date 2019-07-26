using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ScoresAwardAlu : AwardAlu
{
    private int[] totalScores;
    private int[] currentScores;
    public ScoresAwardAlu(int playes)
        :base(playes)
    {
        totalScores = new int[PlayerPositionCount];
        currentScores = new int[PlayerPositionCount];
    }
    //返回应该出多少礼品
    public int AddScoresAndGivePlayerAward(InsertCoinsAlu.PlayerPosition p, int scores,UniInsertCoinsOptionsFile opfile, UniInsertCoinsRecord recordFile)
    {
        totalScores[(int)p] += scores;
        currentScores[(int)p] += scores;
        int ret = 0;
        while(currentScores[(int)p] >= opfile.awardNeedScore)
        {
            ret += opfile.awardCount;
            currentScores[(int)p] -= opfile.awardNeedScore;
        }
        if (ret != 0)
        {
            GivePlayerAward(p, opfile, recordFile);
        }
        return ret;
    }

}
