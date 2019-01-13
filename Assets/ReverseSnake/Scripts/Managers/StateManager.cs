using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class StateManager
    {
        private static StateManager instance;
        private static EcsWorld instanceWorld;

        private ReverseSnakeWorld _world;

        private StateManager(ReverseSnakeWorld world)
        {
            _world = world;
        }

        public static StateManager GetInstance(ReverseSnakeWorld world)
        {
            if (instance == null || world != instanceWorld)
            {
                instance = new StateManager(world);
                instanceWorld = world;
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

            var scoreFilter = _world.GetFilter<EcsFilter<Score>>();
            var score = scoreFilter.Components1[0];

            score.Amount = state.Score;
            score.Silent = true;
            _world.MarkComponentAsUpdated<Score>(scoreFilter.Entities[0]);

            var target = state.Targets[0];
            var targetEventData = _world.CreateEntityWith<Target>();
            targetEventData.Row = target.Row;
            targetEventData.Column = target.Column;
            targetEventData.Round = target.Round;
            targetEventData.Value = target.Value;
            targetEventData.Silent = true;

            var targetBoardElement = _world.BoardElements
                .Find(el => el.Row == target.Row && el.Column == target.Column);
            targetBoardElement.ContainsTarget = true;
            targetBoardElement.Round = target.Round;

            CreateSteps(state.Steps);

            var wallsEventData = _world.CreateEntityWith<CreateWallsEvent>();
            wallsEventData.Walls = state.ActiveWalls;
        }

        private void CreateSteps(List<Step> steps)
        {
            foreach (var step in steps)
            {
                var stepEvent = _world.CreateEntityWith<Step>();
                stepEvent.Row = step.Row;
                stepEvent.Column = step.Column;
                stepEvent.Number = step.Number;
                stepEvent.StartNumber = step.StartNumber;
                stepEvent.Round = step.Round;
                stepEvent.Silent = true;
                stepEvent.DontUseSound = true;

                var boardElement = _world.BoardElements
                    .Find(el => el.Row == step.Row && el.Column == step.Column);
                boardElement.ContainsSnakeStep = true;
                boardElement.Round = step.Round;
            }
        }
    }
}
