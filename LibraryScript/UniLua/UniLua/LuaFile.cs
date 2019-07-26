using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniLua.Tools;

namespace UniLua
{
    class LuaFile
    {
        public static LuaFile currentLuaFile = new LuaFile();
        public static FileLoadInfo OpenFile(string filename)
        {
            return new FileLoadInfo(currentLuaFile.ReadFile(filename));
        }
        public static bool Readable(string filename)
        {
            return currentLuaFile.IsExistsFile(filename);
        }
        public string LUA_ROOT = "";
        public LuaFile()
        {
            LuaFile.currentLuaFile = this;
            LUA_ROOT = Application.streamingAssetsPath + "\\" + "LuaRoot\\";
        }
        protected virtual string GetFilePath(string filename)
        {
            return LUA_ROOT + filename;
        }
        public virtual MemoryStream ReadFile(string filename)
        {
            try
            {
                string filePath = GetFilePath(filename);
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                file.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, buffer.Length);
                file.Close();
                return new MemoryStream(buffer);
            }
            catch (System.Exception ex)
            {
                ULDebug.LogError(ex.ToString());
                return new MemoryStream();
            }
        }
        public virtual bool IsExistsFile(string filename)
        {
            try
            {
                return File.Exists(GetFilePath(filename));
            }
            catch (System.Exception ex)
            {
                ULDebug.LogError(ex.ToString());
                return false;
            }
        }
    }

    internal class FileLoadInfo : ILoadInfo, IDisposable
    {
        public FileLoadInfo(MemoryStream stream)
        {
            Stream = stream;
            Buf = new Queue<byte>();
        }

        public int ReadByte()
        {
            if (Buf.Count > 0)
                return (int)Buf.Dequeue();
            else
                return Stream.ReadByte();
        }

        public int PeekByte()
        {
            if (Buf.Count > 0)
                return (int)Buf.Peek();
            else
            {
                var c = Stream.ReadByte();
                if (c == -1)
                    return c;
                Save((byte)c);
                return c;
            }
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        private const string UTF8_BOM = "\u00EF\u00BB\u00BF";
        private MemoryStream Stream;
        private Queue<byte> Buf;

        private void Save(byte b)
        {
            Buf.Enqueue(b);
        }

        private void Clear()
        {
            Buf.Clear();
        }

        private int SkipBOM()
        {
            for (var i = 0; i < UTF8_BOM.Length; ++i)
            {
                var c = Stream.ReadByte();
                if (c == -1 || c != (byte)UTF8_BOM[i])
                    return c;
                Save((byte)c);
            }
            // perfix matched; discard it
            Clear();
            return Stream.ReadByte();
        }

        public void SkipComment()
        {
            var c = SkipBOM();

            // first line is a comment (Unix exec. file)?
            if (c == '#')
            {
                do
                {
                    c = Stream.ReadByte();
                } while (c != -1 && c != '\n');
                Save((byte)'\n'); // fix line number
            }
            else if (c != -1)
            {
                Save((byte)c);
            }
        }
    }





    //internal class LuaFile
    //{
    //    private static readonly string LUA_ROOT = System.IO.Path.Combine(Application.streamingAssetsPath, "LuaRoot");
    //    public static FileLoadInfo OpenFile( string filename )
    //    {
    //        var path = System.IO.Path.Combine(LUA_ROOT, filename);
    //        return new FileLoadInfo( File.Open( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) );
    //    }

    //    public static bool Readable( string filename )
    //    {
    //        var path = System.IO.Path.Combine(LUA_ROOT, filename);
    //        try {
    //            using( var stream = File.Open( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {
    //                return true;
    //            }
    //        }
    //        catch( Exception ) {
    //            return false;
    //        }
    //    }
    //}

    //internal class FileLoadInfo : ILoadInfo, IDisposable
    //{
    //    public FileLoadInfo( FileStream stream )
    //    {
    //        Stream = stream;
    //        Buf = new Queue<byte>();
    //    }

    //    public int ReadByte()
    //    {
    //        if( Buf.Count > 0 )
    //            return (int)Buf.Dequeue();
    //        else
    //            return Stream.ReadByte();
    //    }

    //    public int PeekByte()
    //    {
    //        if( Buf.Count > 0 )
    //            return (int)Buf.Peek();
    //        else
    //        {
    //            var c = Stream.ReadByte();
    //            if( c == -1 )
    //                return c;
    //            Save( (byte)c );
    //            return c;
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        Stream.Dispose();
    //    }

    //    private const string UTF8_BOM = "\u00EF\u00BB\u00BF";
    //    private FileStream 	Stream;
    //    private Queue<byte>	Buf;

    //    private void Save( byte b )
    //    {
    //        Buf.Enqueue( b );
    //    }

    //    private void Clear()
    //    {
    //        Buf.Clear();
    //    }

    //    private int SkipBOM()
    //    {
    //        for( var i=0; i<UTF8_BOM.Length; ++i )
    //        {
    //            var c = Stream.ReadByte();
    //            if(c == -1 || c != (byte)UTF8_BOM[i])
    //                return c;
    //            Save( (byte)c );
    //        }
    //        // perfix matched; discard it
    //        Clear();
    //        return Stream.ReadByte();
    //    }

    //    public void SkipComment()
    //    {
    //        var c = SkipBOM();

    //        // first line is a comment (Unix exec. file)?
    //        if( c == '#' )
    //        {
    //            do {
    //                c = Stream.ReadByte();
    //            } while( c != -1 && c != '\n' );
    //            Save( (byte)'\n' ); // fix line number
    //        }
    //        else if( c != -1 )
    //        {
    //            Save( (byte)c );
    //        }
    //    }
    //}

}

