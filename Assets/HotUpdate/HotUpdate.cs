using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HotUpdate : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(UpdateAssets());
    }

    private IEnumerator UpdateAssets()
    {
        AsyncOperationHandle checkUpdate = Addressables.CheckForCatalogUpdates(false);
        yield return checkUpdate;
        List<string> checkResults = checkUpdate.Result as List<string>;
        Addressables.Release(checkUpdate);

        if (checkResults.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkResults, false);
            yield return updateHandle;
            List<IResourceLocator> catalogs = updateHandle.Result;
            Addressables.Release(catalogs);

            foreach (var catalog in catalogs)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(catalog);
                yield return sizeHandle;
                long totalSize = sizeHandle.Result;
                Addressables.Release(sizeHandle);

                if (totalSize > 0)
                {
                    var downloadHandle = Addressables.DownloadDependenciesAsync(catalog.Keys);
                    while (downloadHandle.Status == AsyncOperationStatus.None)
                    {
                        float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                        yield return null;
                    }
                }
            }
        }
    }
}
