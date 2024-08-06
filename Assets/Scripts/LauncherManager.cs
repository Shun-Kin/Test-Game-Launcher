using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LauncherManager : MonoBehaviour
{
    public List<GameButtons> buttons;
    private AddressablesManager addressablesManager;


    private async void Start()
    {
        addressablesManager = GetComponentInParent<AddressablesManager>();
        if (await DataManager.SignInAsync())    // ����� ���� �����/������������ ��� ��� �����
        {
            foreach (GameButtons button in buttons)
            {
                button.download.interactable = true;
            }
        }

        buttons[0].start.interactable = await addressablesManager.InCacheAsync(addressablesManager.sceneBinds[0].label);    // ��������� ������� ������� ��� � ���� (����������� �� �������)
        buttons[1].start.interactable = await addressablesManager.InCacheAsync(addressablesManager.sceneBinds[1].label);
    }

    private async void DownloadGame(int index)
    {
        buttons[index].download.interactable = false;
        buttons[index].start.interactable = await addressablesManager.DownloadAssetsAsync(addressablesManager.sceneBinds[index].label); // ������� ������ ���� � �������
        buttons[index].download.interactable = true;
    }

    private async void UnloadGame(int index)
    {
        buttons[index].unload.interactable = false;
        buttons[index].start.interactable = false;
        await addressablesManager.UnloadAssetsCacheAsync(addressablesManager.sceneBinds[index].label);  // �������� ��� ����
        buttons[index].unload.interactable = true;
    }

    private void StartGame(int index)
    {
        addressablesManager.LoadScene(addressablesManager.sceneBinds[index].reference); // ��������� ����� ����
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