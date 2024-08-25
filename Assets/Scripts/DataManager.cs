using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;

public static class DataManager
{
    private static bool isError;
    private static readonly Dictionary<string, object> data = new Dictionary<string, object>();


    public static async UniTask<bool> SignInAsync(string username, string password)
    {
        try
        {
            // Для сохранения/загрузки данных используется сервис Unity CloudSave
            await UnityServices.InitializeAsync().AsUniTask();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password).AsUniTask();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            isError = true;
            return false;
        }

        isError = false;    // Сбросить ошибку для возможности сохранения
        return true;
    }

    public static async UniTask<bool> SaveDataAsync<T>(string key, T value)
    {
        if (isError)    // Не сохранять на сервер
        {
            return false;
        }

        try
        {
            data[key] = value;
            await CloudSaveService.Instance.Data.Player.SaveAsync(data).AsUniTask();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            isError = true;
            return false;
        }
    }

    public static async UniTask<T> LoadDataAsync<T>(string key)
    {
        try
        {
            Dictionary<string, Item> dataItem = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key }).AsUniTask();
            return dataItem[key].Value.GetAs<T>();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            isError = true;
            return default;
        }
    }
}
