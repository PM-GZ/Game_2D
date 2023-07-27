using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class uScene : BaseObject
{
    public struct SceneParams
    {
        public string sceneName;
        public LoadSceneMode mode;
        public bool activeOnLoad;
        public Action onLoadStart;
        public Action onLoadEnd;
        public Action<SceneInstance> onComplete;

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
    private SceneParams _SceneParams;
    private AsyncOperationHandle<SceneInstance> _SceneAsync;
    private IEnumerator _SceneCoroutine;

    public event Action onSwitchScene;

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

        onSwitchScene?.Invoke();

        _SceneParams.onLoadStart?.Invoke();
        _SceneAsync = uAsset.LoadSceneAsync(_SceneParams.sceneName, _SceneParams.mode, _SceneParams.activeOnLoad);
        while (!_SceneAsync.IsDone)
        {
            yield return null;
        }
        curScene = _SceneAsync.Result.Scene;
        fade.EndFade();
        _SceneParams.onLoadEnd?.Invoke();
        _SceneParams.onComplete?.Invoke(_SceneAsync.Result);

        _SceneAsync = default;
        _SceneCoroutine = null;

    }

    public void SwitchScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);
        Main.Ui.CloseAll();

        _SceneParams = param;
        _SceneCoroutine = ExcuteSwitchScene();
        StartCoroutine(_SceneCoroutine);
    }

    private IEnumerator ExcuteSwitchScene()
    {
        _SceneParams.onLoadStart?.Invoke();
        onSwitchScene?.Invoke();

        var loading = Main.Ui.CreatePanel<PanelLoading>();
        yield return uAsset.LoadSceneAsync(_SceneParams.sceneName, _SceneParams.mode, _SceneParams.activeOnLoad);
        curScene = _SceneAsync.Result.Scene;

        loading.SetProgrssValue(0.1f);
        Main.Data.Map.InitMapGoodsData();

        loading.SetProgrssValue(0.2f);
        Main.Data.Map.InitRandomGoods();

        loading.SetProgrssValue(1f);

        _SceneParams.onLoadEnd?.Invoke();
        _SceneParams.onComplete?.Invoke(_SceneAsync.Result);

        loading.Close();
        _SceneAsync = default;
        _SceneCoroutine = null;
    }
}
