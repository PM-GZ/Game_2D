using System.Collections.Generic;
using System.IO;
using System.Text;

public static class TEXT
{



    private static Dictionary<string, string> mTextDict = new();


    public static void Init()
    {
        byte[] data = PacketUtils.GetPacket("Languages.p");
        InitData(data);
    }

    private static void InitData(byte[] data)
    {
        mTextDict.Clear();

        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                while (reader.BaseStream.Position < data.Length)
                {
                    var length = reader.ReadInt32();
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    mTextDict.Add(key, value);
                }
            }
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
