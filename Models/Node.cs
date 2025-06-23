namespace MinimumSpanningTreeWithKruskal.Models
{
    public class Node
    {
        public int Id { get; set; } // حذف required

        public required string Label { get; set; } // اگه مطمئنی همیشه داده داره، بذار بمونه

        public virtual ICollection<Edge> OutgoingEdges { get; set; } = new List<Edge>();
        public virtual ICollection<Edge> IncomingEdges { get; set; } = new List<Edge>();
    }

}
