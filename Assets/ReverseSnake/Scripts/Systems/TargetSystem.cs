using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs.UnityIntegration;
using Assets.ReverseSnake.Scripts.Enums;

[EcsInject]
public class TargetSystem : IEcsInitSystem, IEcsRunSystem
{
    const string BoardElementPath = "Objects/Target";

    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElements = null;
    EcsFilter<UpdateTargetEvent> _updateTargetEventsFilter = null;
    EcsFilter<Target> _targetFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        var boardElement = GetRandomBoardElement();

        var entity = _world.CreateEntity();
        var element = _world.AddComponent<Target>(entity);
        SetTargetData(element, boardElement, 1);

        var prefab = _world.AddComponent<UnityPrefabComponent>(entity);
        prefab.Attach(BoardElementPath);
        prefab.Prefab.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = prefab.Prefab.transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = element.Value.GetDescription();

        prefab.Prefab.SetActive(true);
    }

    void IEcsRunSystem.OnUpdate()
    {
        for (var i = 0; i < _updateTargetEventsFilter.EntitiesCount; i++)
        {
            var eventEntity = _updateTargetEventsFilter.Components1[i];

            for (var j = 0; j < _targetFilter.EntitiesCount; j++)
            {
                var boardElement = GetRandomBoardElement();
                SetTargetData(_targetFilter.Components1[j], boardElement, eventEntity.Round);
                UpdateTargetPosition(_targetFilter.Entities[j], boardElement);
            }
            _world.RemoveEntity(_updateTargetEventsFilter.Entities[i]);
        }
    }

    void IEcsInitSystem.OnDestroy() { }

    private BoardElement GetRandomBoardElement()
    {
        var enitity = _boardElements.Entities
            .ToList()
            .Where((e) => {
                var el = _world.GetComponent<BoardElement>(e);
                return !el.ContainsSnakeStep;
            })
            .RandomElement();
        return _world.GetComponent<BoardElement>(enitity);
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