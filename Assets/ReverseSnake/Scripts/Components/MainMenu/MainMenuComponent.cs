using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuComponent : MonoBehaviour
{
    public string gameScene;
    public string leaderboardScene;
    public string settingsScene;

    public void NewGame()
    {
        GameStartup.LoadState = false;
        SceneManager.LoadScene(gameScene);
    }

    public void LoadGame()
    {
        GameStartup.LoadState = true;
        SceneManager.LoadScene(gameScene);
    }

    public void Leaderboard()
    {
        SceneManager.LoadScene(leaderboardScene);
    }

    public void Settings()
    {
        SceneManager.LoadScene(settingsScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
