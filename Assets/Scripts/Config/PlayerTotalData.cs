using System;




public class PlayerTotalData
{
    public PlayerData playerData;
    public PackageData packageData;
    public SceneData sceneData;
    public TaskData taskData;
}

[Serializable]
public struct PlayerData
{
    public string playerName;
    public ushort playerLevel;
    public ushort roleID;
    public ushort sceneIndex;
    public uVector3 playerPos;
}

[Serializable]
public struct SceneData
{
    public ushort sceneIndex;
    public string sceneName;
}

[Serializable]
public struct PackageData
{

}

[Serializable]
public struct TaskData
{

}

[Serializable]
public struct uVector3
{
    public float x;
    public float y;
    public float z;
}