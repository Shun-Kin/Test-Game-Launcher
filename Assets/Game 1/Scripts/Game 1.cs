using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Game1 : MonoBehaviour
{
    private const string ClickCount = "clickCount"; // Название переменной, которая хранится в Unity CloudSave

    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image cliclImage;

    private AddressablesManager addressablesManager;
    private uint count; // Счётчик кликов
    private bool canQuit;


    private async void Start()
    {
        addressablesManager = GetComponentInParent<AddressablesManager>();
        await addressablesManager.LoadAssetsAsync(destroyCancellationToken);    // Загрузка ассетов происходит асинхронно, можно вызвать синхронно или сделать загрузочный экран
        count = await DataManager.LoadDataAsync<uint>(ClickCount);              // При ошибке будет присвоено значение по умолчанию - 0 (своеобразный оффлайн режим, можно сообщить об этом игроку)
        countText.text = count.ToString();
        Application.wantsToQuit += OnApplicationWantsToQuit;
        cliclImage.enabled = true;  // Включить картинку после загрузки количества кликов (чтобы не кликали раньше)
    }

    public void ClickerPointerDown()
    {
        countText.text = (++count).ToString();
    }

    public async void ToMenu()
    {
        Application.wantsToQuit -= OnApplicationWantsToQuit;
        await DataManager.SaveDataAsync(ClickCount, count); // Сохранить данные на сервер перед выходом в меню
        SceneManager.LoadScene("Menu");
    }

    private bool OnApplicationWantsToQuit()
    {
        if (canQuit)
        {
            return true;
        }
        else
        {
            SaveDataAndQuitAsync().Forget();
        }

        return false;
    }

    /// <summary>Сохраняет данные на сервер перед выходом из приложения</summary>
    private async UniTaskVoid SaveDataAndQuitAsync()
    {
        if (await DataManager.SaveDataAsync(ClickCount, count))
        {
            canQuit = true;
            Application.Quit();
        }
    }
}
