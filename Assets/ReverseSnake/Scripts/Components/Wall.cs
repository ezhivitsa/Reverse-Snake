using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System;
using UnityEngine;

[Serializable]
sealed class Wall
{
    public int Row;

    public int Column;

    public DirectionEnum Direction;

    public bool IsActive;

    [NonSerialized]
    [EcsIgnoreNullCheck]
    public Transform Transform;
}