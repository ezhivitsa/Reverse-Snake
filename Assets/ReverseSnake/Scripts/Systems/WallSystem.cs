using Assets.ReverseSnake.Scripts.Enums;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.UnityIntegration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Managers;

[EcsInject]
public class WallSystem : IEcsInitSystem, IEcsRunSystem
{
    const string WallPath = "Objects/Wall";

    const string DisabledWallMaterial = "Materials/wall-base-material";
    const string ActiveWallMaterial = "Materials/wall-close-material";

    EcsWorld _world = null;

    EcsFilter<Wall> _wallFilter = null;

    private StateManager _stateManager;

    EcsFilter<AddWallEvent> _addWallEventFilter = null;
    EcsFilter<ClearWallEvent> _clearEventFilter = null;
    EcsFilter<ShowWallEvent> _showEventFilter = null;
    EcsFilter<RemoveWallEvent> _removeWallEventFilter = null;
    EcsFilter<CreateWallsEvent> _createWallsEvents = null;

    public void Initialize()
    {
        _stateManager = StateManager.GetInstance(_world);

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

    public void Run()
    {
        HandleAddWallEvent();
        HandleClearWallEvent();
        HandleShowWallEvent();
        HandleRemoveWallEvent();
        HandleCreateWallsEvent();
    }

    public void Destroy() { }

    private void HandleAddWallEvent()
    {
        _addWallEventFilter.HandleEvents(_world, (eventData) => {
            AddWall();
        });
    }

    private void HandleClearWallEvent()
    {
        _clearEventFilter.HandleEvents(_world, (eventData) => {
            for (var i = 0; i < _wallFilter.EntitiesCount; i += 1)
            {
                var wall = _wallFilter.Components1[i];
                if (wall.IsActive)
                {
                    wall.IsActive = false;
                    UpdatePrefab(wall);
                }
            }
        });
    }

    private void HandleShowWallEvent()
    {
        _showEventFilter.HandleEvents(_world, (eventData) => {
            for (var i = 0; i < _wallFilter.EntitiesCount; i += 1)
            {
                var entity = _wallFilter.Components1[i];
                entity.Transform.gameObject.SetActive(eventData.IsActive);
            }
        });
    }

    private void HandleRemoveWallEvent()
    {
        _removeWallEventFilter.HandleEvents(_world, (eventData) =>
        {
            var entities = _wallFilter
                .ToEntitiesList()
                .Where(e => e.IsActive)
                .ToList();

            var wallsToRemove = new List<Wall>();

            for (var i = 0; i < eventData.Walls; i += 1)
            {
                var wall = entities.RandomElement();
                wall.IsActive = false;
                UpdatePrefab(wall);

                var nextPosition = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);

                var reverseDirection = DirectionHelper.GetReverseDirection(wall.Direction);
                var reverseWall = GetWall(entities, nextPosition.Row, nextPosition.Column, reverseDirection);
                reverseWall.IsActive = false;
                UpdatePrefab(reverseWall);

                entities = entities.Where(e => e != wall && e != reverseWall).ToList();

                wallsToRemove.Add(wall);
                wallsToRemove.Add(reverseWall);
            }

            _stateManager.RemoveWalls(wallsToRemove);
        });
    }

    private void HandleCreateWallsEvent()
    {
        _createWallsEvents.HandleEvents(_world, (eventData) => {
            foreach (var wallData in eventData.Walls)
            {
                var wall = GetWall(wallData.Column, wallData.Row, wallData.Direction);
                wall.IsActive = true;

                UpdatePrefab(wall);
            }
        });
    }

    private void GenerateWall(int row, int column, DirectionEnum direction)
    {
        Wall element = _world.CreateEntityWith<Wall>();
        element.Row = row;
        element.Column = column;
        element.Direction = direction;
        element.IsActive = false;

        var wallObject = (GameObject)Resources.Load(WallPath, typeof(GameObject));
        element.Transform = GameObject.Instantiate(wallObject).transform;
        element.Transform.position = GetPositionVector(false, row, column, direction);
        element.Transform.rotation = GetRotationQuaternion(direction);
        element.Transform.gameObject.SetActive(true);
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
        var notActiveWalls = _wallFilter
            .ToEntitiesList()
            .Where(e => !e.IsActive).ToList();

        var availableWalls = notActiveWalls.Where((wall) => {
            var wallsOnPosition = GetWallsOnPosition(notActiveWalls, wall.Row, wall.Column);

            var nextPos = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);
            var wallsOnNextPosition = GetWallsOnPosition(notActiveWalls, nextPos.Row, nextPos.Column);

            return wallsOnPosition.Count > 2 && wallsOnNextPosition.Count > 2;
        }).ToList();

        var randomWall = availableWalls.RandomElement();
        CloseWall(randomWall, availableWalls);
    }

    private List<Wall> GetWallsOnPosition(List<Wall> walls, int row, int column)
    {
        return walls
            .Where(wall => wall.Column == column && wall.Row == row)
            .ToList();
    }

    private void CloseWall(Wall wall, List<Wall> availableWalls)
    {
        wall.IsActive = true;
        UpdatePrefab(wall);

        var nextPosition = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);

        var reverseDirection = DirectionHelper.GetReverseDirection(wall.Direction);
        var reverseWall = GetWall(availableWalls, nextPosition.Row, nextPosition.Column, reverseDirection);

        reverseWall.IsActive = true;
        UpdatePrefab(reverseWall);

        _stateManager.AddWalls(new List<Wall> { wall, reverseWall });
    }

    private void UpdatePrefab(Wall wall)
    {
        wall.Transform.position = GetPositionVector(
            wall.IsActive,
            wall.Row,
            wall.Column,
            wall.Direction
        );
        wall.Transform.gameObject.GetComponent<MeshRenderer>().material = GetMaterial(wall.IsActive);
    }

    private Wall GetWall(List<Wall> walls, int row, int column, DirectionEnum direction)
    {
        return walls.Find(w => w.Column == column && w.Row == row && w.Direction == direction);
    }

    private int GetEntity(Wall wall)
    {
        for (var i = 0; i < _wallFilter.EntitiesCount; i++)
        {
            if (_wallFilter.Components1[i] == wall)
            {
                return _wallFilter.Entities[i];
            }
        }

        return 0;
    }

    private Wall GetWall(int column, int row, DirectionEnum direction)
    {
        for (var i = 0; i < _wallFilter.EntitiesCount; i++)
        {
            var wall = _wallFilter.Components1[i];
            if (wall.Column == column && wall.Row == row && wall.Direction == direction)
            {
                return wall;
            }
        }

        return null;
    }

    private Material GetMaterial(bool isActive)
    {
        var name = isActive ? ActiveWallMaterial : DisabledWallMaterial;
        return (Material)Resources.Load(name, typeof(Material));
    }
}