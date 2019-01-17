using Assets.src;
using Leopotam.Ecs;
using UnityEngine;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts;

[EcsInject]
public class WallSystem : IEcsInitSystem, IEcsRunSystem
{
    ReverseSnakeWorld _world = null;
    EcsFilter<ShowWallEvent> _showEventFilter = null;

    private GameObject _grid;

    public void Initialize()
    {
        _grid = GameObject.FindGameObjectWithTag(AppConstants.GridTag);
    }

    public void Run()
    {
        HandleShowWallEvent();
    }

    public void Destroy() { }

    private void HandleShowWallEvent()
    {
        _showEventFilter.HandleEvents(_world, (eventData) => {
            _grid.SetActive(eventData.IsActive);
        });
    }
}