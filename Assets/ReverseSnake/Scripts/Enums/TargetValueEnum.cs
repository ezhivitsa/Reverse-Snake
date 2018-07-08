using System.ComponentModel;

namespace Assets.ReverseSnake.Scripts.Enums
{
    public enum TargetValueEnum
    {
        [Description("-1")]
        AddWall = 0,

        RemoveWall = 1,

        AddTailRemoveTwoWall = 2,

        RemoveTailAddWall = 3,
    }
}
