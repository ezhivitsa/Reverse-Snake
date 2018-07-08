using Assets.ReverseSnake.Scripts.Enums;
using Assets.src;
using LeopotamGroup.Ecs;
using LeopotamGroup.Ecs.UnityIntegration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Extensions;

[EcsInject]
public class WallSystem : IEcsInitSystem, IEcsRunSystem
{
    const string WallPath = "Objects/Wall";

    const string DisabledWallMaterial = "Materials/wall-base-material";
    const string ActiveWallMaterial = "Materials/wall-close-material";

    EcsWorld _world = null;

    EcsFilter<AddWallEvent> _addWallEventFilter = null;
    EcsFilter<Wall> _wallFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        for (var i = 0; i < AppConstants.Rows; i++)
        {
            for (var j = 0; j < AppConstants.Columns; j++)
            {
                GenerateWall(i, j, DirectionEnum.Top);
                GenerateWall(i, j, DirectionEnum.Bottom);
                GenerateWall(i, j, DirectionEnum.Left);
                GenerateWall(i, j, DirectionEnum.Right);
            }
        }
    }

    void IEcsRunSystem.OnUpdate()
    {
        for (var i = 0; i < _addWallEventFilter.EntitiesCount; i++)
        {
            AddWall();
            _world.RemoveEntity(_addWallEventFilter.Entities[i]);
        }
    }

    void IEcsInitSystem.OnDestroy() { }

    private void GenerateWall(int row, int column, DirectionEnum direction)
    {
        var entity = _world.CreateEntity();
        var element = _world.AddComponent<Wall>(entity);
        element.Row = row;
        element.Column = column;
        element.Direction = direction;
        element.IsActive = false;

        var prefab = _world.AddComponent<UnityPrefabComponent>(entity);
        prefab.Attach(WallPath);
        prefab.Prefab.transform.position = GetPositionVector(false, row, column, direction);
        prefab.Prefab.transform.rotation = GetRotationQuaternion(direction);
        prefab.Prefab.SetActive(true);
    }

    private Vector3 GetPositionVector(bool isActive, int row, int column, DirectionEnum direction)
    {
        var yPos = isActive ? 0.01F : -0.95F;
        Vector3 result;

        switch (direction)
        {
            case DirectionEnum.Top:
                result = CalculatePosition(column, row, -1 / 2F, -1, yPos, 0, 1 / 2F);
                break;

            case DirectionEnum.Bottom:
                result = CalculatePosition(column, row, -1 / 2F, -1, yPos, 1, 3 / 2F);
                break;

            case DirectionEnum.Left:
                result = CalculatePosition(column, row, 0, -1 / 2F, yPos, 1 / 2F, 1);
                break;

            case DirectionEnum.Right:
                result = CalculatePosition(column, row, -1, -3 / 2F, yPos, 1 / 2F, 1);
                break;

            default:
                return new Vector3(0, 0, 0);
        }

        return result - new Vector3(AppConstants.OffsetX, 0, AppConstants.OffsetZ);
    }

    private Vector3 CalculatePosition(
        float columnPos,
        float rowPos,
        float elementXCoeff,
        float borderXCoeff,
        float yPos,
        float elementZCoeff,
        float borderZCoeff
    )
    {
        return new Vector3(
            AppConstants.BoardElementWidth * (columnPos + elementXCoeff + 1) + AppConstants.BorderWidth * (columnPos + borderXCoeff + 2),
            yPos,
            AppConstants.BoardElementWidth * (rowPos + elementZCoeff) + AppConstants.BorderWidth * (rowPos + borderZCoeff)
        );
    }

    private Quaternion GetRotationQuaternion(DirectionEnum direction)
    {
        switch (direction)
        {
            case DirectionEnum.Left:
            case DirectionEnum.Right:
                return Quaternion.Euler(0, 90, 0);

            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }

    private void AddWall()
    {
        List<int> allWalls = new List<int>();
        for (var i = 0; i < _wallFilter.EntitiesCount; i++)
        {
            allWalls.Add(i);
        }

        var notActiveWalls = allWalls.Where(e => !_wallFilter.Components1[e].IsActive).ToList();

        var availableWalls = notActiveWalls.Where((e) => {
            var wall = _wallFilter.Components1[e];
            var wallsOnPosition = GetWallsOnPosition(notActiveWalls, wall.Row, wall.Column);

            var nextPos = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);
            var wallsOnNextPosition = GetWallsOnPosition(notActiveWalls, nextPos.Row, nextPos.Column);

            return wallsOnPosition.Count > 2 && wallsOnNextPosition.Count > 2;
        }).ToList();

        var randomWall = availableWalls.RandomElement();
        CloseWall(randomWall, availableWalls);
    }

    private List<int> GetWallsOnPosition(List<int> walls, int row, int column)
    {
        return walls
            .Where((e) =>
            {
                var wall = _wallFilter.Components1[e];
                return wall.Column == column && wall.Row == row;
            })
            .ToList();
    }

    private void CloseWall(int entity, List<int> availableWalls)
    {
        var entityNum = _wallFilter.Entities[entity];
        var wall = _wallFilter.Components1[entity];
        wall.IsActive = true;

        var prefab = _world.GetComponent<UnityPrefabComponent>(entityNum);
        prefab.Prefab.transform.position = GetPositionVector(wall.IsActive, wall.Row, wall.Column, wall.Direction);
        prefab.Prefab.gameObject.GetComponent<MeshRenderer>().material = GetMaterial(wall.IsActive);

        var nextPosition = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);

        var reverseDirection = DirectionHelper.GetReverseDirection(wall.Direction);
        var reverseEntity = GetWall(availableWalls, nextPosition.Row, nextPosition.Column, reverseDirection);

        var reverseEntityNum = _wallFilter.Entities[reverseEntity];
        var reverseWall = _wallFilter.Components1[reverseEntity];
        reverseWall.IsActive = true;

        var rPrefab = _world.GetComponent<UnityPrefabComponent>(reverseEntityNum);
        rPrefab.Prefab.transform.position = GetPositionVector(
            reverseWall.IsActive,
            reverseWall.Row,
            reverseWall.Column,
            reverseWall.Direction
        );
        rPrefab.Prefab.gameObject.GetComponent<MeshRenderer>().material = GetMaterial(reverseWall.IsActive);
    }

    private int GetWall(List<int> walls, int row, int column, DirectionEnum direction)
    {
        return walls.Find(e => {
            var w = _wallFilter.Components1[e];
            return w.Column == column && w.Row == row && w.Direction == direction;
        });
    }

    private Material GetMaterial(bool isActive)
    {
        var name = isActive ? ActiveWallMaterial : DisabledWallMaterial;
        return (Material)Resources.Load(name, typeof(Material));
    }
}