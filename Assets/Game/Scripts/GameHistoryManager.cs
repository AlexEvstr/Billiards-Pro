using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    private const int maxRecords = 5;

    public static void SaveResult(string winnerName)
    {
        string date = System.DateTime.Now.ToString("dd.MM.yyyy");
        string record = $"{winnerName}|{date}";

        string keyPrefix = IsBotGame(winnerName) ? "BotGame" : "PlayerGame";

        for (int i = maxRecords - 1; i > 0; i--)
        {
            string prev = PlayerPrefs.GetString($"{keyPrefix}_{i - 1}", "");
            PlayerPrefs.SetString($"{keyPrefix}_{i}", prev);
        }

        PlayerPrefs.SetString($"{keyPrefix}_0", record);
        PlayerPrefs.Save();
    }

    private static bool IsBotGame(string winnerName)
    {
        return winnerName == "PLAYER" || winnerName == "BOT";
    }
}
