using Assets.ReverseSnake.Scripts.Managers;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class TargetReactiveSystemOnRemove : EcsReactiveSystem<Target>, IEcsInitSystem
    {
        static public Dictionary<EcsEntity, Target> CachedTargets = new Dictionary<EcsEntity, Target>();

        ReverseSnakeWorld _world = null;

        private StateManager _stateManager;

        public void Initialize()
        {
            _stateManager = StateManager.GetInstance(_world);
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
            foreach (var entity in this) {
                var element = CachedTargets[entity];
                element.Transform.gameObject.SetActive(false);

                CachedTargets.Remove(entity);
                _stateManager.RemoveTarget(element.Row, element.Column, element.Value, element.Round);

                var boardElement = _world.BoardElements.Find(e => e.Column == element.Column && e.Row == element.Row);
                boardElement.ContainsTarget = false;
            }
        }
    }
}
