using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class Game2 : MonoBehaviour
{
    private const string BestTime = "bestTime"; // Название переменной, которая хранится в Unity CloudSave

    public static Game2 instance;

    [SerializeField] private int characterAssetIndex;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text timeText;

    private AddressablesManager addressablesManager;
    private CharacterScript character;
    private Transform characterTransform;
    private float currentTime;
    private float bestTime;


    private async void Start()
    {
        instance = this;
        addressablesManager = GetComponentInParent<AddressablesManager>();
        await addressablesManager.LoadAssetsAsync(destroyCancellationToken);
        character = Instantiate((GameObject)addressablesManager.AssetBinds[characterAssetIndex].reference.Asset).GetComponent<CharacterScript>();
        characterTransform = character.transform;
        bestTime = await DataManager.LoadDataAsync<float>(BestTime);
        if (bestTime == default)    // Оффлайн режим (можно сообщить об этом игроку)
        {
            bestTime = float.MaxValue;
        }
        StartGame();
    }

    public void ShowResult()
    {
        character.CanMove(false);
        Cursor.lockState = CursorLockMode.None;
        resultPanel.SetActive(true);
        currentTime = Time.time - currentTime;
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            DataManager.SaveDataAsync(BestTime, bestTime).Forget();    // При ошибке сети не сохранится (можно обработать, сообщив от этом игроку)
        }
        timeText.text = $"Время: {currentTime:f1}с\r\n\r\nЛучшее время: {bestTime:f1}с";
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
