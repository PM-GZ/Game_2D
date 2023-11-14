





[PanelBind("PanelMainMenu")]
public class PanelMainMenu : BasePanel
{
    [UiBind("Continue")] private UiButton mContinue;
    [UiBind("Load")] private UiButton mLoad;
    [UiBind("NewGame")] private UiButton mNewGame;

    public override void OnStart()
    {
        base.OnStart();
    }
}
