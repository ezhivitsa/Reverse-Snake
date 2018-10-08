using Assets.ReverseSnake.Scripts.Enums;
using Assets.src;
using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Managers;
using System;

[EcsInject]
public class WallSystem : IEcsInitSystem, IEcsRunSystem
{
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

        var lines = GameObject.FindGameObjectsWithTag(AppConstants.BoardLineTag);
        foreach (var line in lines)
        {
            foreach (Transform plane in line.transform)
            {
                var wall = plane.GetComponent<WallElement>();
                GenerateWall(wall.Row, wall.Column, wall.Direction, plane);
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

                entities = entities.Where(e => e != wall).ToList();

                wallsToRemove.Add(wall);
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

    private void GenerateWall(int row, int column, DirectionEnum direction, Transform transform)
    {
        Wall element = _world.CreateEntityWith<Wall>();
        element.Row = row;
        element.Column = column;
        element.Direction = direction;
        element.IsActive = false;
        element.Transform = transform;
        element.Transform.gameObject.SetActive(true);
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
        var wallsOnPosition = new List<Wall>();
        foreach (var direction in Enum.GetValues(typeof(DirectionEnum)).Cast<DirectionEnum>())
        {
            var wall = walls.Find(w => w.Column == column && w.Row == row && w.Direction == direction);
            if (wall != null)
            {
                wallsOnPosition.Add(wall);
            }
            else
            {
                var nextPosition = PositionHelper.GetNextPosition(row, column, direction);
                var reverseDirection = DirectionHelper.GetReverseDirection(direction);

                wall = walls.Find(w =>
                    w.Column == nextPosition.Column &&
                    w.Row == nextPosition.Row &&
                    w.Direction == reverseDirection
                );

                if (wall != null)
                {
                    wallsOnPosition.Add(wall);
                }
            }
        }

        return wallsOnPosition;
    }

    private void CloseWall(Wall wall, List<Wall> availableWalls)
    {
        wall.IsActive = true;
        UpdatePrefab(wall);

        _stateManager.AddWalls(new List<Wall> { wall });
    }

    private void UpdatePrefab(Wall wall)
    {
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