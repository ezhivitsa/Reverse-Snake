using Assets.src.Enums;
using Assets.src.Models;

namespace Assets.src.Helpers
{
    static class PositionHelper
    {
        public static PositionModel GetNextPosition(
            int columnCount,
            int rowsCount,
            int column,
            int row,
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
                result.Column = columnCount - 1;
            }
            else if (result.Column >= columnCount)
            {
                result.Column = 0;
            }
            else if (result.Row < 0)
            {
                result.Row = rowsCount - 1;
            }
            else if (result.Row >= rowsCount)
            {
                result.Row = 0;
            }

            return result;
        }
    }
}
