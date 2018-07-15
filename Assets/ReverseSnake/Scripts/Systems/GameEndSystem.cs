using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Models;
using Assets.src;
using LeopotamGroup.Ecs;
using LeopotamGroup.Ecs.UnityIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[EcsInject]
public class GameEndSystem : IEcsRunSystem
{
    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElementsFilter = null;
    EcsFilter<Wall> _wallsFilter = null;
    EcsFilter<Step> _stepsFilter = null;
    EcsFilter<Target> _targetsFilter = null;

    EcsFilter<CheckGameEndEvent> _gameEndEventFilter = null;

    void IEcsRunSystem.OnUpdate()
    {
        for (var i = 0; i < _gameEndEventFilter.EntitiesCount; i++)
        {
            var eventData = _gameEndEventFilter.Components1[i];

            var directions = new List<DirectionEnum>
            {
                DirectionEnum.Top,
                DirectionEnum.Right,
                DirectionEnum.Bottom,
                DirectionEnum.Left,
            };
            var hasAvailablePosition = HasAvailablePosition(
                eventData.Column,
                eventData.Row,
                eventData.Round,
                eventData.Number,
                directions
            );
            if (!hasAvailablePosition)
            {
                HideAll();
                ShowGameOverScreen();
            }

            _world.RemoveEntity(_gameEndEventFilter.Entities[i]);
        }
    }

    private bool HasAvailablePosition(int column, int row, int round, int number, List<DirectionEnum> directions)
    {
        return directions.Any(d => IsElementAvailable(column, row, round, number, d));
    }

    private bool IsElementAvailable(int column, int row, int round, int number, DirectionEnum direction)
    {
        var position = PositionHelper.GetNextPosition(column, row, direction);
        var element = GetBoardElement(position);
        var wall = GetWall(position);

        if (
            wall.IsActive ||
            (element.ContainsSnakeStep && element.Round == round) ||
            (element.ContainsTarget && element.Round == round && !TargetHelper.CanGetTargetElement(number))
        )
        {
            return false;
        }

        return true;
    }

    private BoardElement GetBoardElement(PositionModel position)
    {
        return _boardElementsFilter
            .ToEntitiesList()
            .Find(e => e.Row == position.Row && e.Column == position.Column);
    }

    private Wall GetWall(PositionModel position)
    {
        return _wallsFilter
            .ToEntitiesList()
            .Find(e => e.Row == position.Row && e.Column == position.Column);
    }

    private void HideAll()
    {
        Action<int> hide = (entity) => {
            var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
            prefab.Prefab.SetActive(false);
        };

        _boardElementsFilter.ToEntitieNumbersList().ForEach(hide);
        _wallsFilter.ToEntitieNumbersList().ForEach(hide);
        _stepsFilter.ToEntitieNumbersList().ForEach(hide);
        _targetsFilter.ToEntitieNumbersList().ForEach(hide);

        var ui = GameObject.FindGameObjectsWithTag(AppConstants.UITag);
        ui.ToList().ForEach(el => el.SetActive(false));
    }

    private void ShowGameOverScreen()
    {
        var gameOver = GameObject.FindGameObjectsWithTag(AppConstants.GameOverTag);
        gameOver.ToList().ForEach(el => el.SetActive(true));
    }
}
