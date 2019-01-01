﻿using Assets.ReverseSnake.Scripts.Managers;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class StepReactiveSystemOnAdd : EcsReactiveSystem<Step>, IEcsInitSystem
    {
        const string StepElementPath = "Objects/SnakeStep";

        ReverseSnakeWorld _world = null;

        private StepManager _manager;
        private StateManager _stateManager;
        private DisabledStepsManager _disabledStepsManager;

        private GameObject _gameElements;

        public void Initialize()
        {
            _manager = StepManager.GetInstance(_world);
            _stateManager = StateManager.GetInstance(_world);
            _disabledStepsManager = DisabledStepsManager.GetInstance();

            _gameElements = GameObject.FindGameObjectWithTag(AppConstants.GameElementsTag);
        }

        public void Destroy()
        {
            _disabledStepsManager.Clear();
        }

        protected override EcsReactiveType GetReactiveType()
        {
            return EcsReactiveType.OnAdded;
        }

        protected override void RunReactive()
        {
            for (var i = 0; i < ReactedEntitiesCount; i++)
            {
                var entity = ReactedEntities[i];
                var step = _world.GetComponent<Step>(entity);

                var boardElement = _world.BoardElements
                    .Find(e => e.Row == step.Row && e.Column == step.Column);

                CreateStep(boardElement, step);

                if (!step.Silent)
                {
                    _stateManager.AddStep(step.Row, step.Column, step.Number, step.StartNumber, step.Round);
                    _manager.StepCreated(step.Row, step.Column, step.Number, step.Round);
                }
            }
        }

        private void CreateStep(BoardElement boardElement, Step element)
        {
            boardElement.ContainsSnakeStep = true;
            boardElement.Round = element.Round;
            
            Transform transform = null;

            var stepGameObject = _disabledStepsManager.GetAvailableStep();
            if (stepGameObject != null)
            {
                transform = stepGameObject.transform;
            }
            else
            {
                var stepObject = (GameObject)Resources.Load(StepElementPath, typeof(GameObject));
                transform = GameObject.Instantiate(stepObject).transform;
                transform.parent = _gameElements.transform;
            }
            
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
                0.1F,
                AppConstants.BoardElementWidth * rowPos + AppConstants.BorderWidth * (rowPos + 1)
            );

            return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
        }

        private string GetStepText(int number)
        {
            return (number - 1).ToString();
        }
    }
}
