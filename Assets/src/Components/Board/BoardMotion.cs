using Assets.src.Enums;
using System.Linq;
using UnityEngine;
using Assets.src.Models;
using System.Collections.Generic;
using Assets.src.Helpers;

public class BoardMotion : BoardBase
{
    public GameObject SnakeStep;
    public GameObject Target;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        var direction = InputToDirection();
        
        if (direction != DirectionEnum.None)
        {
            var stepData = GetLastSnakeStepData();
            var position = stepData.GetComponent<SnakeStepPosition>();

            var isWallClosed = IsWallOnWay(position.ColumnPosition, position.RowPosition, direction);
            if (isWallClosed)
            {
                return;
            }

            if (stepData.Number == 1)
            {
                RemoveAllSteps();
                RemoveTarget();

                AddNewStep(
                    position.ColumnPosition,
                    position.RowPosition,
                    stepData.StartNumber + 2,
                    stepData.StartNumber + 1,
                    direction
                );
                GenerateTarget(Target);

                AddNewWall();
            }
            else
            {
                var isTargetReached = IsTargetReached(
                    position.ColumnPosition,
                    position.RowPosition,
                    stepData.Number,
                    direction
                );
                if (isTargetReached)
                {
                    var targetValue = GetTargetValue();
                    RemoveAllSteps();
                    RemoveTarget();

                    AddNewStep(
                        position.ColumnPosition,
                        position.RowPosition,
                        stepData.StartNumber + 2 + targetValue,
                        stepData.StartNumber + 1 + targetValue,
                        direction
                    );
                    GenerateTarget(Target);
                }
                else
                {
                    var isNextBusy = IsNextPositionBusy(
                        position.ColumnPosition,
                        position.RowPosition,
                        direction
                    );
                    if (isNextBusy)
                    {
                        return;
                    }
                    AddNewStep(
                        position.ColumnPosition,
                        position.RowPosition,
                        stepData.Number,
                        stepData.StartNumber,
                        direction
                    );
                }
            }
        }
    }

    private DirectionEnum InputToDirection()
    {
        DirectionEnum direction = DirectionEnum.None;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = DirectionEnum.Left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = DirectionEnum.Right;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = DirectionEnum.Top;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = DirectionEnum.Bottom;
        }

        return direction;
    }

    private SnakeStepData GetLastSnakeStepData()
    {
        return GetComponentsInChildren<SnakeStepData>()
            .OrderBy(data => data.Number)
            .First();
    }

    private BoardElement GetBoardElementAtPosition(int column, int row)
    {
        return GetComponentsInChildren<BoardElement>()
            .First((element) =>
            {
                var position = element.GetComponentInChildren<BoardElementPosition>();
                return position.ColumnPosition == column && position.RowPosition == row;
            });
    }

    private PositionModel GetNextPosition(int column, int row, DirectionEnum direction)
    {
        var result = new PositionModel()
        {
            Column = column,
            Row = row,
        };

        switch (direction)
        {
            case DirectionEnum.Top:
                result.Row -= 1;
                break;

            case DirectionEnum.Right:
                result.Column -= 1;
                break;

            case DirectionEnum.Bottom:
                result.Row += 1;
                break;

            case DirectionEnum.Left:
                result.Column += 1;
                break;
        }

        var bootstrap = GetComponent<BoardBootstrap>();
        if (result.Column < 0)
        {
            result.Column = bootstrap.Columns - 1;
        }
        else if (result.Column >= bootstrap.Columns)
        {
            result.Column = 0;
        }
        else if (result.Row < 0)
        {
            result.Row = bootstrap.Lines - 1;
        }
        else if (result.Row >= bootstrap.Lines)
        {
            result.Row = 0;
        }

        return result;
    }

    private void AddNewStep(
        int columnPosition,
        int rowPosition,
        int currentNumber,
        int startNumber,
        DirectionEnum direction
    )
    {
        var newPosition = GetNextPosition(columnPosition, rowPosition, direction);

        var element = GetBoardElementAtPosition(newPosition.Column, newPosition.Row);
        var data = element.GetComponentInChildren<BoardElementData>();

        GameObject step = Instantiate(SnakeStep, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var stepPosition = step.GetComponent<SnakeStepPosition>();
        stepPosition.RowPosition = newPosition.Row;
        stepPosition.ColumnPosition = newPosition.Column;

        var newStepData = step.GetComponent<SnakeStepData>();
        newStepData.Number = currentNumber - 1;
        newStepData.StartNumber = startNumber;

        data.ContainsSnakeStep = true;
    }

    private void RemoveAllSteps()
    {
        var steps = GetComponentsInChildren<SnakeStepData>().ToList();
        steps.ForEach((step) =>
        {
            Destroy(step.gameObject);
        });

        GetComponentsInChildren<BoardElementData>()
            .ToList()
            .ForEach((element) =>
            {
                element.ContainsSnakeStep = false;
            });
    }

    private void RemoveTarget()
    {
        var target = GetComponentInChildren<TargetData>();
        Destroy(target.gameObject);

        GetComponentsInChildren<BoardElementData>()
            .ToList()
            .ForEach((element) =>
            {
                element.ContainsTarget = false;
            });
    }

    private bool IsTargetReached(
        int columnPosition,
        int rowPosition,
        int currentNumber,
        DirectionEnum direction
    )
    {
        var targetPosition = GetComponentInChildren<TargetPosition>();
        var newPosition = GetNextPosition(columnPosition, rowPosition, direction);
        return targetPosition.RowPosition == newPosition.Row &&
            targetPosition.ColumnPosition == newPosition.Column &&
            (currentNumber == 2 || currentNumber == 3);
    }

    private int GetTargetValue()
    {
        return GetComponentInChildren<TargetData>().Value;
    }

    private bool IsWallOnWay(int column, int row, DirectionEnum direction)
    {
        return GetWall(column, row, direction).IsActive;
    }

    private WallData GetWall(int column, int row, DirectionEnum direction)
    {
        var walls = GetComponentsInChildren<WallPosition>()
            .Where((w) =>
            {
                return w.RowPosition == row && w.ColumnPosition == column;
            })
            .ToList();

        var wall = walls
            .Select(w => w.GetComponent<WallData>())
            .First(w => w.Direction == direction);

        return wall;
    }

    private bool IsNextPositionBusy(int column, int row, DirectionEnum direction)
    {
        var newPosition = GetNextPosition(column, row, direction);
        var element = GetBoardElementAtPosition(newPosition.Column, newPosition.Row);

        var data = element.GetComponent<BoardElementData>();
        return data.ContainsTarget || data.ContainsSnakeStep;
    }

    private void AddNewWall()
    {
        var allWalls = GetComponentsInChildren<WallData>().ToList();
        var notActiveWalls = allWalls.Where(w => !w.IsActive).ToList();

        var notActiveWallPositions = notActiveWalls.Select(w => w.GetComponent<WallPosition>()).ToList();

        var availableWalls = notActiveWalls.Where((w) => {
            var position = w.GetComponent<WallPosition>();
            var wallsOnPosition = GetWallsOnPosition(
                notActiveWallPositions,
                position.ColumnPosition,
                position.ColumnPosition
           );

            var nextPos = GetNextPosition(position.ColumnPosition, position.RowPosition, w.Direction);
            var wallsOnNextPosition = GetWallsOnPosition(notActiveWallPositions, nextPos.Column, nextPos.Row);

            return wallsOnPosition.Count > 2 && wallsOnNextPosition.Count > 2;
        }).ToList();

        var randomWall = availableWalls.RandomElement();
        CloseWall(randomWall, availableWalls);
    }

    private List<WallPosition> GetWallsOnPosition(List<WallPosition> walls, int column, int row)
    {
        return walls
            .Where(w => w.ColumnPosition == column && w.RowPosition == row)
            .ToList();
    }

    private void CloseWall(WallData wall, List<WallData> availableWalls)
    {
        var wallPosition = wall.GetComponent<WallPosition>();
        var nextPosition = GetNextPosition(
            wallPosition.ColumnPosition,
            wallPosition.RowPosition,
            wall.Direction
        );

        wall.IsActive = true;

        var reverseDirection = DirectionHelper.GetReverseDirection(wall.Direction);
        var reverseWall = availableWalls.Find((w) =>
        {
            var pos = w.GetComponent<WallPosition>();
            return pos.ColumnPosition == nextPosition.Column &&
                pos.RowPosition == nextPosition.Row &&
                w.Direction == reverseDirection;
        });

        reverseWall.IsActive = true;
    }
}
