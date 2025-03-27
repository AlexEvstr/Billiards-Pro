using UnityEngine;
using UnityEngine.UI;

public class GameHistoryUI : MonoBehaviour
{
    public Text[] playerGameNames; // 5 текстов для победителей
    public Text[] playerGameDates; // 5 текстов для дат
    public Text[] botGameNames;
    public Text[] botGameDates;

    private const int maxRecords = 5;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        LoadHistory("PlayerGame", playerGameNames, playerGameDates);
        LoadHistory("BotGame", botGameNames, botGameDates);
    }

    private void LoadHistory(string keyPrefix, Text[] nameTexts, Text[] dateTexts)
    {
        for (int i = 0; i < maxRecords; i++)
        {
            string record = PlayerPrefs.GetString($"{keyPrefix}_{i}", "");
            if (!string.IsNullOrEmpty(record))
            {
                string[] parts = record.Split('|');
                if (parts.Length == 2)
                {
                    nameTexts[i].text = parts[0];
                    dateTexts[i].text = parts[1];
                }
            }
            else
            {
                nameTexts[i].text = "-";
                dateTexts[i].text = "-";
            }
        }
    }

    public void ResetHistory()
    {
        for (int i = 0; i < maxRecords; i++)
        {
            PlayerPrefs.DeleteKey($"PlayerGame_{i}");
            PlayerPrefs.DeleteKey($"BotGame_{i}");
        }
        PlayerPrefs.Save();
        UpdateUI();
    }
}
