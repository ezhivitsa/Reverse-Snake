using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using System.Linq;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class TargetReactivitySystemOnUpdate : EcsUpdateReactiveSystem<Target>, IEcsInitSystem
    {
        ReverseSnakeWorld _world = null;
        EcsFilter<Wall> _wallFilter = null;

        // ToDo: remove this static field
        public static Target OldTarget;

        private StateManager _stateManager;

        public void Initialize()
        {
            _stateManager = StateManager.GetInstance(_world);
        }

        public void Destroy()
        {
        }

        protected override void RunUpdateReactive()
        {
            for (var i = 0; i < ReactedEntitiesCount; i++)
            {
                var entity = ReactedEntities[i];
                var target = _world.GetComponent<Target>(entity);

                UpdateTarget(target);
            }
        }

        private void UpdateTarget(Target element)
        {
            element.Transform.gameObject.SetActive(false);

            var target = element.GetTargetElement();
            target.gameObject.SetActive(true);
            element.Transform = target.transform;
            element.Value = GetTargetValue();

            target.transform.position = TargetHelper.GetPositionVector(element.Row, element.Column);
            element.SetTexture();

            var oldBoardElement = _world.BoardElements
                .Find((el) => el.Row == OldTarget.Row && el.Column == OldTarget.Column);
            oldBoardElement.ContainsTarget = false;

            _stateManager.RemoveTarget(OldTarget.Row, OldTarget.Column, OldTarget.Value, OldTarget.Round);
            _stateManager.AddTarget(element.Row, element.Column, element.Value, element.Round);
        }

        private TargetValueEnum GetTargetValue()
        {
            var activeWalls = _wallFilter
                .ToEntitiesList()
                .Where(e => e.IsActive)
                .ToList()
                .Count;

            return TargetHelper.GetTargetValue(activeWalls / 2);
        }
    }
}
