using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif



[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
    public static Main main { get; private set; }
    private static bool _initFlag = false;

    public static uUi Ui { get; private set; }
    public static GameInput Input { get; private set; }
    public static uData Data { get; private set; }
    public static uScene Scene { get; private set; }


    private  uState _curState;
    private Coroutine _stateCoroutine;

    private void Start()
    {
        if (_initFlag)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        InitCanvas();
        InitManager();
        Ui.CreatePanel<PanelStart>();

        _initFlag = true;
    }

    private void InitManager()
    {
        Input = new();
        Scene = new();
        Data = new();
        Ui = new();
    }

    private void InitCanvas()
    {
        var canvas = Resources.Load<GameObject>("Canvas");
        canvas = Instantiate(canvas, null, false);
        canvas.name = "Canvas";
        DontDestroyOnLoad(canvas);
        var uiCam = canvas.GetComponentInChildren<Camera>();
        var camData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        camData.cameraStack.Add(uiCam);
    }

    private void Update()
    {
        _curState?.Update();
        BaseObject.UpdateAll();
        GameBehaviour.Update();
    }

    private void FixedUpdate()
    {
        GameBehaviour.FixedUpdate();
    }

    private void LateUpdate()
    {
        GameBehaviour.LateUpdate();
    }

    private void OnApplicationFocus(bool focus)
    {

    }

    private void OnApplicationPause(bool pause)
    {

    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        _curState?.Quit();
        _curState = null;

        Data.OnDestroy();
        Input.Dispose();
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
# else
        Application.Quit();
#endif
    }


    public void LoadState(uState state)
    {
        _curState?.Quit();
        _curState = state;
        _stateCoroutine = StartCoroutine(_curState.Enter());
    }
}