﻿using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs.UnityIntegration;
using System.Collections.Generic;

[EcsInject]
public class StepSystem : IEcsInitSystem, IEcsRunSystem
{
    const string BoardElementPath = "Objects/SnakeStep";

    EcsWorld _world = null;

    EcsFilter<BoardElement> _boardElements = null;
    EcsFilter<Step> _stepFilter = null;

    EcsFilter<MovementEvent> _movementsFilter = null;
    EcsFilter<ClearStepEvent> _clearEventFilter = null;

    static List<int> disabledSteps = new List<int>();

    void IEcsInitSystem.OnInitialize()
    {
        var enitity = _boardElements.Entities.ToList().RandomElement();
        var boardElement = _world.GetComponent<BoardElement>(enitity);

        CreateStep(boardElement, AppConstants.StartStepsCount, AppConstants.StartStepsCount, 1);
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleMovementEvent();
        HandleClearEvent();
    }

    void IEcsInitSystem.OnDestroy() { }

    private void CreateStep(BoardElement boardElement, int number, int startNumber, int round)
    {
        boardElement.ContainsSnakeStep = true;
        boardElement.Round = round;

        int entity = 0;
        Step element = null;
        UnityPrefabComponent prefab = null;

        if (disabledSteps.Count > 0)
        {
            entity = disabledSteps[0];
            element = _world.GetComponent<Step>(entity);
            prefab = _world.GetComponent<UnityPrefabComponent>(entity);

            disabledSteps.RemoveAt(0);
        }
        else
        {
            entity = _world.CreateEntity();
            element = _world.AddComponent<Step>(entity);
            prefab = _world.AddComponent<UnityPrefabComponent>(entity);
            prefab.Attach(BoardElementPath);
        }
        
        element.Row = boardElement.Row;
        element.Column = boardElement.Column;
        element.Number = number;
        element.StartNumber = startNumber;
        element.Round = round;
        element.Active = true;

        prefab.Prefab.transform.position = GetPositionVector(boardElement.Row, boardElement.Column);

        var textElement = prefab.Prefab.transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = element.Number.ToString();

        prefab.Prefab.SetActive(true);
    }

    private Vector3 GetPositionVector(int rowPos, int columnPos)
    {
        var result = new Vector3(
            AppConstants.BoardElementWidth * columnPos + AppConstants.BorderWidth * (columnPos + 1),
            2F,
            AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1)
        );

        return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
    }

    private void HandleMovementEvent()
    {
        for (var i = 0; i < _movementsFilter.EntitiesCount; i++)
        {
            var step = _movementsFilter.Components1[i];
            var boardElement = _boardElements.Components1
                .ToList()
                .Find(e => e.Row == step.Row && e.Column == step.Column);

            CreateStep(boardElement, step.Number, step.StartNumber, step.Round);

            _world.RemoveEntity(_movementsFilter.Entities[i]);
        }
    }

    private void HandleClearEvent()
    {
        for (var i = 0; i < _clearEventFilter.EntitiesCount; i++)
        {
            var eventData = _clearEventFilter.Components1[i];

            for (var j = 0; j < _stepFilter.EntitiesCount; j++)
            {
                var step = _stepFilter.Components1[j];
                if (step.Round == eventData.Round)
                {
                    var entity = _stepFilter.Entities[j];
                    var prefab = _world.GetComponent<UnityPrefabComponent>(entity);
                    var element = _world.GetComponent<Step>(entity);

                    element.Active = false;

                    prefab.Prefab.SetActive(false);
                    disabledSteps.Add(entity);
                }
            }

            _world.RemoveEntity(_clearEventFilter.Entities[i]);
        }
    }
}