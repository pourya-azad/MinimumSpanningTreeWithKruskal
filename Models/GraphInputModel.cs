namespace MinimumSpanningTreeWithKruskal.Models
{
    public class GraphInputModel
    {
        public string JsonData { get; set; } = string.Empty;
        public List<string> Nodes { get; set; } = new();
        public List<EdgeInput> Edges { get; set; } = new();
    }
    public class EdgeInput
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int Weight { get; set; }
    }

    public class NodeInput
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    // در اکشنِ POST: var model = JsonConvert.DeserializeObject<GraphInputModel>(model.JsonData);
}
