#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[PanelBind("PanelStart", PanelType.Normal)]
public class PanelStart : BasePanel
{
    public enum ToggleType
    {
        Start,
        Continue,
        Setting,
        Quit
    }

    [UiBind("Toggles")] private UiToggleGroup mToggles;




    public override void OnStart()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        mToggles.InitToggle(OnToggled);
    }

    private void OnToggled(bool isOn, int index)
    {
        if (!isOn) return;
        switch ((ToggleType)index)
        {
            case ToggleType.Start:
                OnStartClick();
                break;
            case ToggleType.Continue:
                break;
            case ToggleType.Setting:
                break;
            case ToggleType.Quit:
                OnQuit();
                break;
        }
    }

    private void OnStartClick()
    {
        var param = uScene.SceneParams.Default;
        param.sceneName = "MainScene";
        param.onLoadEnd = OnLoadSceneEnd;
        Main.Scene.SwitchScene(param);
        //Jump<PanelGameLevel>(false, false);
    }

    private void OnLoadSceneEnd()
    {
        Main.Input.SwitchInput(true, true);
        var prefab = Main.Asset.LoadAsset<GameObject>("Player");
        var player = Object.Instantiate(prefab);
        player.AddComponent<PlayerCtrl>();
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
