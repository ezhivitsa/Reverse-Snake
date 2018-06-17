using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    public Mesh SnakeStepMesh;
    public Material SnakeStepMaterial;

    public Mesh BoardElementMesh;
    public Material BoardElementMaterial;

    public Mesh BoardElementWallMesh;
    public Material BoardElementWallMaterial;

    public int BoardWidth;
    public int BoardHeigth;

    public int BoardElementHeight;
    public int BoardElementWidth;
    public int BoardElementWallHeight;
    public int BoardElementWallWidth;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private void Start()
    {
        //CreateBoardElements();
        //CreateSnakeStep();
    }

    private void Update()
    {
    }

    private void CreateBoardElements()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var boardElementArchetype = entityManager.CreateArchetype(
            typeof(TransformMatrix),
            typeof(Position),
            typeof(MeshInstanceRenderer)
        );

        var boardElementWallArchetype = entityManager.CreateArchetype(
            typeof(TransformMatrix),
            typeof(Position),
            typeof(MeshInstanceRenderer)
        );

        var fullElementHeight = BoardElementHeight + BoardElementWallHeight;
        var fullElementWidth = BoardElementWidth + BoardElementWallWidth;

        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeigth; y++)
            {
                var element = entityManager.CreateEntity(boardElementArchetype);
                entityManager.SetSharedComponentData(element, new MeshInstanceRenderer
                {
                    mesh = BoardElementMesh,
                    material = BoardElementMaterial,
                });

                var elementPosition = new float3(x * fullElementHeight, -0.4f, y * fullElementWidth);
                entityManager.SetComponentData(element, new Position
                {
                    Value = elementPosition
                });

                // top wall
                CreateWall(
                    entityManager,
                    boardElementWallArchetype,
                    elementPosition - new float3(- BoardElementHeight, 0, 0)
                );
                // right wall
                CreateWall(
                    entityManager,
                    boardElementWallArchetype,
                    elementPosition - new float3(0, 0, BoardElementWallWidth)
                );
                // bottom wall
                CreateWall(
                    entityManager,
                    boardElementWallArchetype,
                    elementPosition - new float3(BoardElementHeight, 0, 0)
                );
                // left wall
                CreateWall(
                    entityManager,
                    boardElementWallArchetype,
                    elementPosition - new float3(0, 0, -BoardElementWallWidth)
                );
            }
        }
    }

    private void CreateSnakeStep()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var snakeStepArchetype = entityManager.CreateArchetype(
            typeof(TransformMatrix),
            typeof(Position),
            typeof(MeshInstanceRenderer),
            typeof(TextMesh),
            typeof(SnakeStepInput)
        );

        //var snakeStep = GetComponent<SnakeStepEntity>();
        //Debug.Log(snakeStep);
        //entityManager.AddComponent(snakeStep, typeof(Rigidbody));
        //var snakeStep = entityManager.CreateEntity(snakeStepArchetype);

        //entityManager.SetSharedComponentData(snakeStep, new MeshInstanceRenderer
        //{
        //    mesh = SnakeStepMesh,
        //    material = SnakeStepMaterial,
        //});

        //entityManager.SetComponentData(snakeStep, new Position { Value = new float3(0, 0, 0) });
        //entityManager.SetComponentData(snakeStep, new SnakeStepInput
        //{
        //    PositionNumber = 1,
        //    Value = 3,
        //});
    }

    private void CreateWall(EntityManager entityManager, EntityArchetype archetype, float3 positionValue)
    {
        var wall = entityManager.CreateEntity(archetype);

        entityManager.SetSharedComponentData(wall, new MeshInstanceRenderer
        {
            mesh = BoardElementWallMesh,
            material = BoardElementWallMaterial,
        });

        entityManager.SetComponentData(wall, new Position
        {
            Value = positionValue,
        });
    }
}
