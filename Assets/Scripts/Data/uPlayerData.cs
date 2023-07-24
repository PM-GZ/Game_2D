using System;
using System.Collections;
using UnityEngine;

public class uPlayerData : DataBase
{
    public enum ObjTagType
    {
        Plant,
        Food,
        Seed,

        Box,
        StorageBox,

        enemy,
    }

    private const string SAVE_PLAYER_DATA_NAME = "PlayerData";

    public GameObject role { get; private set; }
    public PlayerTotalData totalPlayerData { get; private set; }
    public PlayerData player { get => totalPlayerData.playerData; }
    public PackageData package { get => totalPlayerData.packageData; }
    public TaskData task { get => totalPlayerData.taskData; }

    private Collider2D[] collider2Ds;
    public Collider2D nearObj { get; private set; }
    public ObjTagType nearObjTag { get; private set; }

    #region override
    public override void InitData()
    {
        InitPlayerData();
    }

    public override void ClearData()
    {
        SavePlayerData();
        totalPlayerData = null;
    }
    #endregion

    #region Init
    private void InitPlayerData()
    {
        StartCoroutine(ReadPlayerData());
    }
    #endregion

    #region Get Func

    public void GetNearObj()
    {
        if (collider2Ds == null || collider2Ds.Length == 0) return;

        float nearDis = float.MaxValue;
        foreach (var coll in collider2Ds)
        {
            if (coll == null) continue;
            if(!Enum.TryParse(coll.tag, out ObjTagType tag)) continue;

            float dis = Vector3.Distance(role.transform.position, coll.transform.position);
            if (dis < nearDis)
            {
                nearDis = dis;
                nearObj = coll;
                nearObjTag = tag;
            }
        }
    }

    public int GetGoodsNum(uint id)
    {
        var goodsDict = package.goodsDict;
        if (goodsDict.TryGetValue(id, out var num))
            return num;

        return 0;
    }
    #endregion

    #region Set Func
    public void SetRoleGO(GameObject go)
    {
        role = go;
    }

    public void SetRole(uint roleId)
    {
        if(roleId == 0) return;

        player.roleData.ID = roleId;
        TABLE.Get<TableRole>().dataDict.TryGetValue(roleId, out player.roleData);
    }

    public void SetRoundObj(Collider2D[] collider2Ds)
    {
        this.collider2Ds = collider2Ds;
        GetNearObj();
    }

    public void SetPackageData(uint id, int num)
    {
        var goodsDict = package.goodsDict;
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
        if (task.Result == null)
        {
            totalPlayerData = PlayerTotalData.Default;
            yield break;
        }

        totalPlayerData = task.Result;
    }

    private void SavePlayerData()
    {
        Vector3 pos = role.transform.position;
        player.playerPos = new uVector3(pos.x, pos.y, pos.z);
        FileUtility.WriteFile(SAVE_PLAYER_DATA_NAME, FileUtility.FileType.PlayerData, totalPlayerData);
    }
    #endregion
}
