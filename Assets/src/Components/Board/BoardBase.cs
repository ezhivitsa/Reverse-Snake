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

        GameObject target = Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var targetPosition = target.GetComponent<TargetPosition>();
        targetPosition.RowPosition = position.RowPosition;
        targetPosition.ColumnPosition = position.ColumnPosition;

        var targetData = target.GetComponent<TargetData>();
        targetData.Value = AppConstants.TargetValue;

        data.ContainsTarget = true;
    }

    protected BoardElement GetRandomAvailableElement()
    {
        return GetAvailableElements().RandomElement();
    }
}
