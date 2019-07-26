using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTLibrary.XML;

class GameSkill
{
#if _GameType_BaoYue
    private const string fileName = "gameskill_by.xml";
#else
    private const string fileName = "gameskill.xml";
#endif

    public enum SkillId
    {
        Id_Skill1 = 0,
        Id_Skill2 = 1,
        Id_Skill3 = 2,
        Id_SkillCount,
    }

    public struct SkillData
    {
        public SkillId id;
        public int oncemoney;
        public int oncebuycount;
        public int valuei;
        public float valuef;
    }
    public SkillData[] skillData = null;


    public void Load()
    {
        XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile(fileName);
        XmlNode root = doc.SelectSingleNode("GameSkill");
        XmlNodeList nodelist = root.SelectNodes("Skill");
        skillData = new SkillData[nodelist.Count];
        for (int i = 0; i < skillData.Length; i++)
        {
            XmlNode n = nodelist[i];
            SkillData data = new SkillData();
            data.id = (SkillId)Convert.ToInt32(n.Attribute("id"));
            data.oncemoney = Convert.ToInt32(n.Attribute("oncemoney"));
            data.oncebuycount = Convert.ToInt32(n.Attribute("oncebuycount"));
            data.valuei = Convert.ToInt32(n.Attribute("valuei"));
            data.valuef = Convert.ToSingle(n.Attribute("valuef"));
            skillData[i] = data;
        }
    }
    public SkillData FindSkillData(SkillId id)
    {
        for (int i = 0; i < skillData.Length; i++)
        {
            if (skillData[i].id == id)
                return skillData[i];
        }
        return skillData[0];
    }
}

