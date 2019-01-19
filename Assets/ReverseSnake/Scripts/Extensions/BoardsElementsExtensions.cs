using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    static class BoardsElementsExtensions
    {
        public static void ClearElements(this IEnumerable<BoardElement> enumerableList, int? round = null)
        {
            foreach (var element in enumerableList)
            {
                if (!round.HasValue || element.Round == round.Value)
                {
                    element.ContainsSnakeStep = false;
                    element.ContainsTarget = false;
                    element.Round = -1;
                }
            }
        }
    }
}
