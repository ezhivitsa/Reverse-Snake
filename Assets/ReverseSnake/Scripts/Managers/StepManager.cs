using LeopotamGroup.Ecs;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class StepManager
    {
        private EcsWorld _world;

        public StepManager(EcsWorld world)
        {
            _world = world;
        }

        public void StepCreated(int column, int row, int number, int round)
        {
            if (number != 1)
            {
                TriggerStepCreatedEvent(column, row, number, round);
            }
        }

        private void TriggerStepCreatedEvent(int column, int row, int number, int round)
        {
            var eventData = _world.CreateEntityWith<CheckGameEndEvent>();
            eventData.Column = column;
            eventData.Row = row;
            eventData.Round = round;
            eventData.Number = number;
        }
    }
}
