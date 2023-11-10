using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif




[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void EditorInit()
    {
        TEXT.Init("CN");
    }
#else

#endif


    private static bool mInitFlag = false;

    public static uData Data { get; private set; }
    public static uUi Ui { get; private set; }
    public static GameInput Input { get; private set; }
    public static uScene Scene { get; private set; }


    public uState CurState { get; private set; }
    private Coroutine mStateCoroutine;
    private long mTotalDownloadSize;
    Assembly mHotUpdateAss;

    private void Start()
    {
        if (mInitFlag)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Init();
        mInitFlag = true;
    }

    private void Init()
    {
        InitManager();
        Data.Init();
    }

    private void InitManager()
    {
        Data = new();
        Input = new();
        Scene = new();
        Ui = new();
    }

    private void Update()
    {
        CurState?.Update();
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
        CurState?.Quit();
        CurState = null;

        Input.Dispose();
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadState(uState state)
    {
        CurState?.Quit();
        CurState = state;
        mStateCoroutine = StartCoroutine(CurState.Enter());
    }

    #region HotUpdate
    private async void CheckAssetsUpdate()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        await checkHandle.Task;

        if (checkHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var catalogs = checkHandle.Result;
            GetDownloadAssets(catalogs);
        }
        else
        {
            //TODO 检查资源更新失败
        }
        Addressables.Release(checkHandle);
    }

    private async void GetDownloadAssets(List<string> catalogs)
    {
        if (catalogs == null || catalogs.Count == 0)
        {
            EnterGame();
            return;
        }

        var catalogsHandle = Addressables.UpdateCatalogs(true, catalogs, false);
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