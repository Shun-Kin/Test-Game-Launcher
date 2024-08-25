using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressablesManager : MonoBehaviour
{
    [SerializeField] private List<SceneBind> sceneBinds;
    [SerializeField] private List<AssetBind> assetBinds;


    public List<SceneBind> SceneBinds
    {
        get { return sceneBinds; }
    }
    public List<AssetBind> AssetBinds
    {
        get { return assetBinds; }
    }


    public async UniTask<bool> InCacheAsync(object key, CancellationToken cancellationToken)
    {
        AsyncOperationHandle<long> opHandle = Addressables.GetDownloadSizeAsync(key);   // Проверка обновлений каталогов происходит автоматически при инициализации Addressables, можно проверить вручную (Addressable Asset Settings -> Only update catalogs manually https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/LoadContentCatalogAsync.html#updating-catalogs)

        try
        {
            return await opHandle.ToUniTask(cancellationToken: cancellationToken) == 0;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
        finally
        {
            Addressables.Release(opHandle);
        }
    }

    public async UniTask<bool> DownloadAssetsAsync(object key, CancellationToken cancellationToken)
    {
        AsyncOperationHandle opHandle = Addressables.DownloadDependenciesAsync(key, false);

        try
        {
            await opHandle.ToUniTask(cancellationToken: cancellationToken);
            return opHandle.Status == AsyncOperationStatus.Succeeded;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
        finally
        {
            Addressables.Release(opHandle);
        }
    }

    public async UniTask UnloadAssetsCacheAsync(object key, CancellationToken cancellationToken)
    {
        AsyncOperationHandle opHandle = Addressables.ClearDependencyCacheAsync(key, false);

        try
        {
            await opHandle.ToUniTask(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            Addressables.Release(opHandle);
        }
    }

    public async UniTask LoadAssetsAsync(CancellationToken cancellationToken)
    {
        AsyncOperationHandle opHandle;

        try
        {
            foreach (AssetBind assetBind in assetBinds)
            {
                switch (assetBind.assetType)
                {
                    case AssetType.Sprite:
                        opHandle = assetBind.reference.LoadAssetAsync<Sprite>();
                        await opHandle.ToUniTask(cancellationToken: cancellationToken);
                        if (assetBind.applyAssetToTarget)
                        {
                            assetBind.target.GetComponentInParent<Image>().sprite = opHandle.Result as Sprite;
                        }
                        break;

                    case AssetType.Prefab:
                        opHandle = assetBind.reference.LoadAssetAsync<GameObject>();
                        await opHandle.ToUniTask(cancellationToken: cancellationToken);
                        if (assetBind.applyAssetToTarget)
                        {
                            Instantiate(opHandle.Result as GameObject, assetBind.target.transform);
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void LoadScene(object key)
    {
        Addressables.LoadSceneAsync(key, LoadSceneMode.Single); // Unity автоматически вызывает UnloadUnusedAssets при загрузке сцены в режиме LoadSceneMode.Single, есть нюансы (https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/UnloadingAddressableAssets.html)
    }

    private void OnDestroy()
    {
        foreach (AssetBind assetBind in assetBinds)
        {
            assetBind.reference.ReleaseAsset();
        }
    }
}


/// <summary>Используется для удобной загрузки сцены и её ассетов</summary>
[Serializable]
public class SceneBind
{
    public AssetReference reference;
    public string label;
}

/// <summary>Используется для удобной загрузки ассетов и их установки</summary>
[Serializable]
public class AssetBind
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
