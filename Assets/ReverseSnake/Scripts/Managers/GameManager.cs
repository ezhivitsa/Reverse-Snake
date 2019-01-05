using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Systems;
using Leopotam.Ecs;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameManager
    {
        private static GameManager _instance;
        private static EcsWorld _instanceWorld;

        private EcsWorld _world;
        private EcsFilter<Step> _stepsFilter = null;

        private GameManager(EcsWorld world, EcsFilter<Step> stepsFilter)
        {
            _world = world;
            _stepsFilter = stepsFilter;
        }

        public static GameManager GetInstance(EcsWorld world, EcsFilter<Step> stepsFilter)
        {
            if (_instance == null || world != _instanceWorld)
            {
                _instance = new GameManager(world, stepsFilter);
                _instanceWorld = world;
            }
            return _instance;
        }

        public void NewStep(Step lastStep, DirectionEnum direction)
        {
            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.Number - 1,
                lastStep.StartNumber,
                lastStep.Round,
                direction
            );
        }

        public void TargetReached(Step lastStep, DirectionEnum direction, TargetValueEnum targetValue)
        {
            var round = lastStep.Round + 1;

            TriggerClearBoardEvent(lastStep.Round);

            TriggerUpdateTargetEvent(round);

            var stepChange = 0;
            switch (targetValue)
            {
                case TargetValueEnum.AddWall:
                    TriggerAddWallEvent();
                    break;

                case TargetValueEnum.RemoveWall:
                    TriggerRemoveWallEvent(1);
                    break;

                case TargetValueEnum.AddTailRemoveTwoWall:
                    TriggerRemoveWallEvent(2);
                    stepChange = 1;
                    break;

                case TargetValueEnum.RemoveTailAddWall:
                    TriggerAddWallEvent();
                    stepChange = -1;
                    break;
            }

            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.StartNumber + stepChange,
                lastStep.StartNumber + stepChange,
                round,
                direction
            );

            TriggerIncreaseScoreEvent();
        }

        public void NewRound(Step lastStep, DirectionEnum direction)
        {
            TriggerClearBoardEvent(lastStep.Round);

            var round = lastStep.Round + 1;
            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.StartNumber + 1,
                lastStep.StartNumber + 1,
                round,
                direction
            );
            TriggerUpdateTargetEvent(round);
        }

        private void TriggerAddNewStepEvent(
            int rowPosition,
            int columnPosition,
            int number,
            int startNumber,
            int round,
            DirectionEnum direction
        )
        {
            var newPosition = PositionHelper.GetNextPosition(rowPosition, columnPosition, direction);

            var stepEvent = _world.CreateEntityWith<Step>();
            stepEvent.Row = newPosition.Row;
            stepEvent.Column = newPosition.Column;
            stepEvent.Number = number;
            stepEvent.StartNumber = startNumber;
            stepEvent.Round = round;
            stepEvent.Silent = false;
            stepEvent.DontUseSound = false;
        }

        private void TriggerUpdateTargetEvent(int round)
        {
            var eventData = _world.CreateEntityWith<UpdateTargetEvent>();
            eventData.Round = round;
            eventData.Column = null;
            eventData.Row = null;
            eventData.Value = null;
            eventData.Silent = false;
        }

        private void TriggerClearBoardEvent(int round)
        {
            var eventData =_world.CreateEntityWith<ClearBoardEvent>();
            eventData.Round = round;

            for (var i = 0; i < _stepsFilter.EntitiesCount; i++)
            {
                var component = _stepsFilter.Components1[i];
                if (component.Round == round)
                {
                    var entity = _stepsFilter.Entities[i];
                    StepReactiveSystemOnRemove.CachedSteps[entity] = component;
                    _world.RemoveEntity(entity);
                }
            }
        }

        private void TriggerAddWallEvent()
        {
            _world.CreateEntityWith<AddWallEvent>();
        }

        private void TriggerRemoveWallEvent(int wallsToRemove)
        {
            var data = _world.CreateEntityWith<RemoveWallEvent>();
            data.Walls = wallsToRemove;
        }

        private void TriggerIncreaseScoreEvent()
        {
            var data = _world.CreateEntityWith<ScoreChangeEvent>();
            data.Amount = 1;
        }
    }
}
