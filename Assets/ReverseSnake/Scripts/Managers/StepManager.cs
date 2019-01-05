using Assets.src;
using Leopotam.Ecs;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class StepManager
    {
        private static StepManager instance;
        private static EcsWorld instanceWorld;

        private EcsWorld _world;

        private StepManager(EcsWorld world)
        {
            _world = world;
        }

        public static StepManager GetInstance(EcsWorld world)
        {
            if (instance == null || world != instanceWorld)
            {
                instance = new StepManager(world);
                instanceWorld = world;
            }
            return instance;
        }

        public void StepCreated(int column, int row, int number, int round)
        {
            if (number != 1)
            {
                TriggerStepCreatedEvent(column, row, number, round);
            }
        }

        public void CreateFirstStep(BoardElement boardElement)
        {
            var stepEvent = _world.CreateEntityWith<Step>();

            stepEvent.Row = boardElement.Row;
            stepEvent.Column = boardElement.Column;
            stepEvent.Number = AppConstants.StartStepsCount;
            stepEvent.StartNumber = AppConstants.StartStepsCount;
            stepEvent.Round = AppConstants.FirstRound;
            stepEvent.DontUseSound = true;
            stepEvent.Silent = false;
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
