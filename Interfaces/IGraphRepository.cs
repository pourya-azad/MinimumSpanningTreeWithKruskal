using System.Collections.Generic;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IGraphRepository
    {
        Graph GetGraphById(int graphId);
        IEnumerable<Node> GetNodesByGraphId(int graphId);
        IEnumerable<Edge> GetEdgesByGraphId(int graphId);
        void AddGraph(Graph graph);
        void AddNode(Node node);
        void AddEdge(Edge edge);
        void RemoveGraph(Graph graph);
        void RemoveNode(Node node);
        void RemoveEdge(Edge edge);
        void SaveChanges();
        ApplicationUser GetUserById(string userId);
        bool GraphExists(string graphName, string userId);
        IEnumerable<Graph> GetGraphs(string userId);
        Graph GetGraph(int graphId, string userId);
        void RemoveNodes(IEnumerable<Node> nodes);
        void RemoveEdges(IEnumerable<Edge> edges);
        IEnumerable<Node> GetNodes(int graphId);
        IEnumerable<Edge> GetEdges(int graphId);
        Graph GetGraph(int graphId);
    }
} 