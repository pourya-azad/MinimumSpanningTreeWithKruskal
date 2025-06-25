using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.ViewModels
{
    public class GraphViewModel
    {
        public required List<Node> Nodes { get; set; }
        public required List<Edge> Edges { get; set; }
        public required List<Edge> MSTEdges { get; set; }
        public bool ShowMST { get; set; }
        public bool HasMST => MSTEdges != null && MSTEdges.Count > 0;
    }

}
