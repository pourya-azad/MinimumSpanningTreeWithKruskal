using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Services;
using MinimumSpanningTreeWithKruskal.ViewModel;

namespace MinimumSpanningTreeWithKruskal.Controllers
{
    public class GraphController : Controller
    {
        private readonly GraphDbContext _db;
        private readonly GraphService _service;

        public GraphController(GraphDbContext db, GraphService service)
        {
            _db = db;
            _service = service;
        }

        public ActionResult Index() => View();

        [HttpPost]
        public ActionResult Index(GraphInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.JsonData))
            {
                ModelState.AddModelError("JsonData", "JSON نمی‌تواند خالی باشد.");
                return View(model);
            }

            GraphData input;
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                input = JsonSerializer.Deserialize<GraphData>(model.JsonData, options);
                if (input is null)
                {
                    ModelState.AddModelError("JsonData", "JSON نامعتبر است.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("JsonData", $"خطا در پردازش JSON: {ex.Message}");
                return View(model);
            }

            var nodeSet = input.Nodes.Select(n => n.Label).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var edge in input.Edges)
            {
                if (!nodeSet.Contains(edge.Source) || !nodeSet.Contains(edge.Target))
                {
                    ModelState.AddModelError("JsonData", $"یال به نود ناموجود اشاره می‌کند: {edge.Source}-{edge.Target}");
                    return View(model);
                }
            }

            _db.Nodes.RemoveRange(_db.Nodes);
            _db.Edges.RemoveRange(_db.Edges);
            _db.MSTEdges.RemoveRange(_db.MSTEdges);
            _db.SaveChanges();

            // ذخیره نودها
            var labelToNode = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in input.Nodes.DistinctBy(n => n.Label))
            {
                var newNode = new Node { Label = node.Label };
                _db.Nodes.Add(newNode);
                labelToNode[node.Label] = newNode;
            }
            _db.SaveChanges();

            // ذخیره یال‌ها
            var addedEdges = new HashSet<(int, int)>();
            foreach (var e in input.Edges)
            {
                var node1 = labelToNode[e.Source];
                var node2 = labelToNode[e.Target];
                int id1 = node1.Id;
                int id2 = node2.Id;

                var key = (Math.Min(id1, id2), Math.Max(id1, id2));
                if (!addedEdges.Contains(key))
                {
                    _db.Edges.Add(new Edge
                    {
                        Node1Id = key.Item1,
                        Node2Id = key.Item2,
                        Weight = e.Weight
                    });
                    addedEdges.Add(key);
                }
            }
            _db.SaveChanges();

            return RedirectToAction("Show");
        }

        public ActionResult Show()
        {
            var nodes = _db.Nodes.ToList();
            var edges = _db.Edges.ToList();
            var mstEdges = _db.MSTEdges.Select(me => me.Edge).ToList();

            var vm = new GraphViewModel
            {
                Nodes = nodes,
                Edges = edges,
                MSTEdges = mstEdges
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Compute()
        {
            var mst = _service.ComputeMST();
            _service.SaveMST(mst);
            return RedirectToAction("Show");
        }
    }
}
