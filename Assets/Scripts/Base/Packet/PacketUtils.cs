using System.IO;
using System.Collections.Generic;



public class PacketUtils
{
    private struct BuildData
    {
        public string FileName;
        public byte[] Data;
    }


    private static volatile Dictionary<string, List<BuildData>> mBuildDataDict = new();
    private static Dictionary<string, byte[]> mPacketDict = new();

    public static void AddFile(string fileName, byte[] data)
    {
        lock (mBuildDataDict)
        {
            var buildData = new BuildData
            {
                FileName = fileName,
                Data = data
            };

            if (!mBuildDataDict.ContainsKey(fileName))
            {
                mBuildDataDict.Add(fileName, new List<BuildData>());
            }
            mBuildDataDict[fileName].Add(buildData);
        }
    }

    public static void BuildAll()
    {
        if (!Directory.Exists(Constant.BUILTIN_PATH))
        {
            Directory.CreateDirectory(Constant.BUILTIN_PATH);
        }
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        foreach (var dic in mBuildDataDict)
        {
            foreach (var data in dic.Value)
            {
                writer.Write(data.Data);
            }
            File.WriteAllBytes(Constant.BUILTIN_PATH + dic.Key, stream.ToArray());
            stream.Position = 0;
        }
        stream.Close();
        writer.Close();
    }

    public static byte[] GetPacket(string packetName)
    {
        if (!mPacketDict.ContainsKey(packetName))
        {
            if (File.Exists(Constant.BUILTIN_PATH + packetName))
            {
                var bytes = File.ReadAllBytes(Constant.BUILTIN_PATH + packetName);
                mPacketDict.Add(packetName, bytes);
            }
            else
            {
                return null;
            }
        }
        return mPacketDict[packetName];
    }
}
