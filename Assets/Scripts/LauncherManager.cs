using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LauncherManager : MonoBehaviour
{
    [SerializeField] private List<GameButtons> buttons;
    // Данные пользователя для подключения к сервису
    [SerializeField] private string username;
    [SerializeField] private string password;

    private AddressablesManager addressablesManager;


    private async void Start()
    {
        addressablesManager = GetComponentInParent<AddressablesManager>();
        if (await DataManager.SignInAsync(username, password))  // Игрок смог войти/подключиться или уже вошёл
        {
            foreach (GameButtons button in buttons)
            {
                button.download.interactable = true;
            }
        }

        buttons[0].start.interactable = await addressablesManager.InCacheAsync(addressablesManager.SceneBinds[0].label, destroyCancellationToken);  // Проверить наличие ассетов игр в кэше (проверяется по очереди)
        buttons[1].start.interactable = await addressablesManager.InCacheAsync(addressablesManager.SceneBinds[1].label, destroyCancellationToken);
    }

    private async void DownloadGame(int index)
    {
        buttons[index].download.interactable = false;
        buttons[index].start.interactable = await addressablesManager.DownloadAssetsAsync(addressablesManager.SceneBinds[index].label, destroyCancellationToken);   // Скачать ассеты игры с сервера
        buttons[index].download.interactable = true;
    }

    private async void UnloadGame(int index)
    {
        buttons[index].unload.interactable = false;
        buttons[index].start.interactable = false;
        await addressablesManager.UnloadAssetsCacheAsync(addressablesManager.SceneBinds[index].label, destroyCancellationToken);    // Очистить кэш игры
        buttons[index].unload.interactable = true;
    }

    private void StartGame(int index)
    {
        addressablesManager.LoadScene(addressablesManager.SceneBinds[index].reference); // Загрузить сцену игры
    }

    // Game 1
    public void DownloadGame1()
    {
        DownloadGame(0);
    }

    public void UnloadGame1()
    {
        UnloadGame(0);
    }

    public void StartGame1()
    {
        StartGame(0);
    }

    // Game 2
    public void DownloadGame2()
    {
        DownloadGame(1);
    }

    public void UnloadGame2()
    {
        UnloadGame(1);
    }

    public void StartGame2()
    {
        StartGame(1);
    }
}


[Serializable]
public class GameButtons
{
    public Button start;
    public Button download;
    public Button unload;
}
