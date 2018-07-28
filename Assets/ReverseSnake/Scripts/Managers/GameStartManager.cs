using LeopotamGroup.Ecs;

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

        public void StartGame()
        {
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

            var boardEventData = _world.CreateEntityWith<ShowBoardEvent>();
            boardEventData.IsActive = false;

            var stepEventData = _world.CreateEntityWith<ClearStepEvent>();
            stepEventData.Round = round;

            var stepTargetData = _world.CreateEntityWith<ClearTargetEvent>();
            stepTargetData.Round = round;

            _world.CreateEntityWith<ClearWallEvent>();

            var wallEventData = _world.CreateEntityWith<ShowWallEvent>();
            wallEventData.IsActive = false;
        }
    }
}
