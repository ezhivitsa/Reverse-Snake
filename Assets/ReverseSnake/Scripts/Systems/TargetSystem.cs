using Assets.src;
using Leopotam.Ecs;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts;

[EcsInject]
public class TargetSystem : IEcsInitSystem, IEcsRunSystem
{
    private StateManager _stateManager;

    const string TargetPath = "Objects/Target";

    ReverseSnakeWorld _world = null;
    
    EcsFilter<Target> _targetFilter = null;
    EcsFilter<Wall> _wallFilter = null;

    EcsFilter<UpdateTargetEvent> _updateTargetEventsFilter = null;
    EcsFilter<ShowTargetEvent> _showTargetEventsFilter = null;

    private GameObject _gameElements;

    public void Initialize()
    {
        _stateManager = StateManager.GetInstance(_world);
        _gameElements = GameObject.FindGameObjectWithTag(AppConstants.GameElementsTag);

        var boardElement = GetRandomBoardElement(1);

        Target element = _world.CreateEntityWith<Target>();

        var targetObject = (GameObject)Resources.Load(TargetPath, typeof(GameObject));
        element.Transform = GameObject.Instantiate(targetObject).transform;
        element.Transform.parent = _gameElements.transform;

        if (!GameStartup.LoadState)
        {
            SetTargetData(element, boardElement, AppConstants.FirstRound);
            UpdatePrefab(element, boardElement);
        }
        else
        {
            element.Transform.gameObject.SetActive(false);
        }
    }

    public void Run()
    {
        HandleUpdateTarget();
        HandleShowTarget();
    }

    void IEcsInitSystem.Destroy()
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
                UpdatePrefab(element, boardElement);
            }
        });
    }

    private void HandleShowTarget()
    {
        _showTargetEventsFilter.HandleEvents(_world, (eventData) =>
        {
            _targetFilter.ToEntitiesList().ForEach((entity) =>
            {
                entity.Transform.gameObject.SetActive(eventData.IsActive);
            });
        });
    }
    
    private BoardElement GetBoardElement(int? column, int? row)
    {
        return _world.BoardElements.Find((el) => el.Column == column && el.Row == row);
    }

    private BoardElement GetRandomBoardElement(int round)
    {
        return _world.BoardElements
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

    private void UpdatePrefab(Target element, BoardElement boardElement)
    {
        element.Transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = element.Transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = element.Value.GetDescription();

        element.Transform.gameObject.SetActive(true);
    }

    private Vector3 GetPositionVector(int rowPos, int columnPos)
    {
        var result = new Vector3(
            AppConstants.BoardElementWidth * columnPos + AppConstants.BorderWidth * (columnPos + 1),
            1F,
            AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1)
        );

        return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
    }
}