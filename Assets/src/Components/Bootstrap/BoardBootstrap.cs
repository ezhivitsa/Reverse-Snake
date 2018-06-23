using Assets.src.Enums;
using UnityEngine;

public class BoardBootstrap : MonoBehaviour {

    public int Lines;

    public int Columns;

    public GameObject Element;
    public GameObject Wall;

	// Use this for initialization
	void Start () {
        GenerateBoard();
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

    private void GenerateWall(int i, int j, DirectionEnum direction)
    {
        GameObject topWall = Instantiate(Wall, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;
        var wallPosition = topWall.GetComponent<WallPosition>();
        wallPosition.RowPosition = i;
        wallPosition.ColumnPosition = j;

        var wallData = topWall.GetComponent<WallData>();
        wallData.Directon = direction;
    }
}
