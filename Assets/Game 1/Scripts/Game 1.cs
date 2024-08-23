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


    private async void Start()
    {
        addressablesManager = GetComponentInParent<AddressablesManager>();
        await addressablesManager.LoadAssetsAsync();                // Загрузка ассетов происходит асинхронно, можно вызвать синхронно или сделать загрузочный экран
        count = await DataManager.LoadDataAsync<uint>(ClickCount);  // При ошибке будет присвоено значение по умолчанию - 0 (своеобразный оффлайн режим, можно сообщить об этом игроку)
        countText.text = count.ToString();
        cliclImage.enabled = true;  // Включить картинку после загрузки количества кликов (чтобы не кликали раньше)
    }

    public void ClickerPointerDown()
    {
        countText.text = (++count).ToString();
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        _ = DataManager.SaveDataAsync(ClickCount, count);   // Сохранить данные на сервер при выходе из мини-игры (при ошибке не сохранится, при выходе из лаунчера тоже не сохраняется)
    }
}
