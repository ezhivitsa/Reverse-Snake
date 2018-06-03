using Assets.src.Managers;
using Assets.src.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.src
{
    public class Board : MonoBehaviour
    {
        public GameObject Element;

        public GameObject Step;

        public GameObject Target;
        
        private StepsManager _stepManager;
        private TargetsManager _targetManager;
        private ElementsManager _elementsManager;
        private WallsManager _wallsManager;

        public void Start()
        {
            _stepManager = new StepsManager(AppConstants.StartStepsCount);
            _targetManager = new TargetsManager();
            _elementsManager = new ElementsManager();
            _wallsManager = new WallsManager(AppConstants.AddNewWallOnStep);

            GenerateBoard();
            AddStartStep();
            AddStartTarget();
        }

        public void Update()
        {
            int position = GetMovePosition();
            if (position < 0)
            {
                return;
            }

            var hasSteps = _stepManager.HasSteps;

            var element = _elementsManager.GetElementAtPosition(position);
            if (element.ChildTarget != null && hasSteps)
            {
                _stepManager.IncreaseAvailableSteps(element.ChildTarget.Value);
            }

            var isReset = !hasSteps || element.ChildTarget != null;
            if (isReset)
            {
                var stepPositions = _stepManager.Reset();
                RemoveElements(stepPositions);

                var targetPositions = _targetManager.Reset();
                RemoveElements(targetPositions);
            }

            AddStepElement(element);

            if (isReset)
            {
                AddStartTarget();
            }

            _wallsManager.DoStep();
            if (_wallsManager.ShouldAddWall)
            {
                _wallsManager.AddWall();
                AddWall();
            }
        }

        private void GenerateBoard()
        {
            for (var i = 0; i < AppConstants.FieldHeight; i++)
            {
                for (var j = 0; j < AppConstants.FieldWidth; j++)
                {
                    var position = new Vector3(
                        (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * j - AppConstants.OffsetX,
                        0.01F,
                        (AppConstants.BoardElementWidth + AppConstants.BorderWidth) * i - AppConstants.OffsetZ
                    );
                    var positionNum = i * AppConstants.FieldHeight + j;

                    var element = new Element
                    {
                        Position = position,
                        BoartPosition = positionNum,
                    };

                    Instantiate(Element, position, Quaternion.identity, transform);

                    _elementsManager.Add(element);
                }
            }
        }

        private void AddStartStep()
        {
            var element = _elementsManager.GetFreeRandomElement();
            AddStepElement(element);
        }

        private void AddStartTarget()
        {
            var element = _elementsManager.GetFreeRandomElement();
            var targetElement = new TargetElement
            {
                Value = AppConstants.TargetValue,
            };
            element.ChildTarget = targetElement;

            AddObjectToElement(Target, element.BoartPosition);
            _targetManager.Add(element.BoartPosition);
        }

        private void AddObjectToElement(GameObject gameObject, int positionNum)
        {
            var element = transform.GetChild(positionNum);
            var position = element.position;

            Instantiate(
                gameObject,
                new Vector3(
                    position.x + AppConstants.BorderWidth,
                    position.y + 2,
                    position.z + AppConstants.BorderWidth
                ),
                Quaternion.identity,
                element
            );
        }

        private int GetMovePosition()
        {
            int position = -1;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                position = _stepManager.LastPosition() - AppConstants.FieldHeight;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                position = _stepManager.LastPosition() + AppConstants.FieldHeight;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                position = _stepManager.LastPosition() - 1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                position = _stepManager.LastPosition() + 1;
            }

            return position;
        }

        private void AddStepElement(Element element)
        {
            var textElement = Step.transform.GetChild(0).GetComponent<TextMesh>();
            textElement.text = _stepManager.GetLeftSteps().ToString();

            if (element.IsFree)
            {
                _stepManager.Add(element.BoartPosition);
                AddObjectToElement(Step, element.BoartPosition);
                element.ContainsStep = true;
            }
        }

        private void RemoveElements(List<int> positions)
        {
            positions.ForEach((position) =>
            {
                var boardElement = _elementsManager.GetElementAtPosition(position);

                var element = transform.GetChild(position);
                var step = element.GetComponentInChildren<Step>();
                var target = element.GetComponentInChildren<Target>();

                if (step != null)
                {
                    boardElement.ContainsStep = false;
                    Destroy(step.gameObject);
                }
                if (target != null)
                {
                    boardElement.ChildTarget = null;
                    Destroy(target.gameObject);
                }
            });
        }

        private void AddWall()
        {
            var wall = _elementsManager.GetRandomOpenWall();
            var element = wall.Element;
        }
    }
}
