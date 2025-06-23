using System.ComponentModel.DataAnnotations.Schema;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class Edge
    {
        public int Id { get; set; }

        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public int Weight { get; set; }

        [ForeignKey(nameof(SourceId))]
        public virtual Node? Source { get; set; }

        [ForeignKey(nameof(TargetId))]
        public virtual Node? Target { get; set; }
    }

}
