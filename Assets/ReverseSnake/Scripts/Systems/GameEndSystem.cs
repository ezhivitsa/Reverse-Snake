using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.IO;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Models;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[EcsInject]
public class GameEndSystem : IEcsInitSystem, IEcsRunSystem
{
    private GameStartManager _manager;
    private StateManager _stateManager;

    private const string _tryAgainWidget = "tryAgain";
    private const string _goToMainMenuWidget = "goToMainMenu";

    ReverseSnakeWorld _world = null;

    EcsFilter<Wall> _wallsFilter = null;
    EcsFilter<Score> _scoreFilter = null;
    EcsFilter<GameOver> _gameOverFilter = null;

    EcsFilter<CheckGameEndEvent> _gameEndEventFilter = null;

    EcsFilter<EcsUiClickEvent> _clickEvents = null;

    public void Initialize()
    {
        _manager = new GameStartManager(_world);
        _stateManager = StateManager.GetInstance(_world);

        var ui = GameObject.FindGameObjectWithTag(AppConstants.GameOverTag);
        var gameOver = _world.CreateEntityWith<GameOver>();
        gameOver.GameObject = ui;
        ui.SetActive(false);
    }

    public void Run()
    {
        HandleGameEnd();
        HandleUiClicks();
    }

    public void Destroy()
    {
        _gameOverFilter.ToEntitieNumbersList().ForEach(entity => {
            _world.RemoveEntity(entity);
        });
    }

    private void HandleGameEnd()
    {
        _gameEndEventFilter.HandleEvents(_world, (eventData) =>
        {
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

                var scoreEntity = _scoreFilter.ToEntitiesList().First();
                SaveLeaderboard.AddResultAndSave(scoreEntity.Amount);

                _stateManager.Clear();
            }
        });
    }

    private void HandleUiClicks()
    {
        for (var i = 0; i < _clickEvents.EntitiesCount; i++)
        {
            EcsUiClickEvent data = _clickEvents.Components1[i];
            switch (data.WidgetName)
            {
                case _tryAgainWidget:
                    OnNewGameClick();
                    break;

                case _goToMainMenuWidget:
                    OnGoToMainMenuClick();
                    break;
            }
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
        return _world.BoardElements.Find(e => e.Row == position.Row && e.Column == position.Column);
    }

    private Wall GetWall(PositionModel position, DirectionEnum direction)
    {
        var nextPosition = PositionHelper.GetNextPosition(position.Row, position.Column, direction);
        var reverseDirection = DirectionHelper.GetReverseDirection(direction);

        return _wallsFilter
            .ToEntitiesList()
            .Find(e =>
                (
                    e.Row == position.Row &&
                    e.Column == position.Column &&
                    e.Direction == direction
                ) ||
                (
                    e.Row == nextPosition.Row &&
                    e.Column == nextPosition.Column &&
                    e.Direction == reverseDirection
                )
            );
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
            entity.UI.SetActive(isActive);
        });
    }

    private void OnNewGameClick()
    {
        ShowScoreUI(true);
        ShowGameOverScreen(false);

        var boardElements = _world.BoardElements;

        var targetElement = boardElements.RandomElement();
        var stepElement = boardElements.Where(e => e != targetElement).RandomElement();

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

    private void OnGoToMainMenuClick()
    {
        SceneManager.LoadScene(AppConstants.MainMenuScene);
    }
}
