using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.WallAlgorithm
{
    public static class DFS
    {
        public static bool IsConnectedWithoutEdge(
            Graph graph,
            int startRow,
            int startColumn,
            int endRow,
            int endColumn
        )
        {
            var firstNode = graph.Nodes[0];
            var stack = new Stack<Node>();
            stack.Push(firstNode);

            var labeledNodes = new HashSet<Node>();

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!labeledNodes.Contains(node))
                {
                    labeledNodes.Add(node);
                    foreach (var adjacentNode in node.AdjacentNodes)
                    {
                        if (
                            !(
                                node.Row == startRow && node.Column == startColumn &&
                                adjacentNode.Row == endRow && adjacentNode.Column == endColumn
                            ) &&
                            !(
                                node.Row == endRow && node.Column == endColumn &&
                                adjacentNode.Row == startRow && adjacentNode.Column == startColumn
                            )
                        )
                        {
                            stack.Push(adjacentNode);
                        }
                    }
                }
            }

            return labeledNodes.Count == graph.Nodes.Count;
        }
    }
}
