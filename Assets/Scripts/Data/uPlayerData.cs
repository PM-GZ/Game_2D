using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class uPlayerData : DataBase
{
    private const string SAVE_PLAYER_DATA_NAME = "PlayerData";

    public GameObject role { get; private set; }
    public PlayerTotalData totalPlayerData { get; private set; }
    public PlayerData player { get => totalPlayerData.playerData; }
    public PackageData package { get => totalPlayerData.packageData; }
    public TaskData task { get => totalPlayerData.taskData; }




    #region override
    public override void InitData()
    {
        InitPlayerData();
    }

    public override void ClearData()
    {
        SavePlayerData();
        totalPlayerData = default;
    }
    #endregion

    #region Init
    private void InitPlayerData()
    {
        StartCoroutine(ReadPlayerData());
    }
    #endregion

    #region Get Func
    public int GetGoodsNum(uint id)
    {
        var goodsDict = package.goodsDict;
        goodsDict ??= new Dictionary<uint, int>();
        if (goodsDict.TryGetValue(id, out var num))
            return num;

        return 0;
    }
    #endregion

    #region Set Func
    public void SetPackageData(uint id, int num)
    {
        var goodsDict = package.goodsDict;
        goodsDict ??= new Dictionary<uint, int>();
        if (!goodsDict.TryAdd(id, num))
        {
            goodsDict[id] += num;
        }
    }
    #endregion

    #region 数据保存/读取
    private IEnumerator ReadPlayerData()
    {
        var task = FileUtility.GetFileAsync<PlayerTotalData>(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        if (task.Result.Equals(null))
        {
            totalPlayerData = new();
            yield break;
        }

        totalPlayerData = task.Result;
    }

    private void SavePlayerData()
    {
        FileUtility.WriteFile(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData, totalPlayerData);
    }
    #endregion
}
