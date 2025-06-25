using Microsoft.EntityFrameworkCore;
using MinimumSpanningTreeWithKruskal.Data;
using MinimumSpanningTreeWithKruskal.Interfaces;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Repositories
{
    public class MSTRepository : IMSTRepository
    {
        private readonly GraphDbContext _db;
        public MSTRepository(GraphDbContext db)
        {
            _db = db;
        }
        public IEnumerable<MSTEdge> GetMSTEdgesByGraphId(int graphId)
            => _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId).ToList();
        public void RemoveMSTEdgesByGraphId(int graphId)
        {
            var oldMSTs = _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId).ToList();
            if (oldMSTs.Any())
                _db.MSTEdges.RemoveRange(oldMSTs);
        }
        public void AddMSTEdge(MSTEdge mstEdge) => _db.MSTEdges.Add(mstEdge);
        public void SaveChanges() => _db.SaveChanges();
        public IEnumerable<MSTEdge> GetMSTEdges(int graphId)
            => _db.MSTEdges
                .Include(me => me.Edge)
                    .ThenInclude(e => e.Node1)
                .Include(me => me.Edge)
                    .ThenInclude(e => e.Node2)
                .Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId)
                .ToList();
        public bool MSTExists(int graphId)
            => _db.MSTEdges.Any(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId);
        public void RemoveMSTEdges(IEnumerable<MSTEdge> mstEdges)
            => _db.MSTEdges.RemoveRange(mstEdges);
    }
}