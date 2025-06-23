namespace MinimumSpanningTreeWithKruskal.Models
{
    public class MSTEdge
    {
        public int Id { get; set; }
        public int EdgeId { get; set; }
        public virtual Edge? Edge { get; set; }
    }

}
