using Assets.ReverseSnake.Scripts.Attributes;
using System.ComponentModel;

namespace Assets.ReverseSnake.Scripts.Enums
{
    public enum TargetValueEnum
    {
        [Description("1(g)")]
        [Probabilities(90, 80)]
        AddWall = 0,

        [Description("-1(r)")]
        [Probabilities(5, 10)]
        RemoveWall = 1,

        [Description("1(g), -2(r)")]
        [Probabilities(2f, 4)]
        AddTailRemoveTwoWall = 2,

        [Description("-1(g), 1(r)")]
        [Probabilities(3f, 6)]
        RemoveTailAddWall = 3,
    }
}
