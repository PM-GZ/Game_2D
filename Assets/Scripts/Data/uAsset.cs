using System;
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
    private static Dictionary<Type, ObjectPool<GameBehaviour>> _goPoolDict = new();



    private static GameBehaviour GetGO<T>(int maxCount = 30) where T : GameBehaviour, new()
    {
        if (!_goPoolDict.TryGetValue(typeof(T), out var goPool))
        {
            goPool = new (() => { return new T(); }, OnGet, OnRelease, OnDestroy, true, 0, maxCount);
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


    /// <summary>
    /// pool 为 true 时，此类型对象会从对象池创建和获取
    /// </summary>
    public static T GetGameObject<T>(bool pool = false, int maxCount = 30) where T : GameBehaviour, new()
    {
        GameBehaviour gb = pool ? GetGO<T>(maxCount) : new T();
        return gb as T;
    }

    public static GameObject LoadGameObject(string goName, bool loadAndRelease = true)
    {
        var prefab = LoadAsset<GameObject>(goName);
        var go = Object.Instantiate(prefab, null, false);
        if (loadAndRelease)
        {
            Addressables.ReleaseInstance(prefab);
        }
        return go;
    }

    public static T LoadAsset<T>(string assetName)
    {
        return LoadAssetAsync<T>(assetName).WaitForCompletion();
    }

    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName)
    {
        return Addressables.LoadAssetAsync<T>(assetName);
    }

    public static IList<T> LoadAsssets<T>(string lable)
    {
        return LoadAssetsAsync<T>(lable).WaitForCompletion();
    }

    public static AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string lable)
    {
        return Addressables.LoadAssetsAsync<T>(lable, null, true);
    }

    public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool activeOnLoad = true)
    {
        var async = SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation = activeOnLoad;
        return async;
    }
}
