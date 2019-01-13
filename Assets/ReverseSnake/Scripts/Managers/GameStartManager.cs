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

        public void EndGame()
        {
            TriggerStartGameEvent(false);
            TriggerClearBoardEvents();
        }

        public void StartGame()
        {
            TriggerShowEvents(true);
            TriggerStartGameEvent(true);
        }

        private void TriggerStartGameEvent(bool isActive)
        {
            var eventData = _world.CreateEntityWith<GameStartEvent>();
            eventData.IsActive = isActive;
        }

        private void TriggerClearBoardEvents()
        {
            _world.CreateEntityWith<ClearWallEvent>();

            TriggerShowEvents(false);
        }

        private void TriggerShowEvents(bool isActive)
        {
            var wallEventData = _world.CreateEntityWith<ShowWallEvent>();
            wallEventData.IsActive = isActive;
        }
    }
}
