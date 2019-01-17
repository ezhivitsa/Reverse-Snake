using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
sealed class Wall
{
    public int Row;

    public int Column;

    public DirectionEnum Direction;

    [NonSerialized]
    [EcsIgnoreNullCheck]
    public List<Transform> Transforms;

    public bool Silent = false;
}