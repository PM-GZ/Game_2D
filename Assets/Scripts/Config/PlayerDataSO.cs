using System;
using UnityEngine;




[CreateAssetMenu(fileName = "PlayerData", menuName = "≈‰÷√/ÕÊº“ ˝æ›")]
public class PlayerDataSO : ScriptableObject
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
    public Vector3 playerPos;
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