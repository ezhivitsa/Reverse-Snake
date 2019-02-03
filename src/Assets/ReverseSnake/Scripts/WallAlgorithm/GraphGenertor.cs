using Assets.src;

namespace Assets.ReverseSnake.Scripts.WallAlgorithm
{
    public static class GraphGenertor
    {
        private static Graph _graph;

        public static Graph Generate()
        {
            if (_graph != null)
            {
                return _graph;
            }

            _graph = new Graph();

            GenerateVertices(_graph);
            GenerateEdges(_graph);

            return _graph;
        }

        public static void Clear()
        {
            _graph = null;
        }

        private static void GenerateVertices(Graph graph)
        {
            for (var i = 0; i < AppConstants.Rows; i += 1)
            {
                for (var j = 0; j < AppConstants.Columns; j += 1)
                {
                    graph.AddNode(i, j);
                }
            }
        }

        private static void GenerateEdges(Graph graph)
        {
            for (var i = 0; i < AppConstants.Rows; i += 1)
            {
                for (var j = 0; j < AppConstants.Columns; j += 1)
                {
                    graph.AddEdge(i, j, (i - 1 + AppConstants.Rows) % AppConstants.Rows, j);
                    graph.AddEdge(i, j, i, (j - 1 + AppConstants.Columns) % AppConstants.Columns);
                }
            }
        }
    }
}
