using Microsoft.EntityFrameworkCore;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphService : IGraphService
    {
        private readonly IGraphRepository _graphRepository;
        private readonly IMSTRepository _mstRepository;
        private readonly IMSTAlgorithm _mstAlgorithm;

        public GraphService(IGraphRepository graphRepository, IMSTRepository mstRepository, IMSTAlgorithm mstAlgorithm)
        {
            _graphRepository = graphRepository;
            _mstRepository = mstRepository;
            _mstAlgorithm = mstAlgorithm;
        }

        public IList<Edge> ComputeMST(int GraphId)
        {
            var edges = _graphRepository.GetEdgesByGraphId(GraphId).ToList();
            var nodes = _graphRepository.GetNodesByGraphId(GraphId).Select(n => n.Id).ToList();
            return _mstAlgorithm.ComputeMST(edges, nodes);
        }

        public void SaveMST(IList<Edge> mst, int graphId)
        {
            _mstRepository.RemoveMSTEdgesByGraphId(graphId);
            _mstRepository.SaveChanges();
            if (mst.Count > 0)
            {
                foreach (var e in mst)
                {
                    _mstRepository.AddMSTEdge(new MSTEdge { EdgeId = e.Id });
                }
                _mstRepository.SaveChanges();
            }
        }
    }
}
