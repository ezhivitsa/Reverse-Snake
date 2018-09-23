using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class StateManager
    {
        private static StateManager instance;

        private EcsWorld _world;

        private StateManager(EcsWorld world)
        {
            _world = world;
        }

        public static StateManager GetInstance(EcsWorld world)
        {
            if (instance == null)
            {
                instance = new StateManager(world);
            }
            return instance;
        }

        public void SetScore(int score)
        {
            var eventData = _world.CreateEntityWith<StateSetScoreEvent>();
            eventData.Score = score;
        }

        public void AddStep(int row, int column, int number, int startNumber, int round)
        {
            var eventData = _world.CreateEntityWith<StateAddStepsEvent>();
            var step = new Step
            {
                Active = true,
                Row = row,
                Column = column,
                Number = number,
                StartNumber = startNumber,
                Round = round,
            };
            eventData.Steps = new List<Step> { step };
        }

        public void RemoveSteps(List<Step> steps)
        {
            var eventData = _world.CreateEntityWith<StateRemoveStepsEvent>();
            eventData.Steps = steps;
        }

        public void AddTarget(int row, int column, TargetValueEnum value, int round)
        {
            var eventData = _world.CreateEntityWith<StateAddTargetsEvent>();

            var target = new Target
            {
                Column = column,
                Row = row,
                Round = round,
                Value = value,
            };
            eventData.Targets = new List<Target> { target };
        }

        public void RemoveTarget(int row, int column, TargetValueEnum value, int round)
        {
            var eventData = _world.CreateEntityWith<StateRemoveTargetsEvent>();

            var target = new Target
            {
                Column = column,
                Row = row,
                Round = round,
                Value = value,
            };
            eventData.Targets = new List<Target> { target };
        }

        public void AddWalls(List<Wall> walls)
        {
            var eventData = _world.CreateEntityWith<StateAddWallsEvent>();
            eventData.Walls = walls;
        }

        public void RemoveWalls(List<Wall> walls)
        {
            var eventData = _world.CreateEntityWith<StateRemoveWallsEvent>();
            eventData.Walls = walls;
        }

        public void Clear()
        {
            _world.CreateEntityWith<StateClearEvent>();
        }

        public void LoadFromState(State state)
        {
            var stateEventData = _world.CreateEntityWith<StateLoadEvent>();
            stateEventData.State = state;

            var scoreEventData = _world.CreateEntityWith<ScoreSetEvent>();
            scoreEventData.Amount = state.Score;
            scoreEventData.Silent = true;

            var target = state.Targets[0];
            var targetEventData = _world.CreateEntityWith<UpdateTargetEvent>();
            targetEventData.Column = target.Column;
            targetEventData.Row = target.Row;
            targetEventData.Round = target.Round;
            targetEventData.Value = target.Value;
            targetEventData.Silent = true;

            var stepsEventData = _world.CreateEntityWith<CreateStepsEvent>();
            stepsEventData.Steps = state.Steps;

            var wallsEventData = _world.CreateEntityWith<CreateWallsEvent>();
            wallsEventData.Walls = state.ActiveWalls;
        }
    }
}
