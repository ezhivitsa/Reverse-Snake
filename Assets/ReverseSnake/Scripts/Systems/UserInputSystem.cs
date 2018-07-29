using Assets.ReverseSnake.Scripts.Enums;
using LeopotamGroup.Ecs;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using System.Collections.Generic;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Extensions;

[EcsInject]
sealed class UserInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private bool _isGameActive = false;

    EcsWorld _world = null;

    EcsFilter<Step> _stepFilter = null;
    EcsFilter<Wall> _wallFilter = null;
    EcsFilter<BoardElement> _boardElementsFilter = null;

    EcsFilter<GameStartEvent> _gameStartFilter = null;

    private GameManager _manager;

    void IEcsInitSystem.OnInitialize()
    {
        _manager = new GameManager(_world);
        _isGameActive = true;
    }

    void IEcsInitSystem.OnDestroy()
    {
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleGameStartEvent();

        if (!_isGameActive)
        {
            return;
        }

        var direction = InputHelper.GetInputArg();
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
                    _manager.TargetReached(lastStep, direction);
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

    private void HandleGameStartEvent()
    {
        _gameStartFilter.HandleEvents(_world, (gameStart) =>
        {
            _isGameActive = gameStart.IsActive;
        });
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
        return GetWall(row, column, direction).IsActive;
    }

    private Wall GetWall(int row, int column, DirectionEnum direction)
    {
        List<Wall> wallsList = new List<Wall>();
        for (var i = 0; i < _wallFilter.EntitiesCount; i++)
        {
            wallsList.Add(_wallFilter.Components1[i]);
        }

        return wallsList.Find(w => {
            return w.Row == row && w.Column == column && w.Direction == direction;
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

        for (var i = 0; i < _boardElementsFilter.EntitiesCount; i++)
        {
            var element = _boardElementsFilter.Components1[i];
            if (element.Row == newPosition.Row && element.Column == newPosition.Column)
            {
                return element;
            }
        }

        return null;
    }
}