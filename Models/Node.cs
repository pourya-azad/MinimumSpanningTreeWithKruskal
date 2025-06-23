namespace MinimumSpanningTreeWithKruskal.Models
{
    public class Node
    {
        public int Id { get; set; }
        public required string Label { get; set; }

        // Optional: برای رابطه دوطرفه با Edge
        public virtual ICollection<Edge> EdgesAsNode1 { get; set; } = new List<Edge>();
        public virtual ICollection<Edge> EdgesAsNode2 { get; set; } = new List<Edge>();
    }


}
