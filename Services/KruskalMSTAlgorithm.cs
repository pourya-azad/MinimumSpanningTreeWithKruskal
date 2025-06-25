using System.Collections.Generic;
using System.Linq;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class KruskalMSTAlgorithm : IMSTAlgorithm
    {
        public IList<Edge> ComputeMST(IEnumerable<Edge> edges, List<int> nodeIds)
        {
            var orderedEdges = edges.OrderBy(e => e.Weight).ToList();
            var index = nodeIds.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);
            var ds = new DisjointSet(nodeIds.Count);
            var mst = new List<Edge>();
            foreach (var e in orderedEdges)
            {
                if (ds.Union(index[e.Node1Id], index[e.Node2Id]))
                    mst.Add(e);
            }
            return mst;
        }
    }
} 