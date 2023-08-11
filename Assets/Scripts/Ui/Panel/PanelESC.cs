



[PanelBind("PanelESC", PanelType.Top, true)]
public class PanelESC : PanelBase
{
    [UiBind("GoBackMain")] private UiButton _goBackMain;
    [UiBind("GoBackMenu")] private UiButton _goBackMenu;




    public override void OnStart()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        _goBackMain.onClick = OnBackMainMenu;
        _goBackMenu.onClick = OnBackMainInterface;
    }

    private void OnBackMainMenu()
    {
        Main.main.LoadState(new uBackMainMnueState());
    }

    private void OnBackMainInterface()
    {
        Main.QuitGame();
    }
}
