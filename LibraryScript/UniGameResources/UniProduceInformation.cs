using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.XML;
using System.IO;
class UniProduceInformation
{
    //产品版本
    public string ProduceVersion;
    //当前支持的程序版本
    //<!--支持直接升级的程序版本，如果程序不是这个版本则无法直接升级，应该重新安装最新版本的程序-->
    public string SupportProgramVersion;
    //游戏ID
    public uint GameId;
    //机器ID,这个值是在程序第一次运行的时候分配的，用来标记一台设备
    //一般产品和产品之间的都是不一样的
    public HelperInterface.ProduceActivateId produceActivateId;

    private void ReadProduceActivateIdFile()
    {
        byte[] data = UniGameResources.ReadFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath,
                                    UniGameResourcesDefine.ProduceActivateIdFileName));
        if (data != null)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));
            produceActivateId = new HelperInterface.ProduceActivateId();
            produceActivateId.activateId = reader.ReadString();
            produceActivateId.activateDate = DateTime.Parse(reader.ReadString());
            reader.Close();
        }
        else
        {
            produceActivateId = new HelperInterface.ProduceActivateId();
            produceActivateId.activateId = Guid.NewGuid().ToString();
            produceActivateId.activateDate = DateTime.Now;
            MemoryStream s=new MemoryStream(256);
            BinaryWriter writer = new BinaryWriter(s);
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(produceActivateId.activateId);
            writer.Write(produceActivateId.activateDate.ToString());
            writer.Flush();
            data = s.ToArray();
            writer.Close();
            UniGameResources.WriteFile(FTLibrary.Text.IStringPath.ConnectPath(UniGameResources.PersistentDataPath,
                                    UniGameResourcesDefine.ProduceActivateIdFileName), data);
        }
    }
    public void Initialization(XmlNode node)
    {
        GameId = Convert.ToUInt32(node.Attribute("gameid"));
        ProduceVersion = node.Attribute("produceversion");
        SupportProgramVersion = node.Attribute("supportprogramversion");
        //读取产品激活ID
        ReadProduceActivateIdFile();
    }
}
