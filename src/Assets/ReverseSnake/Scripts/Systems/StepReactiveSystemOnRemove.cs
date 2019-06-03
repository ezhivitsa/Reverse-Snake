using Assets.ReverseSnake.Scripts.Managers;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class StepReactiveSystemOnRemove : EcsReactiveSystem<Step>, IEcsInitSystem
    {
        ReverseSnakeWorld _world = null;

        private CachedComponentsManager _disabledStepsManager;
        private StateManager _stateManager;

        static public Dictionary<EcsEntity, Step> CachedSteps = new Dictionary<EcsEntity, Step>();

        public void Initialize()
        {
            _disabledStepsManager = CachedComponentsManager.GetInstance();
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
            var stepsToRemove = new List<Step>();
            foreach (var entity in this)
            {
                var element = CachedSteps[entity];

                element.Active = false;
                element.Transform.gameObject.SetActive(false);
                _disabledStepsManager.AddAvailableStep(element.Transform.gameObject);

                stepsToRemove.Add(element);

                CachedSteps.Remove(entity);
            }

            _stateManager.RemoveSteps(stepsToRemove);
        }
    }
}
