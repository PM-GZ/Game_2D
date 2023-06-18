using UnityEngine;


[CreateAssetMenu(fileName = "GameLevelDesCfg", menuName = "����/GameLevelDesCfg")]
public class GameLevelDesSO : ScriptableObject
{
    [Header("��")]
    [TextArea(3, 6)] public string easyLevelDes;

    [Space(8)]

    [Header("����")]
    [TextArea(3, 6)] public string normalLevelDes;

    [Space(8)]

    [Header("����")]
    [TextArea(3, 6)] public string difficultyLevelDes;
}