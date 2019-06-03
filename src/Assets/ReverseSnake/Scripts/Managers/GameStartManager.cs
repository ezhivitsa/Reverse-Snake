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
            GameStartEvent eventData;
            var entity = _world.CreateEntityWith<GameStartEvent>(out eventData);
            eventData.IsActive = isActive;
        }

        private void TriggerClearBoardEvents()
        {
            var wallsFilter = _world.GetFilter<EcsFilter<Wall>>();
            foreach (var ids in wallsFilter)
            {
                var entity = wallsFilter.Entities[ids];
                WallReactivitySystemOnRemove.CachedWalls[entity] = wallsFilter.Components1[ids];
                _world.RemoveEntity(entity);
            }
        }
    }
}
