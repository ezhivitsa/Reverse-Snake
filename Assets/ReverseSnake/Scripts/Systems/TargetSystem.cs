using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs.UnityIntegration;
using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;

[EcsInject]
public class TargetSystem : IEcsInitSystem, IEcsRunSystem
{
    private StateManager _stateManager;

    const string BoardElementPath = "Objects/Target";
    
    EcsWorld _world = null;

    EcsFilterSingle<BoardElements> _boardElements = null;
    EcsFilter<Target> _targetFilter = null;
    EcsFilter<Wall> _wallFilter = null;

    EcsFilter<UpdateTargetEvent> _updateTargetEventsFilter = null;
    EcsFilter<ShowTargetEvent> _showTargetEventsFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        _stateManager = StateManager.GetInstance(_world);


        var boardElement = GetRandomBoardElement(1);

        Target element = null;
        var entity = _world.CreateEntityWith(out element);

        var prefab = _world.AddComponent<UnityPrefabComponent>(entity);
        prefab.Attach(BoardElementPath);

        if (!GameStartup.LoadState)
        {
            SetTargetData(element, boardElement, AppConstants.FirstRound);
            UpdatePrefab(entity, element, boardElement);
        }
        else
        {
            prefab.Prefab.SetActive(false);
        }
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

                var element = _targetFilter.Components1[j];
                SetTargetData(element, boardElement, eventEntity.Round, eventEntity.Value, eventEntity.Silent);
                UpdatePrefab(_targetFilter.Entities[j], element, boardElement);
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
        return _boardElements.Data.Elements.Find((el) => el.Column == column && el.Row == row);
    }

    private BoardElement GetRandomBoardElement(int round)
    {
        return _boardElements.Data.Elements
            .Where((el) => {
                return !el.ContainsSnakeStep || el.Round != round;
            })
            .RandomElement();
    }

    private void SetTargetData(
        Target element,
        BoardElement boardElement,
        int round,
        TargetValueEnum? value = null,
        bool silent = false
    )
    {
        if (element.Round != 0 && !silent)
        {
            _stateManager.RemoveTarget(element.Row, element.Column, element.Value, element.Round);
        }

        element.Row = boardElement.Row;
        element.Column = boardElement.Column;
        element.Value = value != null ? value.Value : GetTargetValue();
        element.Round = round;

        boardElement.ContainsTarget = true;
        boardElement.Round = round;

        if (!silent)
        {
            _stateManager.AddTarget(element.Row, element.Column, element.Value, element.Round);
        }
    }

    private TargetValueEnum GetTargetValue()
    {
        var activeWalls = _wallFilter
            .ToEntitiesList()
            .Where(e => e.IsActive)
            .ToList()
            .Count;

        return TargetHelper.GetTargetValue(activeWalls / 2);
    }

    private void UpdatePrefab(int entity, Target element, BoardElement boardElement)
    {
        var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
        prefab.Prefab.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = prefab.Prefab.transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = element.Value.GetDescription();

        prefab.Prefab.SetActive(true);
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