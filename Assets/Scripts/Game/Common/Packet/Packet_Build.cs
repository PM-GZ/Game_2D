using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class Packet
{
    public Packet() { }

    struct BuildFileInfo
    {
        public uint Id;
        public string FileName;
        public byte[] Data;

        public BuildFileInfo(string file, byte[] data)
        {
            FileName = Path.GetFileName(file);
            Id = Global.GetStr2Hash(FileName);
            Data = data;
        }
    }

    private volatile List<BuildFileInfo> mBuildFiles = new List<BuildFileInfo>();

    public void AddFiles(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            AddFile(files[i]);
        }

    }

    public void AddFile(string file)
    {
        if (File.Exists(file))
        {
            mBuildFiles.Add(new BuildFileInfo(file, File.ReadAllBytes(file)));
        }
    }

    public void AddFile(string file_name, byte[] data)
    {
        lock (mBuildFiles)
        {
            mBuildFiles.Add(new BuildFileInfo()
            {
                Id = Global.GetStr2Hash(file_name),
                FileName = file_name,
                Data = data
            });
        }
    }
    public bool CheckError()
    {
        Dictionary<string, uint> mNameFiles = new Dictionary<string, uint>();
        Dictionary<uint, string> mIdFiles = new Dictionary<uint, string>();
        bool result = false;
        foreach (var file in mBuildFiles)
        {
            if (mNameFiles.ContainsKey(file.FileName))
            {
                Debug.LogError($"重复文件_1 filename = {file.FileName}， Id = {file.Id}");
                Debug.LogError($"重复文件_2 filename = {file.FileName}， Id = {mNameFiles[file.FileName]}");
                result = true;
                continue;
            }
            if (mIdFiles.ContainsKey(file.Id))
            {
                Debug.LogError($"重复文件_1 filename = {file.FileName}， Id = {file.Id}");
                Debug.LogError($"重复文件_2 filename = {mIdFiles[file.Id]}， Id = {file.Id}");
                result = true;
                continue;
            }
            mNameFiles.Add(file.FileName, file.Id);
            mIdFiles.Add(file.Id, file.FileName);
        }
        return result;
    }
    public byte[] Build(byte[] key, byte[] iv)
    {
        if (key == null)
        {
            Encrypted = false;
        }
        else
        {
            Encrypted = true;
        }
        byte[] data = null;
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                WritePacketInfo(writer);
                WritePacketFiles(writer, key, iv);
            }
            data = stream.ToArray();
        }
        return data;
    }

    void WritePacketInfo(BinaryWriter writer)
    {
        writer.Write(Encrypted);
    }

    void WritePacketFiles(BinaryWriter writer, byte[] key, byte[] iv)
    {

        writer.Write(mBuildFiles.Count);
        for (var i = 0; i < mBuildFiles.Count; i++)
        {
            writer.Write(mBuildFiles[i].Id);

            if (Encrypted)
            {
                byte[] bytes = mBuildFiles[i].Data;
                writer.Write(bytes.Length);
                var encrypt_data = AesUtils.Encrypt(bytes, key, iv);
                writer.Write(encrypt_data.Length);
                writer.Write(encrypt_data);
            }
            else
            {
                writer.Write(mBuildFiles[i].Data.Length);

                writer.Write(mBuildFiles[i].Data);
            }
        }
        Debug.Log($"WRITE PACKET FILES {mBuildFiles.Count}. ENCRYPT {Encrypted}");
    }
}