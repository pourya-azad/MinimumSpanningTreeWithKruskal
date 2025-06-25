using System.Collections.Generic;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IGraphService
    {
        IList<Edge> ComputeMST(int GraphId);
        void SaveMST(IList<Edge> mst, int graphId);
    }
} 