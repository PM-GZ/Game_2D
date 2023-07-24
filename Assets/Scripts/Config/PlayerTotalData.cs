using System;
using System.Collections.Generic;

public class PlayerTotalData
{
    public PlayerData playerData;
    public PackageData packageData;
    public SceneData sceneData;
    public TaskData taskData;

    public static PlayerTotalData Default
    {
        get
        {
            PlayerTotalData data = new PlayerTotalData();
            data.playerData = PlayerData.Default;
            data.packageData = PackageData.Default;
            return data;
        }
    }
}

[Serializable]
public class PlayerData
{
    public TableRole.Data roleData;
    public string playerName;
    public ushort playerLevel;
    public ushort roleID;
    public uVector3 playerPos;

    public static PlayerData Default
    {
        get
        {
            PlayerData data = new PlayerData();
            data.roleData = default;
            data.playerName = "";
            data.playerLevel = 0;
            data.roleID = 0;
            data.playerPos = new uVector3(0, 0, 0);
            return data;
        }
    }
}

[Serializable]
public class PackageData
{
    public Dictionary<uint, int> goodsDict;

    public static PackageData Default
    {
        get
        {
            PackageData data = new PackageData();
            data.goodsDict = new();
            return data;
        }
    }
}

[Serializable]
public class SceneData
{
    public ushort sceneIndex;
    public string sceneName;

    public static SceneData Default
    {
        get
        {
            SceneData data = new SceneData();
            data.sceneIndex = 0;
            data.sceneName = "";
            return data;
        }
    }
}

[Serializable]
public class TaskData
{

}

[Serializable]
public struct uVector3
{
    public float x;
    public float y;
    public float z;

    public uVector3(float v1, float v2, float v3) : this()
    {
        x = v1;
        y = v2;
        z = v3;
    }
}