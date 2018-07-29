﻿using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Models;
using Assets.src;
using LeopotamGroup.Ecs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[EcsInject]
public class GameEndSystem : IEcsInitSystem, IEcsRunSystem
{
    private GameStartManager _manager;

    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElementsFilter = null;
    EcsFilter<Wall> _wallsFilter = null;

    EcsFilter<Score> _scoreFilter = null;
    EcsFilter<GameOver> _gameOverFilter = null;

    EcsFilter<CheckGameEndEvent> _gameEndEventFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        _manager = new GameStartManager(_world);

        foreach (var ui in GameObject.FindGameObjectsWithTag(AppConstants.GameOverTag))
        {
            var gameOver = _world.CreateEntityWith<GameOver>();
            gameOver.GameObject = ui;
            ui.SetActive(false);

            Button btn = ui.GetComponentInChildren<Button>();
            btn.onClick.AddListener(OnNewGameClick);
        }
    }

    void IEcsRunSystem.OnUpdate()
    {
        _gameEndEventFilter.HandleEvents(_world, (eventData) => {
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
                ShowScoreUI(false);
                ShowGameOverScreen(true);

                _manager.EndGame(eventData.Round);
            }
        });
    }

    public void OnDestroy()
    {
    }

    private bool HasAvailablePosition(int column, int row, int round, int number, List<DirectionEnum> directions)
    {
        return directions.Any(d => IsElementAvailable(column, row, round, number, d));
    }

    private bool IsElementAvailable(int column, int row, int round, int number, DirectionEnum direction)
    {
        var position = PositionHelper.GetNextPosition(column, row, direction);
        var element = GetBoardElement(position);
        var wall = GetWall(position, DirectionHelper.GetReverseDirection(direction));

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

    private Wall GetWall(PositionModel position, DirectionEnum direction)
    {
        return _wallsFilter
            .ToEntitiesList()
            .Find(e => e.Row == position.Row && e.Column == position.Column && e.Direction == direction);
    }

    private void ShowGameOverScreen(bool isActive)
    {
        _gameOverFilter.ToEntitiesList().ForEach((entity) =>
        {
            entity.GameObject.SetActive(isActive);
        });
    }

    private void ShowScoreUI(bool isActive)
    {
        _scoreFilter.ToEntitiesList().ForEach((entity) =>
        {
            entity.GameObject.SetActive(isActive);
        });
    }

    private void OnNewGameClick()
    {
        ShowScoreUI(true);
        ShowGameOverScreen(false);

        var boardElements = _boardElementsFilter.ToEntitiesList();
        var targetElement = boardElements.RandomElement();
        boardElements.Remove(targetElement);

        var stepElement = boardElements.RandomElement();

        var targetPosition = new PositionModel
        {
            Row = targetElement.Row,
            Column = targetElement.Column,
        };
        var stepPosition = new PositionModel
        {
            Row = stepElement.Row,
            Column = stepElement.Column,
        };

        _manager.StartGame(targetPosition, stepPosition);
    }
}