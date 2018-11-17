using Assets.ReverseSnake.Scripts.Models;
using Assets.src;
using Leopotam.Ecs;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameStartManager
    {
        private EcsWorld _world;

        public GameStartManager(EcsWorld world)
        {
            _world = world;
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

            var stepEventData = _world.CreateEntityWith<ClearStepEvent>();
            stepEventData.Round = round;

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
            var eventData = _world.CreateEntityWith<MovementEvent>();
            eventData.Round = AppConstants.FirstRound;
            eventData.Column = position.Column;
            eventData.Row = position.Row;
            eventData.StartNumber = AppConstants.StartStepsCount;
            eventData.Number = AppConstants.StartStepsCount;
        }
    }
}
