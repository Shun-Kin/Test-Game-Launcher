using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static bool error;
    private static Dictionary<string, object> data = new Dictionary<string, object>();


    public static async Task<bool> SignInAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync("user1", "User1234!");  // Войти под тестовым пользователем
            }
        }
        catch
        {
            error = true;
            return false;
        }

        error = false;  // Сбросить ошибку для возможности сохранения
        return true;
    }

    public static async Task<bool> SaveDataAsync<T>(string key, T value)
    {
        if (error)  // Не сохранять на сервер
        {
            return false;
        }

        try
        {
            data[key] = value;
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            return true;
        }
        catch
        {
            error = true;
            return false;
        }
    }

    public static async Task<T> LoadDataAsync<T>(string key)
    {
        try
        {
            Dictionary<string, Item> dataItem = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key });
            return dataItem[key].Value.GetAs<T>();
        }
        catch
        {
            error = true;
            return default;
        }
    }
}
