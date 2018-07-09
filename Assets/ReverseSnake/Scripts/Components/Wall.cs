using Assets.ReverseSnake.Scripts.Enums;

sealed class Wall
{
    public int Row;

    public int Column;

    public DirectionEnum Direction;

    public bool IsActive;
}