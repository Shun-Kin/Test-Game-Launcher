using UnityEngine;
using TMPro;

public class Game2 : MonoBehaviour
{
    private static Game2 instance;

    public int characterAssetIndex;
    public GameObject resultPanel;
    public TMP_Text timeText;
    private AddressablesManager addressablesManager;
    private CharacterScript character;
    private Transform characterTransform;
    private float currentTime;
    private float bestTime;


    private async void Start()
    {
        instance = this;
        addressablesManager = GetComponentInParent<AddressablesManager>();
        await addressablesManager.LoadAssetsAsync();
        character = Instantiate((GameObject)addressablesManager.assetBinds[characterAssetIndex].reference.Asset).GetComponent<CharacterScript>();
        characterTransform = character.transform;
        bestTime = await DataManager.LoadDataAsync<float>("bestTime");
        if (bestTime == default)    // Оффлайн режим (можно сообщить об этом игроку)
        {
            bestTime = float.MaxValue;
        }
        StartGame();
    }

    private void _ShowResult()
    {
        character.CanMove(false);
        Cursor.lockState = CursorLockMode.None;
        resultPanel.SetActive(true);
        currentTime = Time.time - currentTime;
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            _ = DataManager.SaveDataAsync("bestTime", bestTime);    // При ошибке сети не сохранится (можно обработать, сообщив от этом игроку)
        }
        timeText.text = $"Время: {currentTime:f1}с\r\n\r\nЛучшее время: {bestTime:f1}с";
    }

    public static void ShowResult()
    {
        instance._ShowResult();
    }

    public void StartGame()
    {
        characterTransform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);   // Переместить игрока в исходную точку
        Cursor.lockState = CursorLockMode.Locked;
        resultPanel.SetActive(false);
        character.CanMove(true);
        currentTime = Time.time;
    }
}
