using Microsoft.AspNetCore.Identity;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Graph> GraphAsUser { get; set; } = new List<Graph>();
    }
}