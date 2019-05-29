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
            var entity = _world.CreateEntityWith<StateSetScoreEvent>(out StateSetScoreEvent eventData);
            eventData.Score = score;
        }

        public void AddStep(int row, int column, int number, int startNumber, int round)
        {
            var entity = _world.CreateEntityWith<StateAddStepsEvent>(out StateAddStepsEvent eventData);
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
            var entity = _world.CreateEntityWith<StateRemoveStepsEvent>(out StateRemoveStepsEvent eventData);
            eventData.Steps = steps;
        }

        public void AddTarget(int row, int column, TargetValueEnum value, int round)
        {
            var entity = _world.CreateEntityWith<StateAddTargetsEvent>(out StateAddTargetsEvent eventData);

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
            var entity = _world.CreateEntityWith<StateRemoveTargetsEvent>(out StateRemoveTargetsEvent eventData);

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
            var entity = _world.CreateEntityWith<StateAddWallsEvent>(out StateAddWallsEvent eventData);
            eventData.Walls = walls;
        }

        public void RemoveWalls(List<Wall> walls)
        {
            var entity = _world.CreateEntityWith<StateRemoveWallsEvent>(out StateRemoveWallsEvent eventData);
            eventData.Walls = walls;
        }

        public void Clear()
        {
            _world.CreateEntityWith<StateClearEvent>(out StateClearEvent clearEvent);
        }

        public void LoadFromState(State state)
        {
            var entity = _world.CreateEntityWith<StateLoadEvent>(out StateLoadEvent stateEventData);
            stateEventData.State = state;

            var scoreFilter = _world.GetFilter<EcsFilter<Score>>();
            var score = scoreFilter.Components1[0];

            score.Amount = state.Score;
            score.Silent = true;
            _world.MarkComponentAsUpdated<Score>(scoreFilter.Entities[0]);

            var target = state.Targets[0];
            _world.CreateEntityWith<Target>(out Target targetEventData);
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
            CreateWalls(state.ActiveWalls);
        }

        private void CreateSteps(List<Step> steps)
        {
            foreach (var step in steps)
            {
                var entity = _world.CreateEntityWith<Step>(out Step stepEvent);
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

        private void CreateWalls(List<Wall> walls)
        {
            foreach (var wall in walls)
            {
                var entity = _world.CreateEntityWith<Wall>(out Wall wallEvent);
                wallEvent.Row = wall.Row;
                wallEvent.Column = wall.Column;
                wallEvent.Direction = wall.Direction;
                wallEvent.Silent = true;
            }
        }
    }
}
