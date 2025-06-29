using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class Graph
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Node> NodeAsGraph { get; set; } = new List<Node>();
    }
}
