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

    const string DefaultTargetPath = "Objects/DefaultTarget";
    const string AddTailRemoveTwoWallTarget = "Objects/AddTailRemoveTwoWallTarget";
    const string RemoveTailAddWallTarget = "Objects/RemoveTailAddWallTarget";

    const string MaterialsPath = "Materials/Textures";

    ReverseSnakeWorld _world = null;
    
    EcsFilter<Target> _targetFilter = null;
    EcsFilter<Wall> _wallFilter = null;

    EcsFilter<UpdateTargetEvent> _updateTargetEventsFilter = null;
    EcsFilter<ShowTargetEvent> _showTargetEventsFilter = null;

    private GameObject _gameElements;

    private Transform _defaultTarget;
    private Transform _addTailRemoveTwoWallTarget;
    private Transform _removeTailAddWallTarget;

    public void Initialize()
    {
        _stateManager = StateManager.GetInstance(_world);
        _gameElements = GameObject.FindGameObjectWithTag(AppConstants.GameElementsTag);

        InitializeTargets();

        var boardElement = GetRandomBoardElement(1);

        Target element = _world.CreateEntityWith<Target>();

        if (!GameStartup.LoadState)
        {
            SetTargetData(element, boardElement, AppConstants.FirstRound);
            UpdatePrefab(element, boardElement);
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
        if (element.Transform != null)
        {
            element.Transform.gameObject.SetActive(false);
        }

        var target = GetTargetElement(element.Value);
        target.gameObject.SetActive(true);
        element.Transform = target.transform;

        target.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        if (element.Value == TargetValueEnum.AddWall || element.Value == TargetValueEnum.RemoveWall)
        {
            var textElement = element.Transform.GetChild(0).GetComponent<TextMesh>();
            textElement.text = element.Value.GetDescription();
            textElement.color = GetTargetColor(element.Value);

            var renderer = element.Transform.GetChild(1).GetComponent<Renderer>();
            var name = element.Value.GetTextureName();
            var material = (Material)Resources.Load($"{MaterialsPath}/{name}", typeof(Material));
            renderer.material = material;
        }        
    }

    private Vector3 GetPositionVector(int rowPos, int columnPos)
    {
        var result = new Vector3(
            AppConstants.BoardElementWidth * columnPos + AppConstants.BorderWidth * (columnPos + 1),
            0.1F,
            AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1)
        );

        return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
    }

    private Color GetTargetColor(TargetValueEnum value)
    {
        switch (value)
        {
            case TargetValueEnum.AddWall:
                return new Color32(51, 135, 77, 255);

            case TargetValueEnum.RemoveWall:
                return new Color32(220, 85, 98, 225);

            default:
                return Color.white;
        }
    }

    private Transform GetTargetElement(TargetValueEnum value)
    {
        switch (value)
        {
            case TargetValueEnum.AddTailRemoveTwoWall:
                return _addTailRemoveTwoWallTarget;

            case TargetValueEnum.RemoveTailAddWall:
                return _removeTailAddWallTarget;

            default:
                return _defaultTarget;
        }
    }

    private void InitializeTargets()
    {
        var defaultTarget = (GameObject)Resources.Load(DefaultTargetPath, typeof(GameObject));
        _defaultTarget = GameObject.Instantiate(defaultTarget).transform;
        _defaultTarget.parent = _gameElements.transform;
        _defaultTarget.gameObject.SetActive(false);

        var addTailRemoveTwoWallTarget = (GameObject)Resources.Load(AddTailRemoveTwoWallTarget, typeof(GameObject));
        _addTailRemoveTwoWallTarget = GameObject.Instantiate(addTailRemoveTwoWallTarget).transform;
        _addTailRemoveTwoWallTarget.parent = _gameElements.transform;
        _addTailRemoveTwoWallTarget.gameObject.SetActive(false);

        var removeTailAddWallTarget = (GameObject)Resources.Load(RemoveTailAddWallTarget, typeof(GameObject));
        _removeTailAddWallTarget = GameObject.Instantiate(removeTailAddWallTarget).transform;
        _removeTailAddWallTarget.parent = _gameElements.transform;
        _removeTailAddWallTarget.gameObject.SetActive(false);
    }
}