using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using System.Collections.Generic;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts;

[EcsInject]
sealed class UserInputSystem : IEcsRunSystem, IEcsInitSystem
{
    private bool _isGameActive = false;

    ReverseSnakeWorld _world = null;

    EcsFilter<Step> _stepFilter = null;
    EcsFilter<Wall> _wallFilter = null;
    EcsFilter<Target> _targetFilter = null;

    EcsFilter<GameStartEvent> _gameStartFilter = null;

    private GameManager _manager;
    private InputHelper _inputHelper = new InputHelper();

    public void Initialize()
    {
        _manager = new GameManager(_world);
        _isGameActive = true;

        _inputHelper.Swipe += SwipeDone;
    }

    public void Destroy()
    {
        _inputHelper.Swipe -= SwipeDone;
    }

    public void Run()
    {
        var isGameActive = _isGameActive;
        HandleGameStartEvent();

        if (!isGameActive)
        {
            return;
        }

        _inputHelper.Update();
    }

    private void SwipeDone(object sender, SwipeEventArgs e)
    {
        if (!_isGameActive)
        {
            return;
        }

        var direction = e.direction;
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

    private void HandleGameStartEvent()
    {
        _gameStartFilter.HandleEvents(_world, (gameStart) =>
        {
            _inputHelper.Clear();
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

        var nextPosition = PositionHelper.GetNextPosition(row, column, direction);
        var reverseDirection = DirectionHelper.GetReverseDirection(direction);

        return wallsList.Find(w => {
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