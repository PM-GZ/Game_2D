using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



public static class FileUtility
{
    public enum FileType
    {
        PlayerData,
    }

    private static void Write(string fileName, FileType fileType, byte[] data)
    {
        string path = $"{Constent.FILE_WRITE_PATH}/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllBytes($"{Constent.FILE_WRITE_PATH}/{fileName}.bat", data);
    }

    private static byte[] GetByteData(object obj)
    {
        using MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, obj);

        return ms.GetBuffer();
    }

    public static void WriteFile(string fileName, FileType fileType, object obj)
    {
        var data = GetByteData(obj);
        Write(fileName, fileType, data);
    }
}
