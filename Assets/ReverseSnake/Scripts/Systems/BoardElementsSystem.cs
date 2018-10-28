using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Extensions;
using Leopotam.Ecs;

[EcsInject]
public class BoardElementsSystem : IEcsRunSystem
{
    ReverseSnakeWorld _world = null;

    EcsFilter<ClearBoardEvent> _clearEventFilter = null;

    public void Run()
    {
        HandleClearEvent();
    }

    private void HandleClearEvent()
    {
        _clearEventFilter.HandleEvents(_world, (clearEvent) =>
        {
            foreach (var element in _world.BoardElements)
            {
                if (element.Round == clearEvent.Round)
                {
                    element.ContainsSnakeStep = false;
                    element.ContainsTarget = false;
                    element.Round = -1;
                }
            }
        });
    }
}
