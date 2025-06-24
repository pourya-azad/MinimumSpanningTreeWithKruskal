namespace MinimumSpanningTreeWithKruskal.Models
{
    public class GraphData
    {
        public List<NodeInput> Nodes { get; set; } = new();
        public List<EdgeInput> Edges { get; set; } = new();

        public string GraphName { get; set; }
    }
}
