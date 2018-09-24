using System;
using System.IO;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts
{
    sealed class SaveState
    {
        public static State State = new State();

        public delegate void SerializeAction();
        public static event SerializeAction OnLoaded;
        public static event SerializeAction OnBeforeSave;

        public static void Save(State state)
        {
            OnBeforeSave?.Invoke();

            SaveStateData(state);
        }

        public static void Load()
        {
            State = LoadStateData();

            OnLoaded?.Invoke();
        }

        private static State LoadStateData()
        {
            try
            {
                string json = File.ReadAllText(SavePath());
                return JsonUtility.FromJson<State>(json);
            }
            catch (Exception)
            {
                return new State();
            }
        }

        private static void SaveStateData(State state)
        {
            string json = JsonUtility.ToJson(state);

            StreamWriter sw = File.CreateText(SavePath());
            sw.Close();

            File.WriteAllText(SavePath(), json);
        }

        private static string SavePath()
        {
            return Path.Combine(Application.persistentDataPath, "state.json");
        }
    }
}
