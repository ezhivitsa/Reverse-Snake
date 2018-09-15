using Assets.ReverseSnake.Scripts.Enums;
using System;

[Serializable]
sealed class Wall
{
    public int Row;

    public int Column;

    public DirectionEnum Direction;

    public bool IsActive;
}