using Assets.ReverseSnake.Scripts.IO;
using Assets.ReverseSnake.Scripts.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsComponent : MonoBehaviour
{
    public string MainMenuScene;

    private Toggle _musicToggle;
    private Toggle _soundsToggle;

    private SettingsModel _settings;

    void OnEnable()
    {
        var toggleElements = GetComponentsInChildren<Toggle>();
        _musicToggle = toggleElements[0];
        _soundsToggle = toggleElements[1];

        _musicToggle.onValueChanged.AddListener(MusicToggleValueChanged);
        _soundsToggle.onValueChanged.AddListener(SoundsToggleValueChanged);

        SaveSettings.OnLoaded += OnSettingsLoaded;
        SaveSettings.Load();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu();
        }
    }

    void OnDestroy()
    {
        SaveSettings.OnLoaded -= OnSettingsLoaded;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    private void OnSettingsLoaded()
    {
        _settings = SaveSettings.Settings;

        _musicToggle.isOn = _settings.UseMusic;
        _soundsToggle.isOn = _settings.UseSounds;
    }

    private void MusicToggleValueChanged(bool isOn)
    {
        if (_settings == null)
        {
            return;
        }

        var settings = new SettingsModel
        {
            UseMusic = isOn,
            UseSounds = _settings.UseSounds,
        };
        _settings = settings;
        SaveSettings.Save(settings);
    }

    private void SoundsToggleValueChanged(bool isOn)
    {
        if (_settings == null)
        {
            return;
        }

        var settings = new SettingsModel
        {
            UseMusic = _settings.UseMusic,
            UseSounds = isOn,
        };
        _settings = settings;
        SaveSettings.Save(settings);
    }
}
