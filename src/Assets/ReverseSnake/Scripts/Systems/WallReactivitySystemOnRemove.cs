using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.WallAlgorithm;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class WallReactivitySystemOnRemove : EcsReactiveSystem<Wall>, IEcsInitSystem
    {
        ReverseSnakeWorld _world;

        static public Dictionary<int, Wall> CachedWalls = new Dictionary<int, Wall>();

        private StateManager _stateManager;
        private Graph _graph;

        public void Initialize()
        {
            _stateManager = StateManager.GetInstance(_world);
            _graph = GraphGenertor.Generate();
        }

        public void Destroy()
        {
        }

        protected override EcsReactiveType GetReactiveType()
        {
            return EcsReactiveType.OnRemoved;
        }

        protected override void RunReactive()
        {
            var wallsToRemove = new List<Wall>();

            for (var i = 0; i < ReactedEntitiesCount; i++)
            {
                var entity = ReactedEntities[i];
                var wall = CachedWalls[entity];

                foreach (var transform in wall.Transforms)
                {
                    transform.gameObject.SetActive(false);
                }

                var nextPosition = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);

                wallsToRemove.Add(wall);
                _graph.AddEdge(wall.Row, wall.Column, nextPosition.Row, nextPosition.Column);

                CachedWalls.Remove(entity);
            }

            _stateManager.RemoveWalls(wallsToRemove);
        }
    }
}
