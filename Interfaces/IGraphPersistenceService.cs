using MinimumSpanningTreeWithKruskal.Models;
using System.Collections.Generic;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IGraphPersistenceService
    {
        int SaveGraph(GraphData input, string userId);
        void DeleteGraph(int graphId, string userId);
    }
} 