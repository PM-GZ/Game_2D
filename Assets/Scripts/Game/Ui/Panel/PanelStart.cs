using UnityEngine;


[PanelBind("PanelStart", PanelType.Normal)]
public class PanelStart : PanelBase
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
        Main.main.LoadState(new uFirstGameState());
    }

    private void OnQuit()
    {
        Main.QuitGame();
    }
}
