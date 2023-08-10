using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine;

public class uScene : BaseObject
{
    public struct SceneParams
    {
        public string sceneName;
        public LoadSceneMode mode;
        public bool activeOnLoad;
        public Action onLoadStart;
        public Action onLoadEnd;
        public Action<Scene> onComplete;

        public static SceneParams Default
        {
            get
            {
                var param = new SceneParams();
                param.sceneName = string.Empty;
                param.mode = LoadSceneMode.Single;
                param.activeOnLoad = true;
                param.onLoadStart = null;
                param.onLoadEnd = null;
                param.onComplete = null;
                return param;
            }
        }
    }

    public Scene curScene { get; private set; }
    private SceneParams _sceneParams;
    private AsyncOperationHandle<SceneInstance> _sceneAsync;
    private IEnumerator _sceneCoroutine;

    public event Action onSwitchScene;


    public void FadeScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);
        Main.Ui.CloseAll();

        _sceneParams = param;
        _sceneCoroutine = ExcuteFadeScene();
        StartCoroutine(_sceneCoroutine);
    }

    private IEnumerator ExcuteFadeScene()
    {
        var fade = Main.Ui.CreatePanel<PanelFade>();
        fade.StartFade();

        onSwitchScene?.Invoke();

        _sceneParams.onLoadStart?.Invoke();

        yield return ExcuteLoadScene();

        EndDispose();

        fade.EndFade();
    }

    public void SwitchScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);
        Main.Ui.CloseAll();

        _sceneParams = param;
        _sceneCoroutine = ExcuteSwitchScene();
        StartCoroutine(_sceneCoroutine);
    }

    private IEnumerator ExcuteSwitchScene()
    {
        _sceneParams.onLoadStart?.Invoke();
        onSwitchScene?.Invoke();

        var loading = Main.Ui.CreatePanel<PanelLoading>();

        yield return ExcuteLoadScene();

        //loading.SetProgrssValue(0.1f);
        //Main.Data.Map.InitMapGoodsData();

        //loading.SetProgrssValue(0.2f);
        //Main.Data.Map.InitRandomGoods();

        EndDispose();

        loading.SetProgrssValue(1f);
        yield return new WaitForSeconds(0.5f);
        loading.Close();
    }

    private IEnumerator ExcuteLoadScene()
    {
        _sceneAsync = uAsset.LoadSceneAsync(_sceneParams.sceneName, _sceneParams.mode);
        yield return _sceneAsync;
        curScene = _sceneAsync.Result.Scene;
    }

    private void EndDispose()
    {
        _sceneParams.onLoadEnd?.Invoke();
        _sceneParams.onComplete?.Invoke(curScene);

        _sceneAsync = default;
        _sceneCoroutine = null;
    }
}
