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

        // ۱. نمایش فرم دریافت گراف
        public ActionResult Index()
        {
            return View();
        }

        // ۲. دریافت و ذخیره گراف
        [HttpPost]
        public ActionResult Index(GraphInputModel model)
        {
            if (string.IsNullOrWhiteSpace(model.JsonData))
            {
                ModelState.AddModelError("JsonData", "JSON نمی‌تواند خالی باشد.");
                return View(model);
            }

            Console.WriteLine($"JsonData: {model.JsonData}");

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

                Console.WriteLine("Edges after deserialization:");
                foreach (var edge in input.Edges)
                {
                    Console.WriteLine($"Source: '{edge.Source}', Target: '{edge.Target}', Weight: {edge.Weight}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                ModelState.AddModelError("JsonData", $"خطا در پردازش JSON: {ex.Message}");
                return View(model);
            }

            // اعتبارسنجی نودها و یال‌ها
            var nodeSet = input.Nodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var edge in input.Edges)
            {
                if (string.IsNullOrEmpty(edge.Source) || string.IsNullOrEmpty(edge.Target))
                {
                    ModelState.AddModelError("JsonData", $"یال شامل Source یا Target خالی است: Source='{edge.Source}', Target='{edge.Target}'");
                    return View(model);
                }
                if (!nodeSet.Contains(edge.Source) || !nodeSet.Contains(edge.Target))
                {
                    ModelState.AddModelError("JsonData", $"یال با Source '{edge.Source}' یا Target '{edge.Target}' به نود ناموجود اشاره می‌کند.");
                    return View(model);
                }
            }

            // پاک‌سازی دیتابیس
            _db.Nodes.RemoveRange(_db.Nodes);
            _db.Edges.RemoveRange(_db.Edges);
            _db.MSTEdges.RemoveRange(_db.MSTEdges);
            _db.SaveChanges();

            // ذخیره نودها
            foreach (var label in input.Nodes.Distinct())
                _db.Nodes.Add(new Node { Label = label });
            _db.SaveChanges();

            Console.WriteLine($"Nodes saved: {_db.Nodes.Count()}");

            // نگاشت لیبل به Id
            var map = _db.Nodes.ToDictionary(n => n.Label, n => n.Id, StringComparer.OrdinalIgnoreCase);

            // ذخیره یال‌ها — بدون جهت و بدون تکرار
            var addedEdges = new HashSet<(int, int)>();
            foreach (var e in input.Edges)
            {
                int id1 = map[e.Source];
                int id2 = map[e.Target];
                int node1Id = Math.Min(id1, id2);
                int node2Id = Math.Max(id1, id2);

                var edgeKey = (node1Id, node2Id);
                if (!addedEdges.Contains(edgeKey))
                {
                    _db.Edges.Add(new Edge
                    {
                        Node1Id = node1Id,
                        Node2Id = node2Id,
                        Weight = e.Weight
                    });
                    addedEdges.Add(edgeKey);
                }
            }
            _db.SaveChanges();

            return RedirectToAction("Show");
        }

        // ۳. نمایش گراف و MST
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

        // ۴. محاسبه و ذخیرهٔ MST
        [HttpPost]
        public ActionResult Compute()
        {
            var mst = _service.ComputeMST();
            _service.SaveMST(mst);
            return RedirectToAction("Show");
        }
    }
}
