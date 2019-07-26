using System;
using System.IO;
using System.Collections.Generic;

class UniInsertCoinsRecord : UniGameRecordFileBase
{
    public UniInsertCoinsRecord(string path, UniGameResources gameresources)
        : base(path, gameresources)
    {

    }
    //目前机器总收币数
    public int totalCoins = 0;
    //目前机器总出礼品数
    public int totalAward = 0;
    //每日数据统计
    public struct CoinsRecordData
    {
        public int coins;
        public int award;
        public DateTime date;
    }
    //每日数据统计信息
    public List<CoinsRecordData> coinsRecordData = new List<CoinsRecordData>(16);
    //最大记录数
    private const int MaxRecordDataCount = 365;
    //当前数据
    public int currentCoins
    {
        get
        {
            if (coinsRecordData.Count == 0)
                return 0;
            return coinsRecordData[0].coins;
        }
    }
    public int currentAward
    {
        get
        {
            if (coinsRecordData.Count == 0)
                return 0;
            return coinsRecordData[0].award;
        }
    }

    
    protected override void LoadRecord(BinaryReader reader)
    {
        totalCoins = reader.ReadInt32();
        totalAward = reader.ReadInt32();
        int dataCount = reader.ReadInt32();
        for (int i = 0; i < dataCount;i++ )
        {
            CoinsRecordData data = new CoinsRecordData();
            data.coins = reader.ReadInt32();
            data.award = reader.ReadInt32();
            data.date = DateTime.FromBinary(reader.ReadInt64());
            coinsRecordData.Add(data);
        }
    }
    protected override void SaveRecord(BinaryWriter writer)
    {
        writer.Write(totalCoins);
        writer.Write(totalAward);

        writer.Write(coinsRecordData.Count);
        for (int i = 0; i < coinsRecordData.Count;i++ )
        {
            CoinsRecordData data = coinsRecordData[i];
            writer.Write(data.coins);
            writer.Write(data.award);
            writer.Write(data.date.ToBinary());
        }
    }
    //清除数据
    public override void ClearRecord()
    {
        base.ClearRecord();
        totalCoins = 0;
        totalAward = 0;
        coinsRecordData.Clear();
        SaveRecord();
    }

    //检测是否增加新数据
    private void CheckAddNewData()
    {
        DateTime nowDate = DateTime.Now;
        if (coinsRecordData.Count != 0)
        {
            DateTime dataDate = coinsRecordData[0].date;
            if (dataDate.Year == nowDate.Year &&
                    dataDate.Month == nowDate.Month &&
                    dataDate.Day == nowDate.Day)
                return;
        }
        //需要添加一条新数据
        CoinsRecordData data = new CoinsRecordData();
        data.coins = 0;
        data.award = 0;
        data.date = nowDate;
        coinsRecordData.Insert(0, data);
    }
    //玩家投币
    public void PlayerInsertCoins(int coins)
    {
        totalCoins += coins;
        CheckAddNewData();
        CoinsRecordData data = coinsRecordData[0];
        data.coins += coins;
        coinsRecordData[0] = data;

        SaveRecord();
    }
    //玩家出礼品
    public void PlayerAward(int awardCount)
    {
        totalAward += awardCount;
        CheckAddNewData();
        CoinsRecordData data = coinsRecordData[0];
        data.award += awardCount;
        coinsRecordData[0] = data;

        SaveRecord();
    }
}
