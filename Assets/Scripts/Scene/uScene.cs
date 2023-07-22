using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class uScene : BaseObject
{
    public struct SceneParams
    {
        public string sceneName;
        public LoadSceneMode mode;
        public bool activeOnLoad;
        public bool showProgress;
        public Action onLoadStart;
        public Action onLoadEnd;

        public static SceneParams Default 
        {
            get
            {
                var param = new SceneParams();
                param.sceneName = string.Empty;
                param.mode = LoadSceneMode.Single;
                param.activeOnLoad = true;
                param.showProgress = true;
                param.onLoadStart = null;
                param.onLoadEnd = null;
                return param;
            }
        }
    }

    private SceneParams _SceneParams;
    private AsyncOperationHandle _SceneAsync;
    private IEnumerator _SceneCoroutine;
    private bool _ShowProgress;

    public override void Init()
    {
    }

    public void FadeScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);
        Main.Ui.CloseAll();

        _SceneParams = param;
        _SceneCoroutine = ExcuteFadeScene();
        StartCoroutine(_SceneCoroutine);
    }

    private IEnumerator ExcuteFadeScene()
    {
        var fade = Main.Ui.CreatePanel<PanelFade>();
        fade.StartFade();

        _SceneParams.onLoadStart?.Invoke();
        _SceneAsync = uAsset.LoadSceneAsync(_SceneParams.sceneName, _SceneParams.mode, _SceneParams.activeOnLoad);
        while (!_SceneAsync.IsDone)
        {
            yield return null;
        }
        _SceneAsync = default;
        _SceneCoroutine = null;

        _SceneParams.onLoadEnd?.Invoke();
        fade.EndFade();
    }

    public void SwitchScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);
        Main.Ui.CloseAll();

        _SceneParams = param;
        _ShowProgress = param.showProgress;
        _SceneCoroutine = ExcuteSwitchScene();
        StartCoroutine(_SceneCoroutine);
    }

    private IEnumerator ExcuteSwitchScene()
    {
        _SceneParams.onLoadStart?.Invoke();

        var loading = Main.Ui.CreatePanel<PanelLoading>();
        _SceneAsync = uAsset.LoadSceneAsync(_SceneParams.sceneName, _SceneParams.mode, _SceneParams.activeOnLoad);
        while (!_SceneAsync.IsDone)
        {
            loading.SetProgrssValue(_SceneAsync.PercentComplete);
            yield return null;
        }

        loading.Close();
        _SceneAsync = default;
        _SceneCoroutine = null;

        _SceneParams.onLoadEnd?.Invoke();
    }
}
