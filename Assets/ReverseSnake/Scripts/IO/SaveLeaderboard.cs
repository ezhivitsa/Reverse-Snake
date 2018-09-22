using Assets.ReverseSnake.Scripts.Models;
using Assets.src;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.IO
{
    sealed class SaveLeaderboard
    {
        public static LeaderboardModel Leaderboard = new LeaderboardModel();

        public delegate void SerializeAction();
        public static event SerializeAction OnLoaded;
        public static event SerializeAction OnBeforeSave;

        public static void Save(LeaderboardModel model)
        {
            OnBeforeSave?.Invoke();
            SaveLeaderboardData(model);
        }

        public static void AddResultAndSave(int score)
        {
            var model = LoadLeaderboardData();
            model.Results.Add(new ResultModel
            {
                Score = score,
                GameEndDateTime = DateTime.Now.ToString(),
            });
            model.Results = model.Results
                .OrderByDescending(r => r.Score)
                .Take(AppConstants.LeaderboardCount)
                .ToList();
            
            SaveLeaderboardData(model);
        }

        public static void Load()
        {
            Leaderboard = LoadLeaderboardData();
            OnLoaded?.Invoke();
        }

        private static LeaderboardModel LoadLeaderboardData()
        {
            try
            {
                string json = File.ReadAllText(SavePath());
                return JsonUtility.FromJson<LeaderboardModel>(json);
            }
            catch (Exception)
            {
                return new LeaderboardModel();
            }
        }

        private static void SaveLeaderboardData(LeaderboardModel model)
        {
            string json = JsonUtility.ToJson(model);

            StreamWriter sw = File.CreateText(SavePath());
            sw.Close();

            File.WriteAllText(SavePath(), json);
        }

        private static string SavePath()
        {
            return Path.Combine(Application.persistentDataPath, "leaderboard.json");
        }
    }
}
