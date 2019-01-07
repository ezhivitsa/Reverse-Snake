using Assets.ReverseSnake.Scripts.Enums;
using Leopotam.Ecs;
using System;
using UnityEngine;

[Serializable]
sealed class Target
{
    public int Row;

    public int Column;

    public TargetValueEnum Value;

    public int Round;

    [NonSerialized]
    [EcsIgnoreNullCheck]
    public Transform Transform;

    public bool Silent = false;
}