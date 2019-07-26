using System;
using System.Collections.Generic;
using System.IO;
class UniDataFileReader : IDisposable
{
    public BinaryReader reader = null;
    public bool Open(string fileName, bool issecuritydatafile = true)
    {
        byte[] data = UniGameResources.ReadSafeFile(UniGameResources.ConnectPath(UniGameResources.PersistentDataPath,
            fileName));
        if (data == null)
            return false;

        if (issecuritydatafile)
        {
            try
            {
                data = UniDataFileWriter.Decrypt(data);
                if (data == null)
                    return false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        reader = new BinaryReader(new MemoryStream(data));
        return true;
    }
    public void Close()
    {
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }
    public void Dispose()
    {
        Close();
    }
}
