using Assets.src;
using Leopotam.Ecs;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts
{
    class ReverseSnakeWorld : EcsWorld
    {
        public List<BoardElement> BoardElements { get; set; }
        public State State { get; set; }

        public ReverseSnakeWorld()
        {
            BoardElements = new List<BoardElement>();
            State = new State();

            InitializeElements();
            InitializeState();
        }

        public T FirstComponent<T>() where T : class, new()
        {
            var filter = GetFilter<EcsFilter<T>>();
            return filter.Components1[0];
        }

        public EcsEntity FirstEntity<T>() where T : class, new()
        {
            var filter = GetFilter<EcsFilter<T>>();
            return filter.Entities[0];
        }

        private void InitializeElements()
        {
            for (var i = 0; i < AppConstants.Rows; i++)
            {
                for (var j = 0; j < AppConstants.Columns; j++)
                {
                    BoardElements.Add(new BoardElement
                    {
                        Row = i,
                        Column = j,
                        ContainsSnakeStep = false,
                        ContainsTarget = false,
                    });
                }
            }
        }

        private void InitializeState()
        {
            State.Targets = new List<Target>();
            State.Steps = new List<Step>();
            State.ActiveWalls = new List<Wall>();
            State.Score = 0;
        }
    }
}
