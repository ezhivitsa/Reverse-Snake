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
        GenerateSnakeStep(
            SnakeStep,
            position.ColumnPosition,
            position.RowPosition,
            AppConstants.StartStepsCount,
            AppConstants.StartStepsCount
        );
    }

    private void GenerateWall(int i, int j, DirectionEnum direction)
    {
        GameObject topWall = Instantiate(Wall, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var wallPosition = topWall.GetComponent<WallPosition>();

        wallPosition.RowPosition = i;
        wallPosition.ColumnPosition = j;
        wallPosition.Direction = direction;
    }
}
