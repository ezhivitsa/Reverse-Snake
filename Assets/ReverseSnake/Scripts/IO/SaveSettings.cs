using Assets.ReverseSnake.Scripts.Models;
using System;
using System.IO;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.IO
{
    public sealed class SaveSettings
    {
        public static SettingsModel Settings = new SettingsModel();

        public delegate void SerializeAction();
        public static event SerializeAction OnLoaded;
        public static event SerializeAction OnBeforeSave;

        public static void Save(SettingsModel model)
        {
            OnBeforeSave?.Invoke();
            SaveSettingsData(model);
        }

        public static void SetDataAndSave(bool useMusic, bool useSounds)
        {
            var model = new SettingsModel
            {
                UseMusic = useMusic,
                UseSounds = useSounds,
            };

            SaveSettingsData(model);
        }

        public static void Load()
        {
            Settings = LoadSettingsData();
            OnLoaded?.Invoke();
        }

        private static SettingsModel LoadSettingsData()
        {
            try
            {
                string json = File.ReadAllText(SavePath());
                return JsonUtility.FromJson<SettingsModel>(json);
            }
            catch (Exception)
            {
                return new SettingsModel();
            }
        }

        private static void SaveSettingsData(SettingsModel model)
        {
            string json = JsonUtility.ToJson(model);

            StreamWriter sw = File.CreateText(SavePath());
            sw.Close();

            File.WriteAllText(SavePath(), json);
        }

        private static string SavePath()
        {
            return Path.Combine(Application.persistentDataPath, "settings.json");
        }
    }
}
