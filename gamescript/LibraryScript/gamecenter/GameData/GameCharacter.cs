using System;
using System.Collections.Generic;
using FTLibrary.XML;

class GameCharacter
{
#if _GameType_BaoYue
    private const string fileName = "gamecharacter_by.xml";
#else
    private const string fileName = "gamecharacter.xml";
#endif
    public struct CharacterData
    {
        public int activemoney;
        public int maxlevel;
        public int realindex;
        public int initzoomindex;
        public int maxzoomindex;
        public struct Curvei2
        {
            public int A;
            public int B;
            public Curvei2(int a, int b)
            {
                A = a;
                B = b;
            }
            //x可以取任意值
            public int GetValue(int x)
            {
                return B + A * x;
            }
        }
        public Curvei2 LevelToMoney;
        public Curvei2 LevelToAttack;
        public Curvei2 LevelToExact;
        public Curvei2 RealLevelToAttack;
        public Curvei2 RealLevelToExact;
    }
    public CharacterData[] characterDataList = null;
    public void Load()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("GameCharacter");
        XmlNode node = root.SelectSingleNode("CharacterData");
        XmlNodeList nodelist = node.SelectNodes("Character");
        XmlNode n, n1;
        characterDataList = new CharacterData[nodelist.Count];
        for (int i = 0; i < characterDataList.Length; i++)
        {
            n = nodelist[i];

            characterDataList[i].activemoney = Convert.ToInt32(n.Attribute("activemoney"));
            characterDataList[i].maxlevel = Convert.ToInt32(n.Attribute("maxlevel"));
            characterDataList[i].realindex = Convert.ToInt32(n.Attribute("realindex"));
            characterDataList[i].initzoomindex = Convert.ToInt32(n.Attribute("initzoomindex"));
            characterDataList[i].maxzoomindex = Convert.ToInt32(n.Attribute("maxzoomindex"));

            n1 = n.SelectSingleNode("LevelToMoney");
            characterDataList[i].LevelToMoney = new CharacterData.Curvei2(Convert.ToInt32(n1.Attribute("a")),
                Convert.ToInt32(n1.Attribute("b")));
            n1 = n.SelectSingleNode("LevelToAttack");
            characterDataList[i].LevelToAttack = new CharacterData.Curvei2(Convert.ToInt32(n1.Attribute("a")),
                Convert.ToInt32(n1.Attribute("b")));
            n1 = n.SelectSingleNode("LevelToExact");
            characterDataList[i].LevelToExact = new CharacterData.Curvei2(Convert.ToInt32(n1.Attribute("a")),
                Convert.ToInt32(n1.Attribute("b")));
            n1 = n.SelectSingleNode("RealLevelToAttack");
            characterDataList[i].RealLevelToAttack = new CharacterData.Curvei2(Convert.ToInt32(n1.Attribute("a")),
                Convert.ToInt32(n1.Attribute("b")));
            n1 = n.SelectSingleNode("RealLevelToExact");
            characterDataList[i].RealLevelToExact = new CharacterData.Curvei2(Convert.ToInt32(n1.Attribute("a")),
                Convert.ToInt32(n1.Attribute("b")));
        }

    }
}