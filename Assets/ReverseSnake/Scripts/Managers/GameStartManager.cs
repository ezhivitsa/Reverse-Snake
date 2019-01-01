using Assets.ReverseSnake.Scripts.Models;
using Assets.ReverseSnake.Scripts.Systems;
using Assets.src;
using Leopotam.Ecs;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameStartManager
    {
        private static GameStartManager _instance;
        private static EcsWorld _instanceWorld;

        private EcsWorld _world;
        private EcsFilter<Step> _stepsFilter = null;

        private GameStartManager(EcsWorld world, EcsFilter<Step> stepsFilter)
        {
            _world = world;
            _stepsFilter = stepsFilter;
        }

        public static GameStartManager GetInstance(EcsWorld world, EcsFilter<Step> stepsFilter)
        {
            if (_instance == null || world != _instanceWorld)
            {
                _instance = new GameStartManager(world, stepsFilter);
                _instanceWorld = world;
            }
            return _instance;
        }

        public void EndGame(int round)
        {
            TriggerStartGameEvent(false);
            TriggerClearBoardEvents(round);
        }

        public void StartGame(PositionModel targetModel, PositionModel stepModel)
        {
            TriggerShowEvents(true);
            TriggerResetScoreEvent();
            TriggerUpdateTargetEvent(targetModel);
            TriggerMovementEvent(stepModel);
            TriggerStartGameEvent(true);
        }

        private void TriggerStartGameEvent(bool isActive)
        {
            var eventData = _world.CreateEntityWith<GameStartEvent>();
            eventData.IsActive = isActive;
        }

        private void TriggerClearBoardEvents(int round)
        {
            var eventData = _world.CreateEntityWith<ClearBoardEvent>();
            eventData.Round = round;

            for (var i = 0; i < _stepsFilter.EntitiesCount; i++)
            {
                var component = _stepsFilter.Components1[i];
                var entity = _stepsFilter.Entities[i];
                StepReactiveSystemOnRemove.CachedSteps[entity] = component;
                _world.RemoveEntity(entity);
            }

            _world.CreateEntityWith<ClearWallEvent>();

            TriggerShowEvents(false);
        }

        private void TriggerShowEvents(bool isActive)
        {
            var targetEventData = _world.CreateEntityWith<ShowTargetEvent>();
            targetEventData.IsActive = isActive;

            var wallEventData = _world.CreateEntityWith<ShowWallEvent>();
            wallEventData.IsActive = isActive;
        }

        private void TriggerResetScoreEvent()
        {
            var eventData = _world.CreateEntityWith<ScoreSetEvent>();
            eventData.Amount = 0;
            eventData.Silent = false;
        }

        private void TriggerUpdateTargetEvent(PositionModel position)
        {
            var eventData = _world.CreateEntityWith<UpdateTargetEvent>();
            eventData.Round = AppConstants.FirstRound;
            eventData.Column = position.Column;
            eventData.Row = position.Row;
            eventData.Silent = false;
        }

        private void TriggerMovementEvent(PositionModel position)
        {
            var eventData = _world.CreateEntityWith<Step>();
            eventData.Round = AppConstants.FirstRound;
            eventData.Column = position.Column;
            eventData.Row = position.Row;
            eventData.StartNumber = AppConstants.StartStepsCount;
            eventData.Number = AppConstants.StartStepsCount;
            eventData.Silent = false;
        }
    }
}
