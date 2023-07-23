using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public static class FileUtility
{
    public enum FileType
    {
        PlayerData,
        MapData,
    }

    private static void Write(string fileName, FileType fileType, byte[] data)
    {
        string path = $"{Constent.FILE_WRITE_PATH}/{fileType}";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllBytes($"{path}/{fileName}.d", data);
    }

    private static byte[] GetByteData(object obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        return Encoding.UTF8.GetBytes(json);
    }

    public static void WriteFile(string fileName, FileType fileType, object obj)
    {
        if (obj == null) return;
        var data = GetByteData(obj);
        Write(fileName, fileType, data);
    }

    public static async Task<T> GetFileAsync<T>(string fileName, FileType fileType)
    {
        string path = $"{Constent.FILE_WRITE_PATH}/{fileType}/{fileName}.d";
        if (!File.Exists(path)) return default;

        var task = File.ReadAllBytes(path);
        var data = await Task.Run(() =>
        {
            string json = Encoding.UTF8.GetString(task);
            return JsonConvert.DeserializeObject<T>(json);
        });
        return data;
    }
}
