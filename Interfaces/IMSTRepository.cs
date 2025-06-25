using System.Collections.Generic;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IMSTRepository
    {
        IEnumerable<MSTEdge> GetMSTEdgesByGraphId(int graphId);
        void RemoveMSTEdgesByGraphId(int graphId);
        void AddMSTEdge(MSTEdge mstEdge);
        void SaveChanges();
        IEnumerable<MSTEdge> GetMSTEdges(int graphId);
        bool MSTExists(int graphId);
        void RemoveMSTEdges(IEnumerable<MSTEdge> mstEdges);
    }
} 