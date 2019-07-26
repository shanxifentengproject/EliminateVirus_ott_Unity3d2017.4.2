using System;
using System.Collections.Generic;
using System.Text;
class UniGameResourcesSupport
{
    //当前的程序版本
    public virtual string ProgramVersion { get { return "1.000"; } }
    //如果填写为local,则表示使用本地资源包
    //这时候需要将所有的资源包都配置成本地的
    internal const string LoaclDownloadUrlPathMode = "local";
    public virtual string DownloadUrlPath { get { return "http://127.0.0.1/unitygame/"; } }

    public virtual string XmlRsaPublicKey { get { return "<RSAKeyValue><Modulus>qJY5035tO2Nzgp7Tg8EkZI9qou2vn3HZpFxX7ScNaRxwUnFpkXF5ZHWJgPlm4VCW0DpCVmooYDx5ZCJsKANx01r5Lb9VFrqNJtUlfXNBlnz2zjDHIvbB1QPMB9a+k/TFbt5P+rtOuDbmswEUcDX+uMcANo/RkK5JiAsGHKx7Cz8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
    public virtual string XmlDesKey { get { return "B88DE37A-9A24-4E5A-98B8-47AE9C9D4D82"; } }
    public virtual void ResolveXmlDesKey(out byte[] rgbKey, out byte[] rgbIV)
    {
        rgbKey = new byte[8];
        rgbIV = new byte[8];
        byte[] data = Encoding.ASCII.GetBytes(XmlDesKey);
        for (int i = 0; i < 8;i++ )
        {
            rgbKey[i] = data[i];
            rgbIV[i] = data[i + 8];
        }
    }

    public virtual string LuaRsaPublicKey { get { return "<RSAKeyValue><Modulus>qJY5035tO2Nzgp7Tg8EkZI9qou2vn3HZpFxX7ScNaRxwUnFpkXF5ZHWJgPlm4VCW0DpCVmooYDx5ZCJsKANx01r5Lb9VFrqNJtUlfXNBlnz2zjDHIvbB1QPMB9a+k/TFbt5P+rtOuDbmswEUcDX+uMcANo/RkK5JiAsGHKx7Cz8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
    public virtual string LuaDesKey { get { return "B88DE37A-9A24-4E5A-98B8-47AE9C9D4D82"; } }
    public virtual void ResolveLuaDesKey(out byte[] rgbKey, out byte[] rgbIV)
    {
        rgbKey = new byte[8];
        rgbIV = new byte[8];
        byte[] data = Encoding.ASCII.GetBytes(LuaDesKey);
        for (int i = 0; i < 8; i++)
        {
            rgbKey[i] = data[i];
            rgbIV[i] = data[i + 8];
        }
    }
}
