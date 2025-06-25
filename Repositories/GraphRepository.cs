using MinimumSpanningTreeWithKruskal.Data;
using MinimumSpanningTreeWithKruskal.Interfaces;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Repositories
{
    public class GraphRepository : IGraphRepository
    {
        private readonly GraphDbContext _db;
        public GraphRepository(GraphDbContext db)
        {
            _db = db;
        }
        public Graph GetGraphById(int graphId) => _db.Graphs.FirstOrDefault(g => g.Id == graphId);
        public IEnumerable<Node> GetNodesByGraphId(int graphId) => _db.Nodes.Where(n => n.GraphId == graphId).ToList();
        public IEnumerable<Edge> GetEdgesByGraphId(int graphId) => _db.Edges.Where(e => e.Node1.GraphId == graphId && e.Node2.GraphId == graphId).ToList();
        public void AddGraph(Graph graph) => _db.Graphs.Add(graph);
        public void AddNode(Node node) => _db.Nodes.Add(node);
        public void AddEdge(Edge edge) => _db.Edges.Add(edge);
        public void RemoveGraph(Graph graph) => _db.Graphs.Remove(graph);
        public void RemoveNode(Node node) => _db.Nodes.Remove(node);
        public void RemoveEdge(Edge edge) => _db.Edges.Remove(edge);
        public void SaveChanges() => _db.SaveChanges();
        public ApplicationUser GetUserById(string userId) => _db.Users.FirstOrDefault(u => u.Id == userId);
        public bool GraphExists(string graphName, string userId) => _db.Graphs.Any(x => x.Name == graphName && x.UserId == userId);
        public IEnumerable<Graph> GetGraphs(string userId) => _db.Graphs.Where(g => g.UserId == userId).ToList();
        public Graph GetGraph(int graphId, string userId) => _db.Graphs.FirstOrDefault(g => g.Id == graphId && g.UserId == userId);
        public void RemoveNodes(IEnumerable<Node> nodes) => _db.Nodes.RemoveRange(nodes);
        public void RemoveEdges(IEnumerable<Edge> edges) => _db.Edges.RemoveRange(edges);
        public IEnumerable<Node> GetNodes(int graphId) => _db.Nodes.Where(n => n.GraphId == graphId).ToList();
        public IEnumerable<Edge> GetEdges(int graphId) => _db.Edges.Where(e => e.Node1.GraphId == graphId && e.Node2.GraphId == graphId).ToList();
        public Graph GetGraph(int graphId) => _db.Graphs.FirstOrDefault(g => g.Id == graphId);
    }
}