using Microsoft.EntityFrameworkCore;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class GraphDbContext : DbContext
    {
        public GraphDbContext(DbContextOptions<GraphDbContext> options) : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Edge> Edges { get; set; }
        public DbSet<MSTEdge> MSTEdges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // رابطه Node1
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.Node1)
                .WithMany(n => n.EdgesAsNode1)
                .HasForeignKey(e => e.Node1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // رابطه Node2
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.Node2)
                .WithMany(n => n.EdgesAsNode2)
                .HasForeignKey(e => e.Node2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // رابطه MSTEdge
            modelBuilder.Entity<MSTEdge>()
                .HasOne(me => me.Edge)
                .WithMany()
                .HasForeignKey(me => me.EdgeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}