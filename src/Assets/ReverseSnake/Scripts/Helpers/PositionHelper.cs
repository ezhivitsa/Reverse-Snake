using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Models;
using Assets.src;

namespace Assets.ReverseSnake.Scripts.Helpers
{
    static class PositionHelper
    {
        public static PositionModel GetNextPosition(
            int row,
            int column,
            DirectionEnum direction
        )
        {
            var result = new PositionModel()
            {
                Column = column,
                Row = row,
            };

            switch (direction)
            {
                case DirectionEnum.Top:
                    result.Row -= 1;
                    break;

                case DirectionEnum.Right:
                    result.Column -= 1;
                    break;

                case DirectionEnum.Bottom:
                    result.Row += 1;
                    break;

                case DirectionEnum.Left:
                    result.Column += 1;
                    break;
            }

            if (result.Column < 0)
            {
                result.Column = AppConstants.Columns - 1;
            }
            else if (result.Column >= AppConstants.Columns)
            {
                result.Column = 0;
            }
            else if (result.Row < 0)
            {
                result.Row = AppConstants.Rows - 1;
            }
            else if (result.Row >= AppConstants.Rows)
            {
                result.Row = 0;
            }

            return result;
        }
    }
}
