using System.Collections;



public class uBackMainMnueState : uState
{
    public override IEnumerator Enter()
    {
        uAsset.UnloadAll();
        Main.Ui.CloseAll();

        var loading = Main.Ui.CreatePanel<PanelLoading>();

        var sceneParam = uScene.SceneParams.Default;
        sceneParam.sceneName = "LaunchScene";
        yield return Main.Scene.SwitchScene(sceneParam);
        loading.SetProgrssValue(0.1f);



        Main.Ui.CreatePanel<PanelStart>();
        loading.SetProgrssValue(1f);

        yield return new uWaitForSeconds(0.5f);
        loading.Close();
        Main.Input.SwitchInput(true, false);
    }
}
