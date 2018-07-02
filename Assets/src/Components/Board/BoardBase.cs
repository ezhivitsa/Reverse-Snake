using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.src;
using Assets.src.Helpers;

public class BoardBase : MonoBehaviour
{
    protected List<BoardElement> GetAvailableElements()
    {
        return GetComponentsInChildren<BoardElement>()
            .Where((element) =>
            {
                var data = element.GetComponentInChildren<BoardElementData>();
                return !data.ContainsSnakeStep && !data.ContainsTarget;
            })
            .ToList();
    }

    protected void GenerateTarget(GameObject gameObject)
    {
        var element = GetRandomAvailableElement();
        var position = element.GetComponentInChildren<BoardElementPosition>();
        var data = element.GetComponentInChildren<BoardElementData>();

        var targetData = GetComponentInChildren<TargetData>(true);

        GameObject target = targetData != null
            ? targetData.gameObject
            : Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;

        target.SetActive(true);

        var targetPosition = target.GetComponent<TargetPosition>();
        targetPosition.RowPosition = position.RowPosition;
        targetPosition.ColumnPosition = position.ColumnPosition;

        if (targetData == null)
        {
            targetData = target.GetComponent<TargetData>();
        }

        targetData.Value = AppConstants.TargetValue;

        data.ContainsTarget = true;
    }

    protected void GenerateSnakeStep(
        GameObject gameObject,
        int columnPosition,
        int rowPosition,
        int currentNumber,
        int startNumber
    )
    {
        var element = GetBoardElementAtPosition(columnPosition, rowPosition);
        var data = element.GetComponentInChildren<BoardElementData>();

        var stepData = GetInactiveStep();

        GameObject step = stepData != null
            ? stepData.gameObject
            : Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;

        step.SetActive(true);

        var stepPosition = step.GetComponent<SnakeStepPosition>();
        stepPosition.RowPosition = rowPosition;
        stepPosition.ColumnPosition = columnPosition;

        if (stepData == null)
        {
            stepData = step.GetComponent<SnakeStepData>();
        }

        stepData.Number = currentNumber - 1;
        stepData.StartNumber = startNumber;
        stepData.SetNumberText();

        data.ContainsSnakeStep = true;
    }

    protected BoardElement GetBoardElementAtPosition(int column, int row)
    {
        return GetComponentsInChildren<BoardElement>()
            .First((element) =>
            {
                var position = element.GetComponentInChildren<BoardElementPosition>();
                return position.ColumnPosition == column && position.RowPosition == row;
            });
    }

    protected BoardElement GetRandomAvailableElement()
    {
        return GetAvailableElements().RandomElement();
    }

    private SnakeStepData GetInactiveStep()
    {
        var steps = GetComponentsInChildren<SnakeStepData>(true);
        foreach (var step in steps)
        {
            if (!step.gameObject.activeSelf)
            {
                return step;
            }
        }

        return null;
    }
}
