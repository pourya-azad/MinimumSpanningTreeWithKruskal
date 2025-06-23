using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.ViewModel
{
    public class GraphViewModel
    {
        public required List<Node> Nodes { get; set; }
        public required List<Edge> Edges { get; set; }
        public required List<Edge> MSTEdges { get; set; }
    }

}
