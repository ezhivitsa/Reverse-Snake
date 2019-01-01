using Assets.ReverseSnake.Scripts.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsComponent : MonoBehaviour
{
    public string mainMenu;

    void OnEnable()
    {
        SaveSettings.OnLoaded += OnSettingsLoaded;
        SaveSettings.Load();
    }

    void OnDestroy()
    {
        SaveSettings.OnLoaded -= OnSettingsLoaded;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    private void OnSettingsLoaded()
    {
        var settings = SaveSettings.Settings;
    }
}
