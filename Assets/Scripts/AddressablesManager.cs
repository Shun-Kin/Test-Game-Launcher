using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressablesManager : MonoBehaviour
{
    public List<SceneBind> sceneBinds;
    public List<AssetBind> assetBinds;


    public async Task<bool> InCacheAsync(object key)
    {
        AsyncOperationHandle<long> opHandle = Addressables.GetDownloadSizeAsync(key);   // Проверка обновлений каталогов происходит автоматически при инициализации Addressables, можно проверить вручную (Addressable Asset Settings -> Only update catalogs manually https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/LoadContentCatalogAsync.html#updating-catalogs)
        await opHandle.Task;
        long result = opHandle.Result;
        Addressables.Release(opHandle);
        return result == 0;
    }

    public async Task<bool> DownloadAssetsAsync(object key)
    {
        AsyncOperationHandle opHandle = Addressables.DownloadDependenciesAsync(key, false);
        await opHandle.Task;
        bool result = opHandle.Status == AsyncOperationStatus.Succeeded;
        Addressables.Release(opHandle);
        return result;
    }

    public async Task UnloadAssetsCacheAsync(object key)
    {
        AsyncOperationHandle opHandle = Addressables.ClearDependencyCacheAsync(key, false);
        await opHandle.Task;
        Addressables.Release(opHandle);
    }

    public async Task LoadAssetsAsync()
    {
        foreach (AssetBind assetBind in assetBinds)
        {
            switch (assetBind.assetType)
            {
                case AssetType.Sprite:
                    AsyncOperationHandle<Sprite> opSpriteHandle = assetBind.reference.LoadAssetAsync<Sprite>();
                    await opSpriteHandle.Task;
                    if (assetBind.applyAssetToTarget)
                    {
                        assetBind.target.GetComponentInParent<Image>().sprite = opSpriteHandle.Result;
                    }
                    break;

                case AssetType.Prefab:
                    AsyncOperationHandle<GameObject> opGOHandle = assetBind.reference.LoadAssetAsync<GameObject>();
                    await opGOHandle.Task;
                    if (assetBind.applyAssetToTarget)
                    {
                        Instantiate(opGOHandle.Result, assetBind.target.transform);
                    }
                    break;
            }
        }
    }

    private void ReleaseAssets()
    {
        foreach (AssetBind assetBind in assetBinds)
        {
            assetBind.reference.ReleaseAsset();
        }
    }

    public void LoadScene(object key)
    {
        Addressables.LoadSceneAsync(key, LoadSceneMode.Single); // Unity автоматически вызывает UnloadUnusedAssets при загрузке сцены в режиме LoadSceneMode.Single, есть нюансы (https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/UnloadingAddressableAssets.html)
    }

    private void OnDestroy()
    {
        ReleaseAssets();    // Освободить загруженные ассеты сцены
    }
}


[Serializable]
public class SceneBind  // Используется для удобной загрузки сцены и её ассетов
{
    public AssetReference reference;
    public string label;
}

[Serializable]
public class AssetBind  // Используется для удобной загрузки ассетов и их установки
{
    public AssetReference reference;
    public AssetType assetType;
    public GameObject target;       // Целевой объект, связанный с загруженным ассетом
    public bool applyAssetToTarget; // Применить ассет к целевому объекту (или обработать загруженный ассет вручную)
}

public enum AssetType
{
    Sprite,
    Prefab
}
