using Assets.src.Enums;
using UnityEngine;
using Assets.src;

public class BoardBootstrap : BoardBase
{
    public int Lines;

    public int Columns;

    public GameObject Element;
    public GameObject Wall;
    public GameObject SnakeStep;
    public GameObject Target;

    // Use this for initialization
    void Start () {
        GenerateBoard();
        GenerateSnakeStep();
        GenerateTarget(Target);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GenerateBoard()
    {
        for (var i = 0; i < Lines; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                GameObject item = Instantiate(Element, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
                var position = item.GetComponent<BoardElementPosition>();
                position.RowPosition = i;
                position.ColumnPosition = j;
                
                GenerateWall(i, j, DirectionEnum.Top);
                GenerateWall(i, j, DirectionEnum.Bottom);
                GenerateWall(i, j, DirectionEnum.Left);
                GenerateWall(i, j, DirectionEnum.Right);
            }
        }
    }

    private void GenerateSnakeStep()
    {
        var element = GetRandomAvailableElement();
        var position = element.GetComponentInChildren<BoardElementPosition>();
        var data = element.GetComponentInChildren<BoardElementData>();

        GameObject step = Instantiate(SnakeStep, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var stepPosition = step.GetComponent<SnakeStepPosition>();
        stepPosition.RowPosition = position.RowPosition;
        stepPosition.ColumnPosition = position.ColumnPosition;

        var stepData = step.GetComponent<SnakeStepData>();
        stepData.Number = AppConstants.StartStepsCount;
        stepData.StartNumber = AppConstants.StartStepsCount;

        data.ContainsSnakeStep = true;
    }

    private void GenerateWall(int i, int j, DirectionEnum direction)
    {
        GameObject topWall = Instantiate(Wall, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var wallPosition = topWall.GetComponent<WallPosition>();
        wallPosition.RowPosition = i;
        wallPosition.ColumnPosition = j;

        var wallData = topWall.GetComponent<WallData>();
        wallData.Direction = direction;
    }
}
