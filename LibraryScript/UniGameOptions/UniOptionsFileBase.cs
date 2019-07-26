using System;
using System.IO;

class UniOptionsFileBase
{
    protected string filePath;
    protected UniGameResources gameResources;
    public UniOptionsFileBase(string path, UniGameResources gameresources)
    {
        filePath = path;
        gameResources = gameresources;
        LoadOptions();
    }
    public virtual uint OptionsType { get { return 0; } }
    //当前配置文件的版本号
    //直接版本号一致才可以读取
    protected virtual string OptionsVersion { get { return "1.0"; } }
    public void LoadOptions()
    {
        //这里有可能由于配置代码和配置的存储文件不一致导致错误
        //这时候就需要重新填充为系统默认的配置信息了
        try
        {
            byte[] buffer = FTLibrary.Command.ISafeFile.ReadFile(filePath);
            if (buffer != null)
            {
                MemoryStream s = new MemoryStream(buffer);
                BinaryReader reader = new BinaryReader(s);
                LoadOptions(reader);
                reader.Close();
            }
            else
            {
                FillDefaultOptions();
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.ToString());
            FillDefaultOptions();
        }
    }
    public void SaveOptions()
    {
        MemoryStream s = new MemoryStream(128);
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        SaveOptions(writer);
        byte[] buffer = s.ToArray();
        writer.Close();
        FTLibrary.Command.ISafeFile.WriteFile(filePath, buffer);
    }
    public void RemoveOptions()
    {
        FTLibrary.Command.ISafeFile.RemoveFile(filePath);
        FillDefaultOptions();
    }
    public void SerializeRead(BinaryWriter writer)
    {
        byte[] buffer = FTLibrary.Command.ISafeFile.ReadFile(filePath);
        if (buffer == null)
        {
            int Length = 0;
            writer.Write(Length);
        }
        else
        {
            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
    }
    public void SerializeWrite(BinaryReader reader)
    {
        int Length = reader.ReadInt32();
        if (Length > 0)
        {
            byte[] buffer = reader.ReadBytes(Length);
            FTLibrary.Command.ISafeFile.WriteFile(filePath, buffer);
            LoadOptions();
        }
        else
        {
            RemoveOptions();
        }
        
    }
    protected virtual void FillDefaultOptions()
    {

    }
    protected virtual void LoadOptions(BinaryReader reader)
    {
        //首先读取并对比版本号
        string version = reader.ReadString();
        if (version != OptionsVersion)
            throw new Exception("diverse options version!");
    }
    protected virtual void SaveOptions(BinaryWriter writer)
    {
        //首先存储版本号
        writer.Write(OptionsVersion);
    }
}
