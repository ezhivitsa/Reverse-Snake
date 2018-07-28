using Assets.ReverseSnake.Scripts.Extensions;
using Assets.src;
using LeopotamGroup.Ecs;
using LeopotamGroup.Ecs.UnityIntegration;
using UnityEngine;

[EcsInject]
public class BoardElementSystem : IEcsInitSystem, IEcsRunSystem
{
    const string BoardElementPath = "Objects/BoardElement";

    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElementFilter = null;

    EcsFilter<ClearBoardEvent> _clearEventFilter = null;
    EcsFilter<ShowBoardEvent> _showEventFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        for (var i = 0; i < AppConstants.Rows; i++)
        {
            for (var j = 0; j < AppConstants.Columns; j++)
            {
                var entity = _world.CreateEntity();
                var element = _world.AddComponent<BoardElement>(entity);
                element.Row = i;
                element.Column = j;
                element.ContainsSnakeStep = false;
                element.ContainsTarget = false;

                var prefab = _world.AddComponent<UnityPrefabComponent>(entity);
                prefab.Attach(BoardElementPath);
                prefab.Prefab.transform.position = GetPositionVector(i, j);
                prefab.Prefab.SetActive(true);
            }
        }
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleClearEvent();
        HandleShowEvent();
    }

    void IEcsInitSystem.OnDestroy () { }

    private void HandleClearEvent()
    {
        _clearEventFilter.HandleEvents(_world, (clearEvent) =>
        {
            for (var j = 0; j < _boardElementFilter.EntitiesCount; j++)
            {
                var element = _boardElementFilter.Components1[j];
                if (element.Round == clearEvent.Round)
                {
                    element.ContainsSnakeStep = false;
                    element.ContainsTarget = false;
                    element.Round = -1;
                }
            }
        });
    }

    private void HandleShowEvent()
    {
        _showEventFilter.HandleEvents(_world, (eventData) => {
            _boardElementFilter.ToEntitieNumbersList().ForEach((entity) =>
            {
                var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
                prefab.Prefab.SetActive(eventData.IsActive);
            });
        });
    }

    private Vector3 GetPositionVector(int rowPos, int columnPos)
    {
        return new Vector3(
            (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * columnPos - AppConstants.OffsetX,
            0.01F,
            (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * rowPos - AppConstants.OffsetZ
        );
    }
}