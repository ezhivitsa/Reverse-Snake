using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Systems;
using Assets.src;
using Leopotam.Ecs;
using System.Linq;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameManager
    {
        private static GameManager _instance;
        private static EcsWorld _instanceWorld;

        private ReverseSnakeWorld _world;

        private GameManager(ReverseSnakeWorld world)
        {
            _world = world;
        }

        public static GameManager GetInstance(ReverseSnakeWorld world)
        {
            if (_instance == null || world != _instanceWorld)
            {
                _instance = new GameManager(world);
                _instanceWorld = world;
            }
            return _instance;
        }

        public void StartNewGame()
        {
            var stepBoardElement = _world.BoardElements.RandomElement();
            stepBoardElement.ContainsSnakeStep = true;
            stepBoardElement.Round = AppConstants.FirstRound;
            CreateFirstStep(stepBoardElement);

            var targetBoardElement = _world.BoardElements
                .Where(el => !el.ContainsSnakeStep)
                .RandomElement();
            targetBoardElement.ContainsTarget = true;
            targetBoardElement.Round = AppConstants.FirstRound;
            CreateFirstTarget(targetBoardElement);

            SetScore(0);
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

            TriggerUpdateTargetEvent(round);
            IncreaseScore();
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

            var boardElement = _world.BoardElements
                .Find((el) => el.Column == newPosition.Column && el.Row == newPosition.Row);
            boardElement.ContainsSnakeStep = true;
            boardElement.Round = round;

            var stepEvent = _world.CreateEntityWith<Step>();
            stepEvent.Row = newPosition.Row;
            stepEvent.Column = newPosition.Column;
            stepEvent.Number = number;
            stepEvent.StartNumber = startNumber;
            stepEvent.Round = round;
            stepEvent.Silent = false;
            stepEvent.DontUseSound = true;
        }

        private void TriggerUpdateTargetEvent(int round)
        {
            var boardElement = _world.BoardElements
                .Where((el) =>
                {
                    return !el.ContainsSnakeStep || el.Round != round;
                })
                .RandomElement();
            boardElement.ContainsTarget = true;
            boardElement.Round = round;

            var targetFilter = _world.GetFilter<EcsFilter<Target>>();
            var target = targetFilter.Components1[0];

            TargetReactivitySystemOnUpdate.OldTarget = new Target
            {
                Column = target.Column,
                Row = target.Row,
                Round = target.Round,
                Value = target.Value,
            };

            target.Column = boardElement.Column;
            target.Row = boardElement.Row;
            target.Round = round;
            target.Silent = false;

            _world.MarkComponentAsUpdated<Target>(targetFilter.Entities[0]);
        }

        private void TriggerClearBoardEvent(int round)
        {
            foreach (var element in _world.BoardElements)
            {
                if (element.Round == round)
                {
                    element.ContainsSnakeStep = false;
                    element.ContainsTarget = false;
                    element.Round = -1;
                }
            }

            var stepsFilter = _world.GetFilter<EcsFilter<Step>>();

            for (var i = 0; i < stepsFilter.EntitiesCount; i++)
            {
                var component = stepsFilter.Components1[i];
                if (component.Round == round)
                {
                    var entity = stepsFilter.Entities[i];
                    StepReactiveSystemOnRemove.CachedSteps[entity] = component;
                    _world.RemoveEntity(entity);
                }
            }
        }

        private void TriggerAddWallEvent()
        {
            var wall = _world.CreateEntityWith<Wall>();
            wall.Silent = false;
            wall.Row = -1;
            wall.Column = -1;
        }

        private void TriggerRemoveWallEvent(int wallsToRemove)
        {
            var wallsFilter = _world.GetFilter<EcsFilter<Wall>>();
            for (var i = 0; i < wallsToRemove; i += 1)
            {
                var num = Enumerable.Range(0, wallsFilter.EntitiesCount).RandomElement();
                var entity = wallsFilter.Entities[num];
                WallReactivitySystemOnRemove.CachedWalls[entity] = wallsFilter.Components1[num];
                _world.RemoveEntity(entity);
            }
        }

        private void CreateFirstStep(BoardElement boardElement)
        {
            var stepEvent = _world.CreateEntityWith<Step>();
            stepEvent.Row = boardElement.Row;
            stepEvent.Column = boardElement.Column;
            stepEvent.Number = AppConstants.StartStepsCount;
            stepEvent.StartNumber = AppConstants.StartStepsCount;
            stepEvent.Round = AppConstants.FirstRound;
            stepEvent.Silent = false;
            stepEvent.DontUseSound = true;
        }

        private void CreateFirstTarget(BoardElement boardElement)
        {
            var target = _world.CreateEntityWith<Target>();
            target.Row = boardElement.Row;
            target.Column = boardElement.Column;
            target.Round = AppConstants.FirstRound;
            target.Value = TargetValueEnum.AddWall;
            target.Silent = false;
        }

        private void SetScore(int amount)
        {
            var scoreFilter = _world.GetFilter<EcsFilter<Score>>();
            var score = scoreFilter.Components1[0];

            score.Amount = amount;
            score.Silent = false;
            _world.MarkComponentAsUpdated<Score>(scoreFilter.Entities[0]);
        }

        private void IncreaseScore()
        {
            var scoreFilter = _world.GetFilter<EcsFilter<Score>>();
            var score = scoreFilter.Components1[0];

            score.Amount += 1;
            score.Silent = false;
            _world.MarkComponentAsUpdated<Score>(scoreFilter.Entities[0]);
        }

        public void ClearAll()
        {
            foreach (var el in _world.BoardElements)
            {
                el.ContainsSnakeStep = false;
                el.ContainsTarget = false;
            }

            var stepsFilter = _world.GetFilter<EcsFilter<Step>>();
            for (var i = 0; i < stepsFilter.EntitiesCount; i++)
            {
                var entity = stepsFilter.Entities[i];
                StepReactiveSystemOnRemove.CachedSteps[entity] = stepsFilter.Components1[i];
                _world.RemoveEntity(entity);
            }

            var targetsFilter = _world.GetFilter<EcsFilter<Target>>();
            for (var i = 0; i < targetsFilter.EntitiesCount; i++)
            {
                var entity = targetsFilter.Entities[i];
                TargetReactiveSystemOnRemove.CachedTargets[entity] = targetsFilter.Components1[i];
                _world.RemoveEntity(entity);
            }
        }
    }
}
