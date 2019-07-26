using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using FTLibrary.Encrypt;

class UniDataFileWriter : IDisposable
{
    private const string publickey = "<RSAKeyValue><Modulus>0Mom1T/RCTpXf5mQQLoDOuSc7OD+hL0PLjOIzpwH1ktJhjG/v5o3JATNIc4T8w+b9d+96/EsuJ1m84Br65FMK+qwEpLdfCfKptNi53DTR+qDMoOr+OTQ/4J+Ha2iwzVZadnlTz/qJauRTEyqncwP4/2V+Nyxi5kSRuRCFrR3GVs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
    private const string privatekey = "<RSAKeyValue><Modulus>0Mom1T/RCTpXf5mQQLoDOuSc7OD+hL0PLjOIzpwH1ktJhjG/v5o3JATNIc4T8w+b9d+96/EsuJ1m84Br65FMK+qwEpLdfCfKptNi53DTR+qDMoOr+OTQ/4J+Ha2iwzVZadnlTz/qJauRTEyqncwP4/2V+Nyxi5kSRuRCFrR3GVs=</Modulus><Exponent>AQAB</Exponent><P>+VEkZo2mN7EERE+CzPKc8oZbV82TcK/9YwdKr3Ez9T6QwbGrB7I7WcYONd2XvjO8NRWJORtZrLDV/MVVjgwt1Q==</P><Q>1mLnYey8p5kvBmPyTviV/lSh//5iEZ+DDZ/r8cuEw7UYOFxYA/RxHym19BXqQGfZ2m442PcreKdlDFO3v/1Sbw==</Q><DP>Pu7jkFuLLJ2rZQ7pBpDrWzDdF9HVuOlDvd6WVKjvo6VSZwJRGNU9tBCRf7la13E5vfCcveSQg030BiVNzlh2rQ==</DP><DQ>JJhfbNzPW0CPwWSAMTDH0dE6kgsnTGDRKxs4WF7oO8wG5WAF+i7YvHwPPiobgYD4tAuKkqamegMBAbisrg4c6w==</DQ><InverseQ>e7OlNXd7Rs6V7vfiOGAY5XvW7OM647AOCkuQoYBZ6ok0195eTcX+fiQDciedGcBZHffra/OkjGvzqo8VNqkcxQ==</InverseQ><D>H7B/AmDkPk1PmHtbTKeu89JdCwr/NnBnjHCf9BP0kI7uwuJsIw2qDtp3tAjOOFcHKeNitRvu2LpzNMQqHR/5em6MEA/JyL2bWW3Gi0jdzMkzwZY+MuVRmFLB1mGoXzEQVJzk1ZkdIJkRsVLrjdm6LFXVB+HrDxSjiUcjTFzyh4k=</D></RSAKeyValue>";
    private const string deskey = "88C219CD-7721-46CC-AA53-A361665C542B";

    private static FTEncipher EncryptHandler = null;
    private static FTEncipher DecryptHandler = null;
    private static FTEncipher getEncryptHandler()
    {
        if (EncryptHandler != null)
            return EncryptHandler;
        EncryptHandler = new FTEncipher();
        EncryptHandler.CreatePrivateKeyRSAEncryptProvider(privatekey);
        byte[] desKeyByte = Encoding.ASCII.GetBytes(deskey);
        byte[] rgbKey = new byte[8];
        byte[] rgbIV = new byte[8];
        for (int j = 0; j < 8; j++)
        {
            rgbKey[j] = desKeyByte[j];
            rgbIV[j] = desKeyByte[j + 8];
        }
        EncryptHandler.CreateEncryptProvider(rgbKey, rgbIV);
        return EncryptHandler;
    }
    private static FTEncipher getDecryptHandler()
    {
        if (DecryptHandler != null)
            return DecryptHandler;
        DecryptHandler = new FTEncipher();
        DecryptHandler.CreatePublicKeyRSAEncryptProvider(publickey);
        byte[] desKeyByte = Encoding.ASCII.GetBytes(deskey);
        byte[] rgbKey = new byte[8];
        byte[] rgbIV = new byte[8];
        for (int j = 0; j < 8; j++)
        {
            rgbKey[j] = desKeyByte[j];
            rgbIV[j] = desKeyByte[j + 8];
        }
        DecryptHandler.CreateDecryptProvider(rgbKey, rgbIV);
        return DecryptHandler;
    }
    public static byte[] Encrypt(byte[] data)
    {
        FTEncipher ftEncipher = getEncryptHandler();
        return ftEncipher.FileEncrypt(data);
    }
    public static byte[] Decrypt(byte[] data)
    {
        FTEncipher ftEncipher = getDecryptHandler();
        return ftEncipher.FileDecrypt(data);
    }


    private string FileName = "";
    private bool isSecurityDataFile;
    public BinaryWriter writer = null;
    public void Open(string filename,bool issecuritydatafile = true)
    {
        FileName = filename;
        isSecurityDataFile = issecuritydatafile;
        writer = new BinaryWriter(new MemoryStream(1024));
        writer.Seek(0, SeekOrigin.Begin);
    }
    public void Close()
    {
        if (writer == null)
            return;
        byte[] data = ((MemoryStream)writer.BaseStream).ToArray();
        writer.Close();
        writer = null;

        if (isSecurityDataFile)
        {
            data = UniDataFileWriter.Encrypt(data);
        }
        UniGameResources.WriteSafeFile(UniGameResources.ConnectPath(UniGameResources.PersistentDataPath,
            FileName), data);
    }
    public void Dispose()
    {
        Close();
    }
}
