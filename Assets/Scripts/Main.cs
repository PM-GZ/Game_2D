using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif



[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
    private static bool _initFlag = false;

    public static uUi Ui { get; private set; }
    public static GameInput Input { get; private set; }
    public static uData Data { get; private set; }
    public static uScene Scene { get; private set; }

    private void Start()
    {
        if (_initFlag)
        {
            ClearScene();
            return;
        }
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        ClearScene();
        yield return null;
        InitCanvas();
        yield return null;
        InitManager();
        Ui.CreatePanel<PanelStart>();

        _initFlag = true;
    }

    private void ClearScene()
    {
        var scene = SceneManager.GetActiveScene();
        var gos = scene.GetRootGameObjects();
        for (int i = gos.Length - 1; i >= 0; i--)
        {
            Destroy(gos[i]);
        }
    }

    private void InitManager()
    {
        Input = new();
        Data = new();
        Scene = new();
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
        BaseObject.Update();
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
}