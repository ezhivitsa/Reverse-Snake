using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.WallAlgorithm
{
    public class Graph
    {
        public List<Node> Nodes { get; private set; }

        public Graph()
        {
            Nodes = new List<Node>();
        }

        public void AddNode(int row, int column)
        {
            var node = new Node
            {
                Row = row,
                Column = column,
            };

            Nodes.Add(node);
        }

        public void AddEdge(int rowStart, int columnStart, int rowEnd, int columnEnd)
        {
            var startNode = GetNode(rowStart, columnStart);
            var endNode = GetNode(rowEnd, columnEnd);

            startNode.AdjacentNodes.Add(endNode);
            endNode.AdjacentNodes.Add(startNode);
        }

        public void RemoveEdge(int rowStart, int columnStart, int rowEnd, int columnEnd)
        {
            var startNode = GetNode(rowStart, columnStart);
            var endNode = GetNode(rowEnd, columnEnd);

            startNode.AdjacentNodes.Remove(endNode);
            endNode.AdjacentNodes.Remove(startNode);
        }

        private Node GetNode(int row, int column)
        {
            return Nodes.Find(n => n.Row == row && n.Column == column);
        }
    }

    public class Node
    {
        public int Column { get; set; }

        public int Row { get; set; }

        public List<Node> AdjacentNodes { get; set; }

        public Node()
        {
            AdjacentNodes = new List<Node>();
        }
    }
}
