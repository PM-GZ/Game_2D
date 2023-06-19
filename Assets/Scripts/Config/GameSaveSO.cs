using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "GameSaveCfg", menuName = "配置/游戏保存数据")]
public class GameSaveSO : ScriptableObject
{
    public GameSaveData gameSaveData;
}

[Serializable]
public struct GameSaveData
{

}