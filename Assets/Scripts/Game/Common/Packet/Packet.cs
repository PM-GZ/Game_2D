using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public partial class Packet
{
    struct ReadFileInfo
    {
        public uint Id;
        public int OriginLength;
        public int Length;
        public long Position;
    }

    #region STATIC
    public static bool EnableDevelopmentMode = false;
    static Dictionary<string, Packet> mPackets = new Dictionary<string, Packet>();
    static UTF8Encoding mEncoding = new UTF8Encoding(false);

    public static bool Load(string packet_path)
    {
        return Load(packet_path, null, null);
    }

    public static bool Load(string packet_path, byte[] key, byte[] iv)
    {
        if (File.Exists(packet_path) == false)
        {
            return false;
        }
        var packet_name = Path.GetFileName(packet_path);
        if (mPackets.TryGetValue(packet_name, out var old_packet))
        {
            old_packet.Dispose();
        }
        mPackets[packet_name] = LoadFromStream(File.OpenRead(packet_path), key, iv);
        return true;
    }

    public static void Load(Stream stream, string packet_name)
    {
        Load(stream, packet_name, null, null);
    }

    public static void Load(Stream stream, string packet_name, byte[] key, byte[] iv)
    {
        if (mPackets.TryGetValue(packet_name, out var old_packet))
        {
            old_packet.Dispose();
        }
        mPackets[packet_name] = LoadFromStream(stream, key, iv);
    }

    public static void Load(byte[] bytes, string packet_name, byte[] key, byte[] iv)
    {
        if (mPackets.TryGetValue(packet_name, out var old_packet))
        {
            old_packet.Dispose();
        }
        mPackets[packet_name] = LoadFromBytes(bytes, key, iv);
    }

    public static void Unload(string packet_name)
    {
        if (mPackets.TryGetValue(packet_name, out var packet))
        {
            packet.Dispose();
            mPackets.Remove(packet_name);
        }
    }

    public static void UnloadAll()
    {
        foreach (var pair in mPackets)
        {
            pair.Value.Dispose();
        }
        mPackets.Clear();
    }

    public static Packet LoadFromStream(Stream stream, byte[] key, byte[] iv)
    {
        var packet = new Packet();
        packet.mReader = new BinaryReader(stream);
        packet.mKey = key;
        packet.mIv = iv;
        packet.InitRead();
        return packet;
    }

    public static Packet LoadFromBytes(byte[] bytes, byte[] key, byte[] iv)
    {
        return LoadFromStream(new MemoryStream(bytes), key, iv);
    }

    public static byte[] GetFile(string packet_name, string file_name)
    {
        if (mPackets.TryGetValue(packet_name, out var packet))
        {
            return packet.GetFile(file_name);
        }
        return null;
    }

    public static string GetTextFile(string packet_name, string file_name)
    {
        if (mPackets.TryGetValue(packet_name, out var packet))
        {
            return packet.GetTextFile(file_name);
        }
        return string.Empty;
    }

    #endregion

    public bool Encrypted { get; private set; }
    public bool EnableCache { get; private set; }
    private byte[] mKey;
    private byte[] mIv;
    private BinaryReader mReader;
    Dictionary<uint, ReadFileInfo> mReadFiles;
    Dictionary<uint, byte[]> mCachedFiles;

    public bool TryGetTextFile(string file_path, out string text)
    {
        var file_id = Global.GetStr2Hash(Path.GetFileName(file_path));
        return TryGetTextFile(file_id, out text);
    }

    public bool TryGetTextFile(uint file_id, out string text)
    {
        text = string.Empty;
        if (TryGetFile(file_id, out var bytes))
        {
            text = mEncoding.GetString(bytes);
            return true;
        }
        return false;
    }
    public string GetTextFile(string file_path)
    {
        if (TryGetTextFile(file_path, out var text))
        {
            return text;
        }
        return string.Empty;
    }

    public bool TryGetFile(string file_path, out byte[] bytes)
    {
        var file_id = Global.GetStr2Hash(Path.GetFileName(file_path));
        return TryGetFile(file_id, out bytes);
    }

    public byte[] GetFile(string file_path)
    {
        if (TryGetFile(file_path, out var bytes))
        {
            return bytes;
        }
        return null;
    }

    public bool TryGetFile(uint file_id, out byte[] bytes)
    {
        bytes = null;
        if (mReadFiles.TryGetValue(file_id, out var info))
        {
            mReader.BaseStream.Position = info.Position;
            if (!EnableCache || !mCachedFiles.TryGetValue(file_id, out bytes))
            {
                if (Encrypted)
                {
                    if (info.OriginLength <= 0)
                    {
                        bytes = new byte[0];
                    }
                    else
                    {
                        bytes = AesUtils.Decrypt(mReader.ReadBytes(info.Length), info.OriginLength, mKey, mIv);
                    }
                    if (EnableCache)
                    {
                        mCachedFiles[file_id] = bytes;
                    }
                }
                else
                {
                    bytes = mReader.ReadBytes(info.Length);
                    if (EnableCache)
                    {
                        mCachedFiles[file_id] = bytes;
                    }
                }
            }
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        if (mReader != null)
        {
            mReader.Dispose();
            mReader = null;
        }
        if (mReadFiles != null)
        {
            mReadFiles.Clear();
            mReadFiles = null;
        }
        if (mCachedFiles != null)
        {
            mCachedFiles.Clear();
            mCachedFiles = null;
        }
    }

    #region READ

    void InitRead()
    {
        ReadPacketInfo();
        ReadPacketFiles();
    }

    void ReadPacketInfo()
    {
        Encrypted = mReader.ReadBoolean();
    }

    void ReadPacketFiles()
    {
        var file_count = mReader.ReadInt32();
        if (file_count <= 0)
        {
            Debug.LogError($"packet load error. file count not right :{file_count}");
            return;
        }
        mReadFiles = new Dictionary<uint, ReadFileInfo>(file_count);
        mCachedFiles = new Dictionary<uint, byte[]>(file_count);
        for (var i = 0; i < file_count; i++)
        {
            var file_id = mReader.ReadUInt32();
            var origin_file_length = mReader.ReadInt32();
            if (Encrypted)
            {
                var file_length = mReader.ReadInt32();
                var file_position = mReader.BaseStream.Position;

                mReadFiles.Add(file_id, new ReadFileInfo
                {
                    Id = file_id,
                    OriginLength = origin_file_length,
                    Length = file_length,
                    Position = file_position,
                });
                mReader.BaseStream.Position = mReader.BaseStream.Position + file_length;
            }
            else
            {
                var file_position = mReader.BaseStream.Position;
                mReadFiles.Add(file_id, new ReadFileInfo
                {
                    Id = file_id,
                    Length = origin_file_length,
                    Position = file_position,
                });
                mReader.BaseStream.Position = mReader.BaseStream.Position + origin_file_length;
            }

        }
    }
    #endregion
}
