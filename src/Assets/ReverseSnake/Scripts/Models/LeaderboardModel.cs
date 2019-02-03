using System;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Models
{
    [Serializable]
    public class LeaderboardModel
    {
        public List<ResultModel> Results;

        public LeaderboardModel()
        {
            Results = new List<ResultModel>();
        }
    }

    [Serializable]
    public class ResultModel
    {
        public int Score;

        public string GameEndDateTime;
    }
}
