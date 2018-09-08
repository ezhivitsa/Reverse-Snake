using Assets.ReverseSnake.Scripts.Enums;
using LeopotamGroup.Ecs;
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
    }
}
