using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Helpers;
using LeopotamGroup.Ecs;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameManager
    {
        private EcsWorld _world;

        public GameManager(EcsWorld world)
        {
            _world = world;
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

            var movementEvent = _world.CreateEntityWith<MovementEvent>();
            movementEvent.Row = newPosition.Row;
            movementEvent.Column = newPosition.Column;
            movementEvent.Number = number;
            movementEvent.StartNumber = startNumber;
            movementEvent.Round = round;
        }

        private void TriggerUpdateTargetEvent(int round)
        {
            var eventData = _world.CreateEntityWith<UpdateTargetEvent>();
            eventData.Round = round;
            eventData.Column = null;
            eventData.Row = null;
        }

        private void TriggerClearBoardEvent(int round)
        {
            var eventData =_world.CreateEntityWith<ClearBoardEvent>();
            eventData.Round = round;

            var stepEventData = _world.CreateEntityWith<ClearStepEvent>();
            stepEventData.Round = round;
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
