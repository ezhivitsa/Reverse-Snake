using Assets.ReverseSnake.Scripts.Enums;

sealed class UpdateTargetEvent
{
    public int Round;

    public int? Column;

    public int? Row;

    public bool Silent;

    public TargetValueEnum? Value;
}
