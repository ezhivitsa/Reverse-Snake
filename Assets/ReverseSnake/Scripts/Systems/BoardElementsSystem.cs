﻿using Assets.ReverseSnake.Scripts.Extensions;
using Assets.src;
using LeopotamGroup.Ecs;
using System.Collections.Generic;

[EcsInject]
public class BoardElementsSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsWorld _world = null;

    EcsFilterSingle<BoardElements> _boardElements = null;

    EcsFilter<ClearBoardEvent> _clearEventFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        _boardElements.Data.Elements = new List<BoardElement>();

        for (var i = 0; i < AppConstants.Rows; i++)
        {
            for (var j = 0; j < AppConstants.Columns; j++)
            {
                _boardElements.Data.Elements.Add(new BoardElement
                {
                    Row = i,
                    Column = j,
                    ContainsSnakeStep = false,
                    ContainsTarget = false,
                });
            }
        }
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleClearEvent();
    }

    void IEcsInitSystem.OnDestroy() { }

    private void HandleClearEvent()
    {
        _clearEventFilter.HandleEvents(_world, (clearEvent) =>
        {
            _boardElements.Data.Elements.ForEach((element) =>
            {
                if (element.Round == clearEvent.Round)
                {
                    element.ContainsSnakeStep = false;
                    element.ContainsTarget = false;
                    element.Round = -1;
                }
            });
        });
    }
}