using System.Collections;


public class uFirstGameState : uState
{
    public override IEnumerator Enter()
    {
        Main.Ui.CloseAll();

        var loading = Main.Ui.CreatePanel<PanelLoading>();

        loading.SetProgrssStep("生成地形");
        var sceneParam = uScene.SceneParams.Default;
        sceneParam.sceneName = "MainScene";
        yield return Main.Scene.SwitchScene(sceneParam);
        loading.SetProgrssValue(0.1f);

        loading.SetProgrssStep("生成生物");
        SetPlayer();
        loading.SetProgrssValue(0.5f);


        loading.SetProgrssStep("清理世界");
        loading.SetProgrssValue(1f);

        loading.SetProgrssStep("正在进入...");
        yield return new uWaitForSeconds(0.5f);
        loading.Close();
        Main.Input.SwitchInput(true, true);
    }

    private void SetPlayer()
    {
        uAsset.GetGameObject<PlayerCtrl>();
        Main.Ui.CreatePanel<PanelPlayerUi>();
    }
}
