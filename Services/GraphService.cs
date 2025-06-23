using Microsoft.EntityFrameworkCore;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphService
    {
        private readonly GraphDbContext _db;
        public GraphService(GraphDbContext db) { _db = db; }

        public IList<Edge> ComputeMST()
        {
            var edges = _db.Edges
                .Include(e => e.Node1)
                .Include(e => e.Node2)
                .ToList()
                .OrderBy(e => e.Weight)
                .ToList();

            var nodes = _db.Nodes.Select(n => n.Id).ToList();
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


        public void SaveMST(IList<Edge> mst)
        {
            // اول همهٔ رکوردهای قدیمی را پاک می‌کنیم
            _db.MSTEdges.RemoveRange(_db.MSTEdges);
            _db.SaveChanges();

            // سپس MST جدید را اضافه می‌کنیم
            foreach (var e in mst)
            {
                _db.MSTEdges.Add(new MSTEdge { EdgeId = e.Id });
            }
            _db.SaveChanges();
        }
    }

}
