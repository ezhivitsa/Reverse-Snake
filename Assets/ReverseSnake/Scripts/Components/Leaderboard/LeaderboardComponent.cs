using Assets.ReverseSnake.Scripts.IO;
using Assets.src;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardComponent : MonoBehaviour
{
    public string mainMenu;

    void OnEnable()
    {
        SaveLeaderboard.OnLoaded += OnLeaderboardLoaded;
        SaveLeaderboard.Load();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    void OnDestroy()
    {
        SaveLeaderboard.OnLoaded -= OnLeaderboardLoaded;
    }

    private void OnLeaderboardLoaded()
    {
        var leaderboard = SaveLeaderboard.Leaderboard;

        var lines = GetComponentsInChildren<Canvas>()
            .Where(el => el.tag == AppConstants.LeaderboardLineTag)
            .ToList();
        for (var i = 0; i < lines.Count && i < leaderboard.Results.Count; i += 1)
        {
            var ui = lines[i];
            var result = leaderboard.Results[i];

            Text[] labels = ui.GetComponentsInChildren<Text>();

            Text scopeText = labels[1];
            Text dateText = labels[2];

            scopeText.text = result.Score.ToString();
            dateText.text = FormatDate(result.GameEndDateTime);
        }
    }

    private string FormatDate(string dateTimeString)
    {
        var dateTime = DateTime.Parse(dateTimeString);
        return dateTime.ToString("MM/dd/yyyy");
    }
}
