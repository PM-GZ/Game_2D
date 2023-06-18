using System.Collections.Generic;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;


public class uAsset : BaseObject
{
    public struct AssetData
    {
        public string name;
        public string path;
    }

    private static Dictionary<string, List<AssetData>> mAssetDict = new();
    public override void Init()
    {
    }

    public T LoadAsset<T>(string assetName)
    {
        return LoadAssetAsync<T>(assetName).WaitForCompletion();
    }

    public AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName)
    {
        return Addressables.LoadAssetAsync<T>(assetName);
    }

    public IList<T> LoadAsssets<T>(string lable)
    {
        return LoadAssetsAsync<T>(lable).WaitForCompletion();
    }

    public AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(string lable)
    {
        return Addressables.LoadAssetsAsync<T>(lable, null, true);
    }

    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single, bool activeOnLoad = true)
    {
        return Addressables.LoadSceneAsync(path, mode, activeOnLoad);
    }
}
