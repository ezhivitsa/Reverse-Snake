﻿using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Systems;
using Assets.src;
using Leopotam.Ecs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Managers
{
    sealed class GameManager
    {
        private static GameManager _instance;
        private static EcsWorld _instanceWorld;

        private ReverseSnakeWorld _world;

        private GameManager(ReverseSnakeWorld world)
        {
            _world = world;
        }

        public static GameManager GetInstance(ReverseSnakeWorld world)
        {
            if (_instance == null || world != _instanceWorld)
            {
                _instance = new GameManager(world);
                _instanceWorld = world;
            }
            return _instance;
        }

        public void StartNewGame()
        {
            var stepBoardElement = _world.BoardElements.RandomElement();
            stepBoardElement.ContainsSnakeStep = true;
            stepBoardElement.Round = AppConstants.FirstRound;
            CreateFirstStep(stepBoardElement);

            var targetBoardElement = _world.BoardElements
                .Where(el => !el.ContainsSnakeStep)
                .RandomElement();
            targetBoardElement.ContainsTarget = true;
            targetBoardElement.Round = AppConstants.FirstRound;
            CreateFirstTarget(targetBoardElement);

            SetScore(0);
        }

        public void NewStep(Step lastStep, DirectionEnum direction)
        {
            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.Number - 1,
                lastStep.StartNumber,
                lastStep.Round,
                direction
            );
        }

        public void TargetReached(Step lastStep, DirectionEnum direction, TargetValueEnum targetValue)
        {
            var round = lastStep.Round + 1;

            TriggerClearBoardEvent(lastStep.Round);

            var stepChange = 0;
            switch (targetValue)
            {
                case TargetValueEnum.AddWall:
                    TriggerAddWallEvent();
                    break;

                case TargetValueEnum.RemoveWall:
                    TriggerRemoveWallEvent(1);
                    break;

                case TargetValueEnum.AddTailRemoveTwoWall:
                    TriggerRemoveWallEvent(2);
                    stepChange = 1;
                    break;

                case TargetValueEnum.RemoveTailAddWall:
                    TriggerAddWallEvent();
                    stepChange = -1;
                    break;
            }

            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.StartNumber + stepChange,
                lastStep.StartNumber + stepChange,
                round,
                direction
            );

            TriggerUpdateTargetEvent(round);
            IncreaseScore();
        }

        public void NewRound(Step lastStep, DirectionEnum direction)
        {
            TriggerClearBoardEvent(lastStep.Round);

            var round = lastStep.Round + 1;
            TriggerAddNewStepEvent(
                lastStep.Row,
                lastStep.Column,
                lastStep.StartNumber + 1,
                lastStep.StartNumber + 1,
                round,
                direction
            );
            TriggerUpdateTargetEvent(round);
        }

        private void TriggerAddNewStepEvent(
            int rowPosition,
            int columnPosition,
            int number,
            int startNumber,
            int round,
            DirectionEnum direction
        )
        {
            var newPosition = PositionHelper.GetNextPosition(rowPosition, columnPosition, direction);

            var boardElement = _world.BoardElements
                .Find((el) => el.Column == newPosition.Column && el.Row == newPosition.Row);
            boardElement.ContainsSnakeStep = true;
            boardElement.Round = round;

            var entity = _world.CreateEntityWith<Step>(out Step stepEvent);
            stepEvent.Row = newPosition.Row;
            stepEvent.Column = newPosition.Column;
            stepEvent.Number = number;
            stepEvent.StartNumber = startNumber;
            stepEvent.Round = round;
            stepEvent.Silent = false;
            stepEvent.DontUseSound = true;
        }

        private void TriggerUpdateTargetEvent(int round)
        {
            var boardElement = _world.BoardElements
                .Where((el) => !el.ContainsSnakeStep || el.Round != round)
                .RandomElement();
            boardElement.ContainsTarget = true;
            boardElement.Round = round;
            
            var target = _world.FirstComponent<Target>();

            TargetReactivitySystemOnUpdate.OldTarget = new Target
            {
                Column = target.Column,
                Row = target.Row,
                Round = target.Round,
                Value = target.Value,
            };

            target.Column = boardElement.Column;
            target.Row = boardElement.Row;
            target.Round = round;
            target.Silent = false;

            _world.MarkComponentAsUpdated<Target>(_world.FirstEntity<Target>());
        }

        private void TriggerClearBoardEvent(int round)
        {
            _world.BoardElements.ClearElements(round);

            var stepsFilter = _world.GetFilter<EcsFilter<Step>>();

            foreach (var idx in stepsFilter)
            {
                var component = stepsFilter.Components1[idx];
                if (component.Round == round)
                {
                    var entity = stepsFilter.Entities[idx];
                    StepReactiveSystemOnRemove.CachedSteps[entity] = component;
                    _world.RemoveEntity(entity);
                }
            }
        }

        private void TriggerAddWallEvent()
        {
            var entity = _world.CreateEntityWith<Wall>(out Wall wall);
            wall.Silent = false;
            wall.Row = -1;
            wall.Column = -1;
        }

        private void TriggerRemoveWallEvent(int wallsToRemove)
        {
            var wallsFilter = _world.GetFilter<EcsFilter<Wall>>();

            var ids = new List<int>();
            var idsToRemove = new List<int>();
            foreach (var idx in wallsFilter)
            {
                ids.Add(idx);
            }

            for (var i = 0; i < wallsToRemove; i += 1)
            {
                var num = ids.RandomElement();
                idsToRemove.Add(num);
                ids.Remove(num);
            }

            foreach (var idx in idsToRemove)
            {
                var entity = wallsFilter.Entities[idx];
                WallReactivitySystemOnRemove.CachedWalls[entity] = wallsFilter.Components1[idx];
                _world.RemoveEntity(entity);
            }
        }

        private void CreateFirstStep(BoardElement boardElement)
        {
            var entity = _world.CreateEntityWith<Step>(out Step stepEvent);
            stepEvent.Row = boardElement.Row;
            stepEvent.Column = boardElement.Column;
            stepEvent.Number = AppConstants.StartStepsCount;
            stepEvent.StartNumber = AppConstants.StartStepsCount;
            stepEvent.Round = AppConstants.FirstRound;
            stepEvent.Silent = false;
            stepEvent.DontUseSound = true;
        }

        private void CreateFirstTarget(BoardElement boardElement)
        {
            var entity = _world.CreateEntityWith<Target>(out Target target);
            target.Row = boardElement.Row;
            target.Column = boardElement.Column;
            target.Round = AppConstants.FirstRound;
            target.Value = TargetValueEnum.AddWall;
            target.Silent = false;
        }

        private void SetScore(int amount)
        {
            var score = _world.FirstComponent<Score>();

            score.Amount = amount;
            score.Silent = false;
            _world.MarkComponentAsUpdated<Score>(_world.FirstEntity<Score>());
        }

        private void IncreaseScore()
        {
            var score = _world.FirstComponent<Score>();
            SetScore(score.Amount + 1);
        }

        public void ClearAll()
        {
            _world.BoardElements.ClearElements();

            var stepsFilter = _world.GetFilter<EcsFilter<Step>>();
            foreach (var idx in stepsFilter)
            {
                var entity = stepsFilter.Entities[idx];
                StepReactiveSystemOnRemove.CachedSteps[entity] = stepsFilter.Components1[idx];
                _world.RemoveEntity(entity);
            }

            var targetsFilter = _world.GetFilter<EcsFilter<Target>>();
            foreach (var idx in targetsFilter)
            {
                var entity = targetsFilter.Entities[idx];
                TargetReactiveSystemOnRemove.CachedTargets[entity] = targetsFilter.Components1[idx];
                _world.RemoveEntity(entity);
            }
        }
    }
}
