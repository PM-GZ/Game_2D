using System.IO;
using System.Text;
using System.Collections.Generic;


public static class TEXT
{
    private const string PACKET_NAME = "Languages.p";
    private static Dictionary<string, byte[]> mLanguageDataDict = new();
    private static Dictionary<string, string> mTextDict = new();

    public static void Init(string language = "CN")
    {
        GetData($"{language}.bytes");
    }

    private static void GetData(string fileName)
    {
        if (!mLanguageDataDict.TryGetValue(fileName, out var data))
        {
            data = PacketUtils.GetPacket(PACKET_NAME);
            LoadData(data, fileName);
        }
        data = mLanguageDataDict[fileName];
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                InitData(reader, data.Length);
                stream.Close();
                reader.Close();
            }
        }
    }

    private static void LoadData(byte[] data, string fileName)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                while (reader.BaseStream.Position < data.Length)
                {
                    string name = reader.ReadString();
                    var length = reader.ReadInt32();
                    if (name.Equals(fileName))
                    {
                        mLanguageDataDict.Add(fileName, reader.ReadBytes((int)stream.Position + length));
                        stream.Close();
                        reader.Close();
                        return;
                    }
                    reader.BaseStream.Position += length;
                }
            }
        }
    }

    private static void InitData(BinaryReader reader, long length)
    {
        mTextDict.Clear();

        while (reader.BaseStream.Position < length)
        {
            var key = reader.ReadString();
            var value = reader.ReadString();
            mTextDict.Add(key, value);
        }
    }

    public static string GetText(string key)
    {
        if (mTextDict.TryGetValue(key, out var text))
        {
            return text;
        }
        return string.Empty;
    }
}
