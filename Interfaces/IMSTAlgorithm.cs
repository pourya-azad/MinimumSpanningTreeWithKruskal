using System.Collections.Generic;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IMSTAlgorithm
    {
        IList<Edge> ComputeMST(IEnumerable<Edge> edges, List<int> nodeIds);
    }
} 