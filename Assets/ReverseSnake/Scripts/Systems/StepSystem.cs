using Assets.src;
using Leopotam.Ecs;
using UnityEngine;
using Assets.ReverseSnake.Scripts.Extensions;
using System.Collections.Generic;
using Assets.ReverseSnake.Scripts.Managers;

[EcsInject]
public class StepSystem : IEcsInitSystem, IEcsRunSystem
{
    const string StepElementPath = "Objects/SnakeStep";

    EcsWorld _world = null;

    EcsFilterSingle<BoardElements> _boardElements = null;
    EcsFilter<Step> _stepFilter = null;

    EcsFilter<MovementEvent> _movementsFilter = null;
    EcsFilter<ClearStepEvent> _clearEventFilter = null;
    EcsFilter<CreateStepsEvent> _createEvents = null;

    private StepManager _manager;
    private StateManager _stateManager;

    private List<int> _disabledSteps = new List<int>();
    private GameObject _gameElements;

    public void Initialize()
    {
        _manager = new StepManager(_world);
        _stateManager = StateManager.GetInstance(_world);

        _gameElements = GameObject.FindGameObjectWithTag(AppConstants.GameElementsTag);

        if (!GameStartup.LoadState)
        {
            var boardElement = _boardElements.Data.Elements.RandomElement();
            CreateStep(boardElement, AppConstants.StartStepsCount, AppConstants.StartStepsCount, AppConstants.FirstRound);

            _stateManager.AddStep(
                boardElement.Row,
                boardElement.Column,
                AppConstants.StartStepsCount,
                AppConstants.StartStepsCount,
                AppConstants.FirstRound
            );
        }
    }

    public void Run()
    {
        HandleMovementEvent();
        HandleClearEvent();
        HandleCreateStepsEvent();
    }

    public void Destroy()
    {
    }

    private void CreateStep(BoardElement boardElement, int number, int startNumber, int round)
    {
        boardElement.ContainsSnakeStep = true;
        boardElement.Round = round;

        int entity = 0;
        Step element = null;
        Transform transform = null;

        if (_disabledSteps.Count > 0)
        {
            entity = _disabledSteps[0];
            element = _world.GetComponent<Step>(entity);
            transform = element.Transform;

            _disabledSteps.RemoveAt(0);
        }
        else
        {
            entity = _world.CreateEntityWith(out element);

            var stepObject = (GameObject)Resources.Load(StepElementPath, typeof(GameObject));
            transform = GameObject.Instantiate(stepObject).transform;
            transform.parent = _gameElements.transform;
        }
        
        element.Row = boardElement.Row;
        element.Column = boardElement.Column;
        element.Number = number;
        element.StartNumber = startNumber;
        element.Round = round;
        element.Active = true;
        element.Transform = transform;

        element.Transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = element.Transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = GetStepText(element.Number);

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

    private void HandleMovementEvent()
    {
        _movementsFilter.HandleEvents(_world, (step) => {
            var boardElement = _boardElements.Data.Elements
                .Find(e => e.Row == step.Row && e.Column == step.Column);

            CreateStep(boardElement, step.Number, step.StartNumber, step.Round);

            _stateManager.AddStep(step.Row, step.Column, step.Number, step.StartNumber, step.Round);
            _manager.StepCreated(step.Row, step.Column, step.Number, step.Round);
        });
    }

    private void HandleClearEvent()
    {
        _clearEventFilter.HandleEvents(_world, (eventData) => {
            var stepsToRemove = new List<Step>();

            for (var j = 0; j < _stepFilter.EntitiesCount; j++)
            {
                var step = _stepFilter.Components1[j];
                if (step.Round == eventData.Round)
                {
                    var entity = _stepFilter.Entities[j];
                    var element = _world.GetComponent<Step>(entity);

                    element.Active = false;
                    element.Transform.gameObject.SetActive(false);
                    _disabledSteps.Add(entity);

                    stepsToRemove.Add(element);
                }
            }

            _stateManager.RemoveSteps(stepsToRemove);
        });
    }

    private void HandleCreateStepsEvent()
    {
        _createEvents.HandleEvents(_world, (eventData) => {
            foreach(var step in eventData.Steps)
            {
                var boardElement = _boardElements.Data.Elements
                    .Find(e => e.Row == step.Row && e.Column == step.Column);

                CreateStep(boardElement, step.Number, step.StartNumber, step.Round);
            }
        });
    }

    private string GetStepText(int number)
    {
        return (number - 1).ToString();
    }
}