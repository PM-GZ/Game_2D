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
        Jump<PanelGameLevel>(false, false);
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
