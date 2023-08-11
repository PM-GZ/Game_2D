using System.Collections;


public class uFirstGameState : uState
{
    public override IEnumerator Enter()
    {
        Main.Ui.CloseAll();

        var loading = Main.Ui.CreatePanel<PanelLoading>();

        loading.SetProgrssStep("���ɵ���");
        var sceneParam = uScene.SceneParams.Default;
        sceneParam.sceneName = "MainScene";
        yield return Main.Scene.SwitchScene(sceneParam);
        loading.SetProgrssValue(0.1f);

        loading.SetProgrssStep("��������");
        SetPlayer();
        loading.SetProgrssValue(0.5f);


        loading.SetProgrssStep("��������");
        loading.SetProgrssValue(1f);

        loading.SetProgrssStep("���ڽ���...");
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
