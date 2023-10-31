using System.Collections.Generic;
using System.IO;
using System.Text;

public class PacketEditor
{
    static volatile Dictionary<string, Packet> mPackets = new Dictionary<string, Packet>();

    static byte[] mKey;
    static byte[] mIv;
    static List<string> mBuiltinPackets = new List<string>();
    static UTF8Encoding mEncoding = new UTF8Encoding(false);

    public static void Init(List<string> builtin_packets)
    {
        mBuiltinPackets = builtin_packets;
    }

    public static void Init(byte[] key, byte[] iv, List<string> builtin_packets)
    {
        mKey = key;
        mIv = iv;
        mBuiltinPackets = builtin_packets;
    }

    public static bool Load(string packet_path)
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
        mPackets[packet_name] = Packet.LoadFromBytes(File.ReadAllBytes(packet_path), mKey, mIv);
        return true;
    }

    public static void Load(byte[] bytes, string packet_name)
    {
        if (mPackets.TryGetValue(packet_name, out var old_packet))
        {
            old_packet.Dispose();
        }
        mPackets[packet_name] = Packet.LoadFromBytes(bytes, mKey, mIv);
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

    public static void LoadPacket(byte[] data, string name)
    {
        var packet = Packet.LoadFromBytes(data, mKey, mIv);
        mPackets[name] = packet;
    }

    public static bool IsPacketLoaded(string package_name)
    {
        return mPackets.ContainsKey(package_name);
    }

    public static byte[] GetFile(string package_name, string file_name)
    {
        var data = EditorLoader(package_name, file_name);
        return data;
    }

    private static byte[] EditorLoader(string package_name, string file_name)
    {
        if (mBuiltinPackets.Contains(package_name))
        {
            if (!IsPacketLoaded(package_name))
            {
                if (!Load("Builtin/" + package_name))
                {
                    return null;
                }
            }
            return GetFileFromPacket(package_name, file_name);
        }
        else
        {
            var file_extension = Path.GetExtension(file_name);
            var files = Directory.GetFiles("Assets/EditorData/", $"*{file_extension}", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (string.Equals(Path.GetFileName(file), file_name))
                {
                    return File.ReadAllBytes(file);
                }
            }
            return null;
        }
    }

    public static byte[] GetFileFromPacket(string packet_name, string file_name)
    {
        if (mPackets.TryGetValue(packet_name, out var packet))
        {
            return packet.GetFile(file_name);
        }
        else
        {
            return null;
        }
    }

    public static string GetTextFile(string packet_name, string file_name)
    {
        var bytes = GetFile(packet_name, file_name);
        if (bytes != null)
        {
            return mEncoding.GetString(GetFile(packet_name, file_name));
        }
        else
        {
            return string.Empty;
        }
    }

    public static void AddFile(string packet_name, string file_name, byte[] data)
    {
        if (mPackets.TryGetValue(packet_name, out var packet) == false)
        {
            throw new System.Exception("未找到数据包 可能数据包未加载或者创建: " + packet_name);
        }
        packet.AddFile(file_name, data);
    }

    public static void BuildAll(string client_path, string server_path)
    {
        if (client_path != null)
        {
            if (!Directory.Exists(client_path))
            {
                Directory.CreateDirectory(client_path);
            }
        }
        if (server_path != null)
        {
            if (!Directory.Exists(server_path))
            {
                Directory.CreateDirectory(server_path);
            }
        }
        foreach (var pair in mPackets)
        {
            var bytes = pair.Value.Build(null, null);
            File.WriteAllBytesAsync(client_path + pair.Key, bytes);
            File.WriteAllBytesAsync(server_path + pair.Key, bytes);
        }
    }

    public static void BuildAll(string encryPath, string noEncryPath, byte[] key, byte[] iv)
    {
        if (encryPath != null)
        {
            if (!Directory.Exists(encryPath))
            {
                Directory.CreateDirectory(encryPath);
            }
            foreach (var pair in mPackets)
            {
                File.WriteAllBytesAsync(encryPath + pair.Key, pair.Value.Build(key, iv));
            }
        }
        if (noEncryPath != null)
        {
            if (!Directory.Exists(noEncryPath))
            {
                Directory.CreateDirectory(noEncryPath);
            }
            foreach (var pair in mPackets)
            {
                File.WriteAllBytesAsync(noEncryPath + pair.Key, pair.Value.Build(null, null));
            }
        }
    }

    public static Packet Create(string packet_name)
    {
        lock (mPackets)
        {
            if (!mPackets.TryGetValue(packet_name, out var packet))
            {
                packet = new Packet();
                mPackets.Add(packet_name, packet);
            }
            return packet;
        }
    }
}
