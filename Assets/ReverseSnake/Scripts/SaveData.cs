using System.IO;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts
{
    sealed class SaveData
    {
        public static State state = new State();

        public delegate void SerializeAction();
        public static event SerializeAction OnLoaded;
        public static event SerializeAction OnBeforeSave;

        public static void Save(State state)
        {
            OnBeforeSave?.Invoke();

            SaveState(state);
        }

        public static void Load()
        {
            state = LoadState();

            OnLoaded?.Invoke();
        }

        private static State LoadState()
        {
            string json = File.ReadAllText(SavePath());

            return JsonUtility.FromJson<State>(json);
        }

        private static void SaveState(State state)
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
