using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs.UnityIntegration;
using Assets.ReverseSnake.Scripts.Enums;
using System.Collections.Generic;

[EcsInject]
public class TargetSystem : IEcsInitSystem, IEcsRunSystem
{
    const string BoardElementPath = "Objects/Target";
    
    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElements = null;
    EcsFilter<Target> _targetFilter = null;

    EcsFilter<UpdateTargetEvent> _updateTargetEventsFilter = null;
    EcsFilter<ShowTargetEvent> _showTargetEventsFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        var boardElement = GetRandomBoardElement(1);

        var entity = _world.CreateEntity();
        var element = _world.AddComponent<Target>(entity);
        SetTargetData(element, boardElement, AppConstants.FirstRound);

        var prefab = _world.AddComponent<UnityPrefabComponent>(entity);
        prefab.Attach(BoardElementPath);
        prefab.Prefab.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = prefab.Prefab.transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = element.Value.GetDescription();

        prefab.Prefab.SetActive(true);
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleUpdateTarget();
        HandleShowTarget();
    }

    void IEcsInitSystem.OnDestroy()
    {
    }

    private void HandleUpdateTarget()
    {
        _updateTargetEventsFilter.HandleEvents(_world, (eventEntity) =>
        {
            for (var j = 0; j < _targetFilter.EntitiesCount; j++)
            {
                BoardElement boardElement = null;
                if (eventEntity.Column != null && eventEntity.Row != null)
                {
                    boardElement = GetBoardElement(eventEntity.Column, eventEntity.Row);
                }
                else
                {
                    boardElement = GetRandomBoardElement(eventEntity.Round);
                }
                SetTargetData(_targetFilter.Components1[j], boardElement, eventEntity.Round);
                UpdateTargetPosition(_targetFilter.Entities[j], boardElement);
            }
        });
    }

    private void HandleShowTarget()
    {
        _showTargetEventsFilter.HandleEvents(_world, (eventData) =>
        {
            _targetFilter.ToEntitieNumbersList().ForEach((entity) =>
            {
                var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
                prefab.Prefab.SetActive(eventData.IsActive);
            });
        });
    }

    private BoardElement GetBoardElement(int? column, int? row)
    {
        return _boardElements
            .ToEntitiesList()
            .Find((el) => el.Column == column && el.Row == row);
    }

    private BoardElement GetRandomBoardElement(int round)
    {
        return _boardElements
            .ToEntitiesList()
            .Where((el) => {
                return !el.ContainsSnakeStep || el.Round != round;
            })
            .RandomElement();
    }

    private void SetTargetData(Target element, BoardElement boardElement, int round)
    {
        element.Row = boardElement.Row;
        element.Column = boardElement.Column;
        element.Value = TargetValueEnum.AddWall;
        element.Round = round;

        boardElement.ContainsTarget = true;
        boardElement.Round = round;
    }

    private void UpdateTargetPosition(int entity, BoardElement boardElement)
    {
        var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
        prefab.Prefab.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);
    }

    private Vector3 GetPositionVector(int rowPos, int columnPos)
    {
        var result = new Vector3(
            AppConstants.BoardElementWidth * columnPos + AppConstants.BorderWidth * (columnPos + 1),
            2F,
            AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1)
        );

        return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
    }
}