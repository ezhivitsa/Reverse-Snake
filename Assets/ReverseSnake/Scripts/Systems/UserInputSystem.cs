using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using System.Collections.Generic;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts;
using UnityEngine;
using Assets.src;

[EcsInject]
sealed class UserInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private bool _isGameActive = false;

    ReverseSnakeWorld _world = null;

    EcsFilter<Step> _stepFilter = null;
    EcsFilter<Wall> _wallFilter = null;
    EcsFilter<Target> _targetFilter = null;

    EcsFilter<GameStartEvent> _gameStartFilter = null;
    EcsFilter<SwipeDoneEvent> _swipeDoneEventFilter = null;

    private GameManager _manager;
    private InputHelper _inputHelper;
    private GameObject _grid;

    public void Initialize()
    {
        _manager = GameManager.GetInstance(_world);
        _inputHelper = new InputHelper(_world);

        _grid = GameObject.FindGameObjectWithTag(AppConstants.GridTag);

        _isGameActive = true;
    }

    public void Destroy()
    {
    }

    public void Run()
    {
        var isGameActive = _isGameActive;
        HandleGameStartEvent();
        HandleSwipeDoneEvent();

        if (!isGameActive)
        {
            return;
        }

        _inputHelper.Update();
    }

    private void HandleSwipeDoneEvent()
    {
        _swipeDoneEventFilter.HandleEvents(_world, (eventData) =>
        {
            SwipeDone(eventData.Direction);
        });
    }

    private void HandleGameStartEvent()
    {
        _gameStartFilter.HandleEvents(_world, (gameStart) =>
        {
            _inputHelper.Clear();
            _isGameActive = gameStart.IsActive;
            _grid.SetActive(gameStart.IsActive);
        });
    }

    private void SwipeDone(DirectionEnum direction)
    {
        if (!_isGameActive)
        {
            return;
        }

        if (direction != DirectionEnum.None)
        {
            var lastStep = GetLastStep();
            var isWallClosed = IsWallOnWay(lastStep.Row, lastStep.Column, direction);
            if (isWallClosed)
            {
                return;
            }

            if (lastStep.Number == 1)
            {
                _manager.NewRound(lastStep, direction);
            }
            else
            {
                var isTargetReached = IsTargetReached(lastStep.Row, lastStep.Column, lastStep.Number, direction);
                if (isTargetReached)
                {
                    var newPosition = PositionHelper.GetNextPosition(lastStep.Row, lastStep.Column, direction);
                    var target = _targetFilter
                        .ToEntitiesList()
                        .Find(t => t.Row == newPosition.Row && t.Column == newPosition.Column);
                    _manager.TargetReached(lastStep, direction, target.Value);
                }
                else
                {
                    var isNextBusy = IsNextPositionBusy(lastStep.Row, lastStep.Column, direction);
                    if (isNextBusy)
                    {
                        return;
                    }

                    _manager.NewStep(lastStep, direction);
                }
            }
        }
    }

    private Step GetLastStep()
    {
        List<Step> stepsList = new List<Step>();
        for (var i = 0; i < _stepFilter.EntitiesCount; i++)
        {
            var entityData = _stepFilter.Components1[i];
            if (entityData.Active)
            {
                stepsList.Add(entityData);
            }
        }
        return stepsList
            .OrderBy(data => data.Number)
            .First();
    }

    private bool IsWallOnWay(int row, int column, DirectionEnum direction)
    {
        return GetWall(row, column, direction) != null;
    }

    private Wall GetWall(int row, int column, DirectionEnum direction)
    {
        var nextPosition = PositionHelper.GetNextPosition(row, column, direction);
        var reverseDirection = DirectionHelper.GetReverseDirection(direction);

        return _wallFilter.ToEntitiesList().FirstOrDefault(w => {
            return (w.Row == row && w.Column == column && w.Direction == direction) ||
             (w.Row == nextPosition.Row && w.Column == nextPosition.Column && w.Direction == reverseDirection);
        });
    }

    private bool IsNextPositionBusy(int row, int column, DirectionEnum direction)
    {
        var nextBoardElement = GetNextBoardElement(row, column, direction);
        return nextBoardElement.ContainsTarget || nextBoardElement.ContainsSnakeStep;
    }

    private bool IsTargetReached(int row, int column, int currentNumber, DirectionEnum direction)
    {
        var nextBoardElement = GetNextBoardElement(row, column, direction);
        
        return nextBoardElement.ContainsTarget && TargetHelper.CanGetTargetElement(currentNumber);
    }

    private BoardElement GetNextBoardElement(int row, int column, DirectionEnum direction)
    {
        var newPosition = PositionHelper.GetNextPosition(row, column, direction);
        return _world.BoardElements.Find((element) =>
        {
            return element.Row == newPosition.Row && element.Column == newPosition.Column;
        });
    }
}