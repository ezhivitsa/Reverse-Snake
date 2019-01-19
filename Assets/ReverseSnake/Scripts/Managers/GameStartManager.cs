using Assets.ReverseSnake.Scripts.Systems;
using Leopotam.Ecs;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameStartManager
    {
        private static GameStartManager _instance;
        private static EcsWorld _instanceWorld;

        private EcsWorld _world;

        private GameStartManager(EcsWorld world)
        {
            _world = world;
        }

        public static GameStartManager GetInstance(EcsWorld world)
        {
            if (_instance == null || world != _instanceWorld)
            {
                _instance = new GameStartManager(world);
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
            TriggerStartGameEvent(true);
        }

        private void TriggerStartGameEvent(bool isActive)
        {
            var eventData = _world.CreateEntityWith<GameStartEvent>();
            eventData.IsActive = isActive;
        }

        private void TriggerClearBoardEvents()
        {
            var wallsFilter = _world.GetFilter<EcsFilter<Wall>>();
            for (var i = wallsFilter.EntitiesCount - 1; i >= 0; i -= 1)
            {
                var entity = wallsFilter.Entities[i];
                WallReactivitySystemOnRemove.CachedWalls[entity] = wallsFilter.Components1[i];
                _world.RemoveEntity(entity);
            }
        }
    }
}
