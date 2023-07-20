using System.Collections;
using UnityEngine;



public class uPlayerData : DataBase
{
    private const string SAVE_PLAYER_DATA_NAME = "PlayerData";

    public GameObject role { get; private set; }
    public PlayerTotalData totalPlayerData { get; private set; }
    public PlayerData player { get => totalPlayerData.playerData; }
    public PackageData package { get => totalPlayerData.packageData; }
    public TaskData task { get => totalPlayerData.taskData; }

    public override void InitData()
    {
        InitPlayerData();
    }

    public override void ClearData()
    {
        SavePlayerData();
        totalPlayerData = null;
    }

    private void InitPlayerData()
    {
        StartCoroutine(GetPlayerData());
    }

    private IEnumerator GetPlayerData()
    {
        var task = FileUtility.GetFileAsync<PlayerTotalData>(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        if (task.Result == null) yield break;

        totalPlayerData = task.Result;
    }

    private void SavePlayerData()
    {
        FileUtility.WriteFile(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData, totalPlayerData);
    }
}
