using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTLibrary.XML;

class FtGameLevel
{
#if _GameType_BaoYue
    private const string fileName = "gamelevel_by.xml";
#else
    private const string fileName = "gamelevel.xml";
#endif
    public struct MapData
    {
        public struct LevelData
        {
            public int time;
            public int missiontarget;
            public int winawardmoney;
            public int failawardmoney;
            public int oneemenymoney;
            public int oneemenyhp;
        }
        public LevelData[] levelData;
        public string scenename;
    }
    public MapData[] mapData = null;
    //为了方便计算这里计算出每张地图的关卡数
    public int[] mapMaxLevel = null;

    public void Load()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("GameLevel");
        XmlNode node = root.SelectSingleNode("LevelData");
        XmlNodeList itemlist = node.SelectNodes("Map");
        mapData = new MapData[itemlist.Count];
        for (int i = 0; i < mapData.Length; i++)
        {
            XmlNode mapNode = itemlist[i];
            mapData[i].scenename = mapNode.Attribute("scenename");
            XmlNodeList nodelist = mapNode.SelectNodes("Level");
            mapData[i].levelData = new MapData.LevelData[nodelist.Count];
            for (int j = 0; j < mapData[i].levelData.Length; j++)
            {
                XmlNode n = nodelist[j];
                mapData[i].levelData[j].time = Convert.ToInt32(n.Attribute("time"));
                mapData[i].levelData[j].missiontarget = Convert.ToInt32(n.Attribute("missiontarget"));
                mapData[i].levelData[j].winawardmoney = Convert.ToInt32(n.Attribute("winawardmoney"));
                mapData[i].levelData[j].failawardmoney = Convert.ToInt32(n.Attribute("failawardmoney"));
                mapData[i].levelData[j].oneemenymoney = Convert.ToInt32(n.Attribute("oneemenymoney"));
                mapData[i].levelData[j].oneemenyhp = Convert.ToInt32(n.Attribute("oneemenyhp"));
            }
        }

        mapMaxLevel = new int[mapData.Length];
        for (int i = 0; i < mapMaxLevel.Length; i++)
        {
            mapMaxLevel[i] = mapData[i].levelData.Length;
        }


    }
}

