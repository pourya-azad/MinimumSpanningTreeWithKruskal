using System.ComponentModel.DataAnnotations.Schema;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class Edge
    {
        public int Id { get; set; }

        public int Node1Id { get; set; }
        public int Node2Id { get; set; }
        public int Weight { get; set; }

        [ForeignKey(nameof(Node1Id))]
        public virtual Node? Node1 { get; set; }

        [ForeignKey(nameof(Node2Id))]
        public virtual Node? Node2 { get; set; }
    }


}
