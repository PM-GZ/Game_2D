using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "GameSaveCfg", menuName = "����/��Ϸ��������")]
public class GameSaveSO : ScriptableObject
{
    public GameSaveData gameSaveData;
}

[Serializable]
public struct GameSaveData
{

}