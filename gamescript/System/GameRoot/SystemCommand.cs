using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;

class SystemCommand
{
    public static void Initialization(XmlDocument doc)
    {
        XmlNode root = doc.SelectSingleNode("systemcommand");
        XmlNode node = root.SelectSingleNode("GameScene");
        FirstSceneName = node.Attribute("firstscenename");
        node = root.SelectSingleNode("CompanyLogoProcess");
        XmlNodeList nodelist = node.SelectNodes("Step");
        CompanyLogoOrder = new string[nodelist.Count];
        for (int i = 0; i < CompanyLogoOrder.Length; i++)
        {
            CompanyLogoOrder[i] = nodelist[i].Attribute("texturename");
        }
        //加载音乐配置
        MusicPlayer.Initialization();
        //加载音效配置
        SoundEffectPlayer.Initialization();
    }
    //系统帧率限制
    public static int targetFrameRate = 60;
    //第一次进入的场景
    public static string FirstSceneName = "";
    //公司LOGO顺序
    public static string[] CompanyLogoOrder = null;
}
