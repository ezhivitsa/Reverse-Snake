using Assets.src.Enums;
using System.Linq;
using UnityEngine;
using Assets.src.Models;
using System.Collections.Generic;
using Assets.src.Helpers;
using Assets.src.InputController;

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
        var direction = InputController.GetInputArg();
        if (direction == DirectionEnum.None)
        {
            return;
        }

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

                AddNewWall();
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

    private SnakeStepData GetLastSnakeStepData()
    {
        return GetComponentsInChildren<SnakeStepData>()
            .OrderBy(data => data.Number)
            .First();
    }

    private PositionModel GetNextPosition(int column, int row, DirectionEnum direction)
    {
        var bootstrap = GetComponent<BoardBootstrap>();
        return PositionHelper.GetNextPosition(
            bootstrap.Columns,
            bootstrap.Lines,
            column,
            row,
            direction
        );
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
        GenerateSnakeStep(SnakeStep, newPosition.Column, newPosition.Row, currentNumber, startNumber);
    }

    private void RemoveAllSteps()
    {
        BroadcastMessage("RemoveAllStepsMessage");
    }

    private void RemoveTarget()
    {
        BroadcastMessage("RemoveTargetMessage");
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
        var positions = GetComponentsInChildren<WallPosition>().ToList();
        return GetWall(positions, column, row, direction);
    }

    private WallData GetWall(List<WallPosition> positions, int column, int row, DirectionEnum direction)
    {
        var wall = positions.Find(w => {
            return w.RowPosition == row && w.ColumnPosition == column && w.Direction == direction;
        });

        return wall.GetComponent<WallData>();
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

        var availableWalls = notActiveWallPositions.Where((position) => {
            var wallsOnPosition = GetWallsOnPosition(
                notActiveWallPositions,
                position.ColumnPosition,
                position.RowPosition
           );

            var nextPos = GetNextPosition(position.ColumnPosition, position.RowPosition, position.Direction);
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

    private void CloseWall(WallPosition wallPosition, List<WallPosition> availableWalls)
    {
        var wall = wallPosition.GetComponent<WallData>();
        wall.IsActive = true;

        var nextPosition = GetNextPosition(
            wallPosition.ColumnPosition,
            wallPosition.RowPosition,
            wallPosition.Direction
        );

        var reverseDirection = DirectionHelper.GetReverseDirection(wallPosition.Direction);
        var reverseWallPos = GetWall(availableWalls, nextPosition.Column, nextPosition.Row, reverseDirection);

        var reverseWall = reverseWallPos.GetComponent<WallData>();
        reverseWall.IsActive = true;
    }
}
