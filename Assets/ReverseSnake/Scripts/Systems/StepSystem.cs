using Assets.src;
using Leopotam.Ecs;
using UnityEngine;
using Assets.ReverseSnake.Scripts.Extensions;
using System.Collections.Generic;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts;

[EcsInject]
public class StepSystem : IEcsInitSystem
{
    const string StepElementPath = "Objects/SnakeStep";

    ReverseSnakeWorld _world = null;

    private StepManager _manager;
    private StateManager _stateManager;

    public void Initialize()
    {
        _manager = StepManager.GetInstance(_world);
        _stateManager = StateManager.GetInstance(_world);

        if (!GameStartup.LoadState)
        {
            var boardElement = _world.BoardElements.RandomElement();
            _manager.CreateFirstStep(boardElement);

            _stateManager.AddStep(
                boardElement.Row,
                boardElement.Column,
                AppConstants.StartStepsCount,
                AppConstants.StartStepsCount,
                AppConstants.FirstRound
            );
        }
    }

    public void Destroy()
    {
    }
}