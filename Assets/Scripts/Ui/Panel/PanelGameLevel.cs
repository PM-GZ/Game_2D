using System;
using TMPro;
using UnityEngine.UI;

[PanelBind("PanelGameLevel", PanelType.Normal)]
public class PanelGameLevel : BasePanel
{
    public enum LevelType
    {
        Easy,
        Normal,
        Difficulty
    }

    [UiBind("Levels")] private UiToggleGroup mLevels;
    [UiBind("Easy")] private Toggle mEasy;
    [UiBind("Normal")] private Toggle mNormal;
    [UiBind("Difficulty")] private Toggle mDifficulty;
    [UiBind("LevelDes")] private TMP_Text mLevelDes;
    [UiBind("GameStart")] private UiButton mGameStart;


    public override void OnStart()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        mLevels.InitToggle(OnLevelToggle);
        mLevels.SetDefaultToggle(0);
    }

    private void OnLevelToggle(bool isOn, int index)
    {
        if (!isOn) return;
        var levelDes = Main.Data.GameData.gameLevelDesSO;
        switch ((LevelType)index)
        {
            case LevelType.Easy:
                mLevelDes.text = levelDes.easyLevelDes;
                break;
            case LevelType.Normal:
                mLevelDes.text = levelDes.normalLevelDes;
                break;
            case LevelType.Difficulty:
                mLevelDes.text = levelDes.difficultyLevelDes;
                break;
        }
    }
}
