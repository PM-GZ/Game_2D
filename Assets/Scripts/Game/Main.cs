using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
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
    public static uScene Scene { get; private set; }


    private uState _curState;
    private Coroutine _stateCoroutine;
    private List<string> mCatalogs;
    private long mTotalDownloadSize;
    Assembly mHotUpdateAss;

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

        _initFlag = true;
    }

    private void InitManager()
    {
        Input = new();
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

    #region HotUpdate
    private async void CheckAssetsUpdate()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        await checkHandle.Task;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            mCatalogs = checkHandle.Result;
            GetDownloadAssets();
        }
        else
        {
            //TODO 检查资源更新失败
        }
        Addressables.Release(checkHandle);
    }

    private async void GetDownloadAssets()
    {
        if (mCatalogs == null || mCatalogs.Count == 0)
        {
            EnterGame();
            return;
        }

        var catalogsHandle = Addressables.UpdateCatalogs(true, mCatalogs, false);
        await catalogsHandle.Task;

        if (catalogsHandle.Status == AsyncOperationStatus.Succeeded)
        {
            AsyncOperationHandle<long> sizeAsync = default;
            foreach (var item in catalogsHandle.Result)
            {
                sizeAsync = Addressables.GetDownloadSizeAsync(item);
                await sizeAsync.Task;

                if (sizeAsync.Status == AsyncOperationStatus.Succeeded)
                {
                    mTotalDownloadSize += sizeAsync.Result;
                }
                else
                {
                    //TODO 资源大小获取失败
                }
            }
            Addressables.Release(sizeAsync);
            DownloadAssets(catalogsHandle.Result);
        }
        else
        {
            //TODO 资源获取失败
        }
        Addressables.Release(catalogsHandle);
    }

    private async void DownloadAssets(List<IResourceLocator> result)
    {
        if (result == null || result.Count == 0)
        {
            EnterGame();
            return;
        }

        long downloadSize = 0;
        foreach (var locator in result) 
        {
            var downloadAsync = Addressables.DownloadDependenciesAsync(locator);
            await downloadAsync.Task;

            if (downloadAsync.Status == AsyncOperationStatus.Succeeded)
            {
                var sizeAsync = Addressables.GetDownloadSizeAsync(downloadAsync.Result);
                await sizeAsync.Task;
                if(sizeAsync.Status == AsyncOperationStatus.Succeeded)
                {
                    downloadSize += sizeAsync.Result;
                }

                await Task.Yield();
            }
            else
            {
                //TODO
            }
        }
        EnterGame();
    }
    #endregion

    private void EnterGame()
    {
#if !UNITY_EDITOR
        ReleaseEnterGame();
#else
        mHotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Game");
#endif
        //TODO 进入游戏
    }

    private async void ReleaseEnterGame()
    {
        var loadDllAsync = Addressables.LoadAssetAsync<TextAsset>("Game.dll");
        await loadDllAsync.Task;

        if(loadDllAsync.Status == AsyncOperationStatus.Succeeded)
        {
            mHotUpdateAss = Assembly.Load(loadDllAsync.Result.bytes);
        }
        else
        {
            //TODO 加载DLL失败
        }
    }
}