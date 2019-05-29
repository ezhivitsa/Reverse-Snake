using Assets.ReverseSnake.Scripts.Managers;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class ScoreReactivitySystemOnUpdate : EcsUpdateReactiveSystem<Score>, IEcsInitSystem
    {
        new ReverseSnakeWorld _world = null;

        private StateManager _stateManager = null;

        public void Initialize()
        {
            _stateManager = StateManager.GetInstance(_world);

            var scoreElement = GameObject.FindGameObjectWithTag(AppConstants.ScoreTag);
            var ui = GameObject.FindGameObjectWithTag(AppConstants.UITag);

            var entity = _world.CreateEntityWith<Score>(out Score score);
            score.GameObject = scoreElement;
            score.UI = ui;
            score.Result = scoreElement.GetComponent<Text>();
        }

        public void Destroy()
        {
        }

        protected override void RunUpdateReactive()
        {
            foreach (var entity in this) {
                var score = _world.GetComponent<Score>(entity);
                UpdateScore(score);
            }
        }

        private void UpdateScore(Score score)
        {
            score.Result.text = FormatText(score.Amount);

            if (!score.Silent)
            {
                _stateManager.SetScore(score.Amount);
            }
        }

        private string FormatText(int v)
        {
            return string.Format("{0}", v);
        }
    }
}
