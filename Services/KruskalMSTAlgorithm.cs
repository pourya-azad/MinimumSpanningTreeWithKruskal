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
            var orderedEdges = edges.OrderBy(edge => edge.Weight).ToList();
            var index = nodeIds.Select((id, idx) => new { id, idx })
                .ToDictionary(x => x.id, x => x.idx);
            var ds = new DisjointSet(nodeIds.Count);
            var mst = new List<Edge>();
            foreach (var edge in orderedEdges)
            {
                if (ds.Union(index[edge.Node1Id], index[edge.Node2Id]))
                    mst.Add(edge);
            }
            return mst;
        }
    }
} 