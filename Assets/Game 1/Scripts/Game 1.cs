using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading;
using System.IO;
using System;

public class Game1 : MonoBehaviour
{
    public TMP_Text countText;
    public Image cliclImage;
    private AddressablesManager addressablesManager;
    private int count;  // Счётчик кликов


    private async void Start()
    {
        addressablesManager = GetComponentInParent<AddressablesManager>();
        await addressablesManager.LoadAssetsAsync();                // Загрузка ассетов происходит асинхронно, можно вызвать синхронно или сделать загрузочный экран
        count = await DataManager.LoadDataAsync<int>("clickCount"); // При ошибке будет присвоено значение по умолчанию - 0 (своеобразный оффлайн режим, можно сообщить об этом игроку)
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
        _ = DataManager.SaveDataAsync("clickCount", count);  // Сохранить данные на сервер при выходе из мини-игры (при ошибке не сохранится, при выходе из лаунчера тоже не сохраняется)
    }
}
