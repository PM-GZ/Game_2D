using System.IO;
using UnityEngine;



public class uPlayerData : BaseData
{
    public GameObject Role { get; private set; }
    public PlayerDataSO TotalPlayerData { get; private set; }
    public PlayerData Player { get; private set; }
    public PackageData Package { get; private set; }
    public TaskData Task { get; private set; }

    public override void InitData()
    {
        InitPlayerData();
    }

    private void InitPlayerData()
    {
        string path = Constent.PlayerDataPath;
        if (!File.Exists(path)) return;

        var file = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(file, TotalPlayerData);
        if(TotalPlayerData == null) return;

        Player = TotalPlayerData.playerData;
        Package = TotalPlayerData.packageData;
        Task = TotalPlayerData.taskData;
    }

    public void SavePlayerData()
    {
        var json = JsonUtility.ToJson(TotalPlayerData);
        File.WriteAllText(Constent.PlayerDataPath, json);
    }

    public override void ClearData()
    {
        TotalPlayerData = null;
        Player = default;
        Package = default;
        Task = default;
    }
}
