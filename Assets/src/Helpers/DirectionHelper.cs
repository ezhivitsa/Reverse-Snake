using Assets.src.Enums;

namespace Assets.src.Helpers
{
    static public class DirectionHelper
    {
        static public DirectionEnum GetReverseDirection(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Top:
                    return DirectionEnum.Bottom;

                case DirectionEnum.Right:
                    return DirectionEnum.Left;

                case DirectionEnum.Bottom:
                    return DirectionEnum.Top;

                case DirectionEnum.Left:
                    return DirectionEnum.Right;

                default:
                    return DirectionEnum.None;
            }
        }
    }
}
