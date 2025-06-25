using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphPersistenceService : IGraphPersistenceService
    {
        private readonly IGraphRepository _graphRepository;
        private readonly IMSTRepository _mstRepository;
        public GraphPersistenceService(IGraphRepository graphRepository, IMSTRepository mstRepository)
        {
            _graphRepository = graphRepository;
            _mstRepository = mstRepository;
        }

        public int SaveGraph(GraphData input, string userId)
        {
            var user = _graphRepository.GetUserById(userId);
            if (user == null)
                throw new Exception("کاربر یافت نشد.");
            if (_graphRepository.GraphExists(input.GraphName, userId))
                throw new Exception("شما قبلاً گرافی با این نام ثبت کرده‌اید.");

            var graph = new Graph
            {
                Name = input.GraphName,
                UserId = userId,
                ApplicationUser = user
            };
            _graphRepository.AddGraph(graph);
            _graphRepository.SaveChanges();

            var labelToNode = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in input.Nodes.Where(n => n != null).DistinctBy(n => n.Label))
            {
                var newNode = new Node
                {
                    Label = node.Label,
                    GraphId = graph.Id,
                };
                _graphRepository.AddNode(newNode);
                labelToNode[node.Label] = newNode;
            }
            _graphRepository.SaveChanges();

            var addedEdges = new HashSet<(int, int)>();
            foreach (var e in input.Edges.Where(e => e != null))
            {
                var node1 = labelToNode[e.Source];
                var node2 = labelToNode[e.Target];
                int id1 = node1.Id;
                int id2 = node2.Id;
                var key = (Math.Min(id1, id2), Math.Max(id1, id2));
                if (!addedEdges.Contains(key))
                {
                    _graphRepository.AddEdge(new Edge
                    {
                        Node1Id = key.Item1,
                        Node2Id = key.Item2,
                        Weight = e.Weight
                    });
                    addedEdges.Add(key);
                }
            }
            _graphRepository.SaveChanges();
            return graph.Id;
        }

        public void DeleteGraph(int graphId, string userId)
        {
            var graph = _graphRepository.GetGraph(graphId, userId);
            if (graph == null)
                throw new Exception("گرافی با این آیدی برای حذف یافت نشد.");

            var mstEdges = _mstRepository.GetMSTEdges(graphId).ToList();
            _mstRepository.RemoveMSTEdges(mstEdges);

            var edges = _graphRepository.GetEdges(graphId).ToList();
            _graphRepository.RemoveEdges(edges);

            var nodes = _graphRepository.GetNodes(graphId).ToList();
            _graphRepository.RemoveNodes(nodes);

            _graphRepository.RemoveGraph(graph);
            _graphRepository.SaveChanges();
        }
    }
} 