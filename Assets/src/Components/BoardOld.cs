//using Assets.src.Enums;
//using Assets.src.Managers;
//using Assets.src.Models;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets.src
//{
//    public class Board : MonoBehaviour
//    {
//        public GameObject Element;
//        public GameObject Step;
//        public GameObject Target;

//        public Material WallClose;

//        private StepsManager _stepManager;
//        private TargetsManager _targetManager;
//        private ElementsManager _elementsManager;

//        public void Start()
//        {
//            _stepManager = new StepsManager(AppConstants.StartStepsCount);
//            _targetManager = new TargetsManager();
//            _elementsManager = new ElementsManager();
//            _positionsManager = new PositionsManager(AppConstants.FieldHeight, AppConstants.FieldWidth);

//            GenerateBoard();
//            AddStartStep();
//            AddStartTarget();
//        }

//        public void Update()
//        {
//            var direction = InputToDirection();
//            if (direction == DirectionEnum.None)
//            {
//                return;
//            }

//            var currentStep = _elementsManager.GetElementAtPosition(_stepManager.LastPosition());
//            var newStep = currentStep.GetNeighbor(direction);
//            DoStep(currentStep, newStep);
//        }

//        private void GenerateBoard()
//        {
//            for (var i = 0; i < AppConstants.FieldHeight; i++)
//            {
//                for (var j = 0; j < AppConstants.FieldWidth; j++)
//                {
//                    var position = _positionsManager.GetPositionVector(i, j);
//                    var positionNum = _positionsManager.GetPositionNumber(i, j);

//                    var element = new Element
//                    {
//                        Position = position,
//                        BoartPosition = positionNum,
//                    };

//                    Instantiate(Element, position, Quaternion.identity, transform);

//                    _elementsManager.Add(element, _positionsManager);
//                }
//            }
//        }

//        private void AddStartStep()
//        {
//            var element = _elementsManager.GetFreeRandomElement();
//            AddStepElement(element);
//        }

//        private void AddStartTarget()
//        {
//            var element = _elementsManager.GetFreeRandomElement();
//            var targetElement = new TargetElement
//            {
//                Value = AppConstants.TargetValue,
//            };
//            element.ChildTarget = targetElement;

//            AddObjectToElement(Target, element.BoartPosition);
//            _targetManager.Add(element.BoartPosition);
//        }

//        private void AddObjectToElement(GameObject gameObject, int positionNum)
//        {
//            var element = transform.GetChild(positionNum);
//            var position = element.position;

//            Instantiate(
//                gameObject,
//                new Vector3(
//                    position.x + AppConstants.BorderWidth - 5,
//                    position.y + 2,
//                    position.z + AppConstants.BorderWidth
//                ),
//                Quaternion.identity,
//                element
//            );
//        }

//        private DirectionEnum InputToDirection()
//        {
//            DirectionEnum direction = DirectionEnum.None;
//            if (Input.GetKeyDown(KeyCode.LeftArrow))
//            {
//                direction = DirectionEnum.Left;
//            }
//            else if (Input.GetKeyDown(KeyCode.RightArrow))
//            {
//                direction = DirectionEnum.Right;
//            }
//            else if (Input.GetKeyDown(KeyCode.UpArrow))
//            {
//                direction = DirectionEnum.Top;
//            }
//            else if (Input.GetKeyDown(KeyCode.DownArrow))
//            {
//                direction = DirectionEnum.Bottom;
//            }

//            return direction;
//        }

//        private void AddStepElement(Element element)
//        {
//            var textElement = Step.transform.GetChild(0).GetComponent<TextMesh>();
//            textElement.text = _stepManager.GetLeftSteps().ToString();

//            if (element.IsFree)
//            {
//                _stepManager.Add(element.BoartPosition);
//                AddObjectToElement(Step, element.BoartPosition);
//                element.ContainsStep = true;
//            }
//        }

//        private void RemoveElements(List<int> positions)
//        {
//            positions.ForEach((position) =>
//            {
//                var boardElement = _elementsManager.GetElementAtPosition(position);

//                var element = transform.GetChild(position);
//                var step = element.GetComponentInChildren<Step>();
//                var target = element.GetComponentInChildren<Target>();

//                if (step != null)
//                {
//                    boardElement.ContainsStep = false;
//                    Destroy(step.gameObject);
//                }
//                if (target != null)
//                {
//                    boardElement.ChildTarget = null;
//                    Destroy(target.gameObject);
//                }
//            });
//        }

//        private void AddWall()
//        {
//            var wall = _elementsManager.GetRandomOpenWall();
//            var boardElement = wall.Element;
//            var boardNeighbor = boardElement.GetNeighbor(wall.Side);

//            AddWallToElement(boardElement, wall.Side);
//            AddWallToElement(boardNeighbor, _positionsManager.GetReverseDirection(wall.Side));
//        }

//        private void AddWallToElement(Element boardElement, DirectionEnum side)
//        {
//            var element = transform.GetChild(boardElement.BoartPosition);

//            MonoBehaviour wallElement = null;
//            switch (side)
//            {
//                case DirectionEnum.Top:
//                    wallElement = element.GetComponentInChildren<RightWall>();
//                    break;

//                case DirectionEnum.Right:
//                    wallElement = element.GetComponentInChildren<BottomWall>();
//                    break;

//                case DirectionEnum.Bottom:
//                    wallElement = element.GetComponentInChildren<LeftWall>();
//                    break;

//                case DirectionEnum.Left:
//                    wallElement = element.GetComponentInChildren<TopWall>();
//                    break;
//            }

//            var renderer = wallElement.GetComponent<Renderer>();
//            renderer.material = WallClose;
//            wallElement.transform.localScale += new Vector3(0, 0, 3.5F);

//            boardElement.CloseWall(side);
//        }

//        private void DoStep(Element from, Element to)
//        {
//            var isAvailableWay = from.CanGoTo(to);

//            if (
//                to.ContainsStep ||
//                (to.ChildTarget != null && !_stepManager.CanGetTarget && _stepManager.HasSteps) ||
//                !isAvailableWay
//            )
//            {
//                return;
//            }

//            var hasSteps = _stepManager.HasSteps;
//            if (to.ChildTarget != null && _stepManager.CanGetTarget)
//            {
//                _stepManager.IncreaseAvailableSteps(to.ChildTarget.Value);
//            }

//            var isAchiveTarget = (to.ChildTarget != null && _stepManager.CanGetTarget);
//            var isReset = !hasSteps || isAchiveTarget;
//            if (isReset)
//            {
//                var stepPositions = _stepManager.Reset();
//                RemoveElements(stepPositions);

//                var targetPositions = _targetManager.Reset();
//                RemoveElements(targetPositions);
//            }

//            AddStepElement(to);

//            if (isReset)
//            {
//                AddStartTarget();
//            }

//            if (isReset && !isAchiveTarget)
//            {
//                AddWall();
//            }
//        }
//    }
//}
