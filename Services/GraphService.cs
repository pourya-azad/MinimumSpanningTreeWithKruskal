using Microsoft.EntityFrameworkCore;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphDbContext _db;
        private readonly IMSTAlgorithm _mstAlgorithm;

        public GraphService(GraphDbContext db, IMSTAlgorithm mstAlgorithm)
        {
            _db = db;
            _mstAlgorithm = mstAlgorithm;
        }

        public IList<Edge> ComputeMST(int GraphId)
        {
            var edges = _db.Edges
                .Where(e => e.Node1.GraphId == GraphId && e.Node2.GraphId == GraphId)
                .Include(e => e.Node1)
                .Include(e => e.Node2)
                .ToList();
            var nodes = _db.Nodes.Where(n => n.GraphId == GraphId).Select(n => n.Id).ToList();
            return _mstAlgorithm.ComputeMST(edges, nodes);
        }

        public void SaveMST(IList<Edge> mst, int graphId)
        {
            // حذف MSTهای قبلی فقط برای همین گراف
            var oldMSTs = _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId).ToList();
            if (oldMSTs.Any())
            {
                _db.MSTEdges.RemoveRange(oldMSTs);
                _db.SaveChanges();
            }

            // ذخیره MST جدید فقط اگر mst خالی نباشد
            if (mst.Count > 0)
            {
                foreach (var e in mst)
                {
                    _db.MSTEdges.Add(new MSTEdge { EdgeId = e.Id });
                }
                _db.SaveChanges();
            }
        }
    }
}
