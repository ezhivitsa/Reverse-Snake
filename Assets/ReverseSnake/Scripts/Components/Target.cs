using Assets.ReverseSnake.Scripts.Enums;
using System;

[Serializable]
sealed class Target
{
    public int Row;

    public int Column;

    public TargetValueEnum Value;

    public int Round;
}