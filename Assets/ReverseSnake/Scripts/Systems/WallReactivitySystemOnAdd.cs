using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.WallAlgorithm;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class WallReactivitySystemOnAdd : EcsReactiveSystem<Wall>, IEcsInitSystem
    {
        ReverseSnakeWorld _world;
        EcsFilter<Wall> _wallsFilter;

        private StateManager _stateManager;
        private Graph _graph;
        private GameObject[] _wallGameObjects;

        public void Initialize()
        {
            _stateManager = StateManager.GetInstance(_world);
            _graph = GraphGenertor.Generate();

            _wallGameObjects = GameObject.FindGameObjectsWithTag(AppConstants.WallTag);
            foreach (var wallObj in _wallGameObjects)
            {
                wallObj.SetActive(false);
            }
        }

        public void Destroy()
        {
            GraphGenertor.Clear();
        }

        protected override EcsReactiveType GetReactiveType()
        {
            return EcsReactiveType.OnAdded;
        }

        protected override void RunReactive()
        {
            for (var i = 0; i < ReactedEntitiesCount; i++)
            {
                var entity = ReactedEntities[i];
                var wall = _world.GetComponent<Wall>(entity);

                CreateWall(wall);

                if (!wall.Silent)
                {
                    _stateManager.AddWalls(new List<Wall> { wall });
                }
            }
        }

        private void CreateWall(Wall wallEntity)
        {
            Wall randomWall;
            if (wallEntity.Row >= 0 && wallEntity.Column >= 0)
            {
                randomWall = wallEntity;
            }
            else
            {
                var notActiveWalls = GetNotActiveWalls();                

                var availableWalls = notActiveWalls.Where((wall) => {
                    var wallsOnPosition = GetWallsOnPosition(wall.Row, wall.Column);

                    var nextPos = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);
                    var wallsOnNextPosition = GetWallsOnPosition(nextPos.Row, nextPos.Column);

                    return wallsOnPosition.Count < 2 && wallsOnNextPosition.Count < 2 &&
                        DFS.IsConnectedWithoutEdge(_graph, wall.Row, wall.Column, nextPos.Row, nextPos.Column);
                }).ToList();

                randomWall = availableWalls.RandomElement();
            }
            if (randomWall == null)
            {
                return;
            }
            
            CloseWall(wallEntity, randomWall);

            var nextPosition = PositionHelper.GetNextPosition(randomWall.Row, randomWall.Column, randomWall.Direction);
            _graph.RemoveEdge(randomWall.Row, randomWall.Column, nextPosition.Row, nextPosition.Column);
        }

        private List<Wall> GetNotActiveWalls()
        {
            var result = new List<Wall>();
            for (var i = 0; i < AppConstants.Rows; i += 1)
            {
                for (var j = 0; j < AppConstants.Columns; j += 1)
                {
                    result.Add(new Wall
                    {
                        Row = i,
                        Column = j,
                        Direction = DirectionEnum.Top
                    });
                    result.Add(new Wall
                    {
                        Row = i,
                        Column = j,
                        Direction = DirectionEnum.Right
                    });
                }
            }

            for (var i = 0; i < _wallsFilter.EntitiesCount; i++)
            {
                var wall = _wallsFilter.Components1[i];
                var component = result
                    .FirstOrDefault(w => w.Row == wall.Row && w.Column == wall.Column && w.Direction == wall.Direction);

                if (component != null)
                {
                    result.Remove(component);
                }
            }
            
            return result;
        }

        private List<Wall> GetWallsOnPosition(int row, int column)
        {
            var activeWalls = _wallsFilter.ToEntitiesList();
            var wallsOnPosition = new List<Wall>();

            var topWall = activeWalls
                .FirstOrDefault(w => w.Row == row && w.Column == column && w.Direction == DirectionEnum.Top);
            if (topWall != null)
            {
                wallsOnPosition.Add(topWall);
            }

            var rightWall = activeWalls
                .FirstOrDefault(w => w.Row == row && w.Column == column && w.Direction == DirectionEnum.Right);
            if (rightWall != null)
            {
                wallsOnPosition.Add(rightWall);
            }

            var bottomPosition = PositionHelper.GetNextPosition(row, column, DirectionEnum.Bottom);
            var bottomWall = activeWalls
                .FirstOrDefault(w =>
                    w.Row == bottomPosition.Row &&
                    w.Column == bottomPosition.Column &&
                    w.Direction == DirectionEnum.Bottom
                );
            if (bottomWall != null)
            {
                wallsOnPosition.Add(bottomWall);
            }

            var leftPosition = PositionHelper.GetNextPosition(row, column, DirectionEnum.Left);
            var leftWall = activeWalls
                .FirstOrDefault(w =>
                    w.Row == bottomPosition.Row &&
                    w.Column == bottomPosition.Column &&
                    w.Direction == DirectionEnum.Left
                );
            if (leftWall != null)
            {
                wallsOnPosition.Add(leftWall);
            }

            return wallsOnPosition;
        }

        private void CloseWall(Wall wallEntity, Wall wall)
        {
            wallEntity.Row = wall.Row;
            wallEntity.Column = wall.Column;
            wallEntity.Direction = wall.Direction;

            var nextPosition = PositionHelper.GetNextPosition(wall.Row, wall.Column, wall.Direction);
            var reverseDirection = DirectionHelper.GetReverseDirection(wall.Direction);

            GameObject wallObject = null;
            GameObject reverseWallObject = null;

            foreach (var wallObj in _wallGameObjects)
            {
                var wallElement = wallObj.transform.GetComponent<WallElement>();
                if (
                    wallElement.Row == wall.Row &&
                    wallElement.Column == wall.Column &&
                    wallElement.Direction == wall.Direction
                )
                {
                    wallObject = wallObj;
                }

                if (
                    wallElement.Row == nextPosition.Row &&
                    wallElement.Column == nextPosition.Column &&
                    wallElement.Direction == reverseDirection
                )
                {
                    reverseWallObject = wallObj;
                }
            }

            wallEntity.Transforms = new List<Transform>();
            if (wallObject != null)
            {
                wallEntity.Transforms.Add(wallObject.transform);
                wallObject.SetActive(true);
            }
            if (reverseWallObject != null)
            {
                wallEntity.Transforms.Add(reverseWallObject.transform);
                reverseWallObject.SetActive(true);
            }
        }
    }
}
