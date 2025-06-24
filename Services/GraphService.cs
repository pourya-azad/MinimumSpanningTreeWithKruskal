using Microsoft.EntityFrameworkCore;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphService
    {
        private readonly GraphDbContext _db;
        public GraphService(GraphDbContext db) { _db = db; }

        public IList<Edge> ComputeMST(int GraphId)
        {
            var edges = _db.Edges
                .Where(e => e.Node1.GraphId == GraphId && e.Node2.GraphId == GraphId)
                .Include(e => e.Node1)
                .Include(e => e.Node2)
                .ToList()
                .OrderBy(e => e.Weight)
                .ToList();

            var nodes = _db.Nodes.Where(n => n.GraphId == GraphId).Select(n => n.Id).ToList();
            var index = nodes.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);

            var ds = new DisjointSet(nodes.Count);
            var mst = new List<Edge>();

            foreach (var e in edges)
            {
                if (ds.Union(index[e.Node1Id], index[e.Node2Id]))
                    mst.Add(e);
            }

            return mst;
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
