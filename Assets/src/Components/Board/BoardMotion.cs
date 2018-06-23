using Assets.src.Enums;
using System.Linq;
using UnityEngine;
using Assets.src.Models;

public class BoardMotion : MonoBehaviour
{
    public GameObject SnakeStep;

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
            if (stepData.Number == 1)
            {
                RemoveAllSteps();
                AddNewStep(
                    position.ColumnPosition,
                    position.RowPosition,
                    stepData.StartNumber + 2,
                    stepData.StartNumber + 1,
                    direction
                );
            }
            else
            {
                // if next element is target
                // else
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
        var element = GetBoardElementAtPosition(columnPosition, rowPosition);

        var newPosition = GetNextPosition(columnPosition, rowPosition, direction);

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
    }
}
