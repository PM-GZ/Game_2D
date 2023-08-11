using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


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

    #region Fade
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
    #endregion

    #region Switch & Progrss
    public IEnumerator SwitchScene(SceneParams param)
    {
        Main.Input.SwitchInput(false, false);

        _sceneParams = param;

        _sceneParams.onLoadStart?.Invoke();
        onSwitchScene?.Invoke();

        yield return ExcuteLoadScene();

        EndDispose();
    }
    #endregion

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
