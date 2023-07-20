



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
        _goBackMain.onClick = OnGoBackMain;
        _goBackMenu.onClick = OnGoBackMenu;
    }

    private void OnGoBackMain()
    {
        var param = uScene.SceneParams.Default;
        param.sceneName = "LaunchScene";
        param.onLoadEnd = () => { Main.Ui.CreatePanel<PanelStart>(); };
        Main.Scene.SwitchScene(param);
    }

    private void OnGoBackMenu()
    {
        Main.QuitGame();
    }
}
