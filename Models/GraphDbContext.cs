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
            // پیکربندی رابطه بین Edge و Node برای Source
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.Source)
                .WithMany(n => n.OutgoingEdges)
                .HasForeignKey(e => e.SourceId)
                .OnDelete(DeleteBehavior.Cascade); // یا DeleteBehavior.Restrict بسته به نیاز

            // پیکربندی رابطه بین Edge و Node برای Target
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.Target)
                .WithMany(n => n.IncomingEdges)
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Restrict); // یا DeleteBehavior.Restrict بسته به نیاز

            // پیکربندی رابطه بین MSTEdge و Edge
            modelBuilder.Entity<MSTEdge>()
                .HasOne(me => me.Edge)
                .WithMany() // اگر Edge رابطه‌ای به MSTEdge ندارد، خالی بگذارید
                .HasForeignKey(me => me.EdgeId)
                .OnDelete(DeleteBehavior.Cascade); // یا DeleteBehavior.Restrict بسته به نیاز
        }
    }
}