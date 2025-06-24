using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MinimumSpanningTreeWithKruskal.Models
{
    public class GraphDbContext : IdentityDbContext<ApplicationUser>
    {
        public GraphDbContext(DbContextOptions<GraphDbContext> options) : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Edge> Edges { get; set; }
        public DbSet<MSTEdge> MSTEdges { get; set; }

        public DbSet<Graph> Graphs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            modelBuilder.Entity<Node>()
                .HasOne(me => me.Graph)
                .WithMany(me => me.NodeAsGraph)
                .HasForeignKey(m => m.GraphId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Graph>()
                .HasOne(g => g.ApplicationUser)
                .WithMany(u => u.GraphAsUser)
                .HasForeignKey(me => me.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}