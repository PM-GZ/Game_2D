using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

public class uAsset
{
    #region
    private static Dictionary<Type, ObjectPool<GameBehaviour>> _goPoolDict = new();


    private static GameBehaviour GetGO<T>(int maxCount = 30) where T : GameBehaviour, new()
    {
        if (!_goPoolDict.TryGetValue(typeof(T), out var goPool))
        {
            goPool = new(() => { return new T(); }, OnGet, OnRelease, OnDestroy, true, 0, maxCount);
        }
        return goPool.Get();
    }

    #region gameObject对象池方法
    private static void OnGet(GameBehaviour gb)
    {
        gb.OnReset();
    }

    private static void OnRelease(GameBehaviour gb)
    {
        gb.OnRelease();
    }

    private static void OnDestroy(GameBehaviour gb)
    {
        gb.Destroy();
    }
    #endregion
    #endregion


    private static List<AsyncOperationHandle> _assetsList = new();
    private static List<AsyncOperationHandle> _instanceAssetsList = new();
    private static List<AsyncOperationHandle> _sceneAssetsList = new();

    #region Asset Load
    /// <summary>
    /// pool 为 true 时，此类型对象会从对象池创建和获取
    /// </summary>
    public static T GetGameObject<T>(bool pool = false, int maxCount = 30) where T : GameBehaviour, new()
    {
        GameBehaviour gb = pool ? GetGO<T>(maxCount) : new T();
        return gb as T;
    }

    public static GameObject LoadGameObject(string goName, Transform parent = null)
    {
        var goOperation = Addressables.InstantiateAsync(goName, parent, false);
        _instanceAssetsList.Add(goOperation);
        var go = goOperation.WaitForCompletion();
        return go;
    }

    public static T LoadAsset<T>(string assetName)
    {
        return LoadAssetAsync<T>(assetName).WaitForCompletion();
    }

    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName)
    {
        var operation = Addressables.LoadAssetAsync<T>(assetName);
        _assetsList.Add(operation);
        return operation;
    }

    public static IList<T> LoadAsssets<T>(string lable)
    {
        return LoadAssetsAsync<T>(lable).WaitForCompletion();
    }

    public static AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string lable)
    {
        var operation = Addressables.LoadAssetsAsync<T>(lable, null, true);
        _assetsList.Add(operation);
        return operation;
    }

    public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        var operation = Addressables.LoadSceneAsync(sceneName, mode);
        if (mode == LoadSceneMode.Additive)
        {
            _sceneAssetsList.Add(operation);
        }
        return operation;
    }
    #endregion

    #region Asset Unload
    public static void UnloadAssets()
    {
        for (int i = _assetsList.Count - 1; i >= 0; i--)
        {
            Addressables.Release(_assetsList[i]);
        }
        _assetsList.Clear();
    }

    public static void UnloadInstanceAssets()
    {
        for (int i = _instanceAssetsList.Count - 1; i >= 0; i--)
        {
            Addressables.ReleaseInstance(_instanceAssetsList[i]);
        }
        _instanceAssetsList.Clear();
    }

    public static void UnloadScenes()
    {
        for (int i = _sceneAssetsList.Count - 1; i >= 0; i--)
        {
            Addressables.UnloadSceneAsync(_sceneAssetsList[i]).WaitForCompletion();
        }
        _sceneAssetsList.Clear();
    }

    public static void ClearPools()
    {
        var goPools = _goPoolDict.ToList();
        for (int i = goPools.Count - 1; i >= 0; i--)
        {
            goPools[i].Value.Dispose();
        }
        _goPoolDict.Clear();
    }

    public static void UnloadAll()
    {
        UnloadAssets();
        UnloadInstanceAssets();
        UnloadScenes();
        ClearPools();
        Resources.UnloadUnusedAssets();
    }
    #endregion
}
