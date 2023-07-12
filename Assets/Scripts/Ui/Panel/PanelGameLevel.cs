using TMPro;
using UnityEngine;
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

    [UiBind("Levels")] private UiToggleGroup _Levels;
    [UiBind("Easy")] private Toggle _Easy;
    [UiBind("Normal")] private Toggle _Normal;
    [UiBind("Difficulty")] private Toggle _Difficulty;
    [UiBind("LevelDes")] private TMP_Text _LevelDes;
    [UiBind("StartGame")] private UiButton _StartGame;


    public override void OnStart()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        _Levels.InitToggle(OnLevelToggle);
        _Levels.SetDefaultToggle(0);
        _StartGame.onClick = OnGameStartClick;
    }

    private void OnLevelToggle(bool isOn, int index)
    {
        if (!isOn) return;
        var levelDes = Main.Data.GameData.gameLevelDesSO;
        switch ((LevelType)index)
        {
            case LevelType.Easy:
                _LevelDes.text = levelDes.easyLevelDes;
                break;
            case LevelType.Normal:
                _LevelDes.text = levelDes.normalLevelDes;
                break;
            case LevelType.Difficulty:
                _LevelDes.text = levelDes.difficultyLevelDes;
                break;
        }
    }

    private void OnGameStartClick()
    {
        var param = uScene.SceneParams.Default;
        param.sceneName = "MainScene";
        param.onLoadEnd = OnLoadSceneEnd;
        Main.Scene.SwitchScene(param);
    }

    private void OnLoadSceneEnd()
    {
        Main.Input.SwitchInput(true, true);
        var prefab = Main.Asset.LoadAsset<GameObject>("Cat");
        var player = Object.Instantiate(prefab);
        player.AddComponent<PlayerCtrl>();
    }
}
