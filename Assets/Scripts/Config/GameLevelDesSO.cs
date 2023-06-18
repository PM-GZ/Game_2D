using UnityEngine;


[CreateAssetMenu(fileName = "GameLevelDesCfg", menuName = "配置/GameLevelDesCfg")]
public class GameLevelDesSO : ScriptableObject
{
    [Header("简单")]
    [TextArea(3, 6)] public string easyLevelDes;

    [Space(8)]

    [Header("正常")]
    [TextArea(3, 6)] public string normalLevelDes;

    [Space(8)]

    [Header("困难")]
    [TextArea(3, 6)] public string difficultyLevelDes;
}