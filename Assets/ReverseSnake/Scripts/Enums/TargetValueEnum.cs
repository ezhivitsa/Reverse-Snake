using Assets.ReverseSnake.Scripts.Attributes;
using System.ComponentModel;

namespace Assets.ReverseSnake.Scripts.Enums
{
    public enum TargetValueEnum
    {
        [Description("1")]
        [Texture("addWallTarget")]
        [Probabilities(90, 80, 75)]
        AddWall = 0,

        [Description("-1")]
        [Texture("removeWallTarget")]
        [Probabilities(5, 10, 13)]
        RemoveWall = 1,

        [Description("1(g), -2(r)")]
        [Probabilities(2f, 4, 6)]
        AddTailRemoveTwoWall = 2,

        [Description("-1(g), 1(r)")]
        [Probabilities(3f, 6, 8)]
        RemoveTailAddWall = 3,
    }
}
