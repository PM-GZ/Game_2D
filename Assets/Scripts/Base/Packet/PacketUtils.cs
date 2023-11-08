using System.IO;
using System.Collections.Generic;
using System;

public class PacketUtils
{
    private class Packet
    {
        public struct BuildData
        {
            public string FileName;
            public byte[] Data;
        }

        private List<BuildData> mBuilds = new List<BuildData>();

        public void Add(string fileName, byte[] data)
        {
            var buildData = new Packet.BuildData
            {
                FileName = fileName,
                Data = data
            };
            mBuilds.Add(buildData);
        }

        public byte[] Build()
        {
            using (var steam = new MemoryStream())
            {
                using (var write = new BinaryWriter(steam))
                {
                    for (int i = 0; i < mBuilds.Count; i++)
                    {
                        var bytes = mBuilds[i];
                        write.Write(bytes.FileName);
                        write.Write(bytes.Data.Length);
                        write.Write(bytes.Data);
                    }
                    return steam.ToArray();
                }
            }
        }
    }


    private static volatile Dictionary<string, Packet> mBuildDataDict = new();
    private static Dictionary<string, byte[]> mPacketDict = new();


    public static void ClearAll()
    {
        mBuildDataDict.Clear();
        mPacketDict.Clear();
    }

    public static void AddFile(string packetName, string fileName, byte[] data)
    {
        lock (mBuildDataDict)
        {
            if (!mBuildDataDict.ContainsKey(packetName))
            {
                mBuildDataDict.Add(packetName, new Packet());
            }
            mBuildDataDict[packetName].Add(fileName, data);
        }
    }

    public static void BuildAll()
    {
        if (!Directory.Exists(Constant.BUILTIN_PATH))
        {
            Directory.CreateDirectory(Constant.BUILTIN_PATH);
        }

        foreach (var dic in mBuildDataDict)
        {
            if(File.Exists(Constant.BUILTIN_PATH + dic.Key))
            {
                File.Delete(Constant.BUILTIN_PATH + dic.Key);
            }
            File.WriteAllBytes(Constant.BUILTIN_PATH + dic.Key, dic.Value.Build());
        }
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
                throw new Exception($"未找到{packetName}包，请检查资源");
            }
        }
        return mPacketDict[packetName];
    }
}
