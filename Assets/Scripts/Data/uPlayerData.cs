using System.IO;
using UnityEngine;



public class uPlayerData : BaseData
{
    private const string SAVE_PLAYER_DATA_NAME = "PlayerData";

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

    private void SavePlayerData()
    {
        FileUtility.WriteFile(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData, TotalPlayerData);
    }

    public override void ClearData()
    {
        SavePlayerData();
        TotalPlayerData = null;
        Player = default;
        Package = default;
        Task = default;
    }
}
