using System;
using System.IO;
class UniGameRecordFileBase
{
    protected string filePath = "";
    protected UniGameResources gameResources;
    public UniGameRecordFileBase(string path, UniGameResources gameresources)
    {
        filePath = path;
        gameResources = gameresources;
        byte[] buffer = FTLibrary.Command.ISafeFile.ReadFile(filePath);
        if (buffer != null)
        {
            MemoryStream s = new MemoryStream(buffer);
            if (s.Length > 0)
            {
                BinaryReader reader = new BinaryReader(s);
                LoadRecord(reader);
                reader.Close();
            }
           
        }
    }
    public void SaveRecord()
    {
        MemoryStream s = new MemoryStream(128);
        BinaryWriter writer = new BinaryWriter(s);
        writer.Seek(0, SeekOrigin.Begin);
        SaveRecord(writer);
        byte[] buffer = s.ToArray();
        writer.Close();
        FTLibrary.Command.ISafeFile.WriteFile(filePath, buffer);
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
        byte[] buffer = reader.ReadBytes(Length);
        FTLibrary.Command.ISafeFile.WriteFile(filePath, buffer);
    }
    protected virtual void LoadRecord(BinaryReader reader)
    {

    }
    protected virtual void SaveRecord(BinaryWriter writer)
    {

    }
    public virtual void PrintReportForms(StreamWriter file)
    {

    }
    public virtual void ClearRecord()
    {

    }
}
