using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Services;
using MinimumSpanningTreeWithKruskal.ViewModel;
using MinimumSpanningTreeWithKruskal.Interfaces;

namespace MinimumSpanningTreeWithKruskal.Controllers
{
    [Authorize]
    public class GraphController : Controller
    {
        private readonly GraphDbContext _db;
        private readonly IGraphService _service;
        private readonly IGraphValidator _validator;
        private readonly UserManager<ApplicationUser> _userManager;

        public GraphController(GraphDbContext db, IGraphService service, IGraphValidator validator, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _service = service;
            _validator = validator;
            _userManager = userManager;
        }

        public ActionResult Index() => View();

        [HttpPost]
        public ActionResult Index(GraphInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            GraphData input;
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                input = JsonSerializer.Deserialize<GraphData>(model.JsonData, options);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در پردازش JSON: {ex.Message}");
                return View(model);
            }

            var errors = _validator.Validate(input);
            if (errors.Any())
            {
                foreach (var err in errors)
                    ModelState.AddModelError("", err);
                return View(model);
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Challenge(); // کاربر لاگین نیست
            }
            var user = _db.Users.FirstOrDefault(u => u.Id == userIdStr);
            if (user == null)
            {
                return Challenge();
            }
            if (_db.Graphs.Any(x => x.Name == input.GraphName && x.UserId == userIdStr))
            {
                ModelState.AddModelError("GraphName", "شما قبلاً گرافی با این نام ثبت کرده‌اید.");
                return View(model);
            }

            var graph = new Graph
            {
                Name = input.GraphName,
                UserId = userIdStr,
                ApplicationUser = user
            };
            _db.Graphs.Add(graph);
            _db.SaveChanges(); // تا Graph.Id مقداردهی شود

            // ذخیره نودها
            var labelToNode = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in input.Nodes.DistinctBy(n => n.Label))
            {
                var newNode = new Node
                {
                    Label = node.Label,
                    GraphId = graph.Id,
                };
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

        public ActionResult Show(int? graphId = null, bool showMST = false)
        {
            if (graphId == null)
            {
                return RedirectToAction("History");
            }
            var nodes = _db.Nodes.Where(n => n.GraphId == graphId).ToList();
            var edges = _db.Edges.Where(e => e.Node1.GraphId == graphId && e.Node2.GraphId == graphId).ToList();
            var mstEdges = _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId).Select(me => me.Edge).ToList();

            var vm = new GraphViewModel
            {
                Nodes = nodes,
                Edges = edges,
                MSTEdges = mstEdges,
                ShowMST = showMST
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Compute(int GraphId)
        {
            // اگر قبلاً برای این گراف MST ذخیره شده بود، هیچ کاری انجام نده
            var hasMST = _db.MSTEdges.Any(me => me.Edge.Node1.GraphId == GraphId && me.Edge.Node2.GraphId == GraphId);
            if (hasMST)
            {
                return RedirectToAction("Show", new { graphId = GraphId });
            }
            var mst = _service.ComputeMST(GraphId);
            _service.SaveMST(mst, GraphId);
            return RedirectToAction("Show", new { graphId = GraphId });
        }

        [HttpGet]
        public ActionResult History()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Challenge();
            }
            string userId = userIdStr;
            var graphs = _db.Graphs.Where(g => g.UserId == userId).ToList();
            return View(graphs);
        }

        [HttpPost]
        public ActionResult Delete(int graphId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Challenge();
            }
            var graph = _db.Graphs.FirstOrDefault(g => g.Id == graphId && g.UserId == userIdStr);
            if (graph == null)
            {
                ModelState.AddModelError("GraphName", "گرافی با این آیدی برای حذف یافت نشد.");
                return RedirectToAction("History");
            }

            // حذف MSTEdges
            var mstEdges = _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId).ToList();
            _db.MSTEdges.RemoveRange(mstEdges);

            // حذف Edges
            var edges = _db.Edges.Where(e => e.Node1.GraphId == graphId && e.Node2.GraphId == graphId).ToList();
            _db.Edges.RemoveRange(edges);

            // حذف Nodes
            var nodes = _db.Nodes.Where(n => n.GraphId == graphId).ToList();
            _db.Nodes.RemoveRange(nodes);

            // حذف خود گراف
            _db.Graphs.Remove(graph);

            _db.SaveChanges();
            return RedirectToAction("History");
        }

        // دانلود گراف کامل به فرمت ورودی Index
        [HttpGet]
        public IActionResult DownloadGraphJson(int graphId)
        {
            var graph = _db.Graphs.FirstOrDefault(g => g.Id == graphId);
            var nodes = _db.Nodes.Where(n => n.GraphId == graphId).Select(n => new { id = n.Id, label = n.Label }).ToList();
            var nodeIdToLabel = nodes.ToDictionary(n => n.id, n => n.label);
            var edges = _db.Edges.Where(e => e.Node1.GraphId == graphId && e.Node2.GraphId == graphId)
                .Select(e => new { source = e.Node1.Label, target = e.Node2.Label, weight = e.Weight }).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(new { graphName = graph?.Name, nodes, edges });
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"graph_{graphId}.json");
        }

        // دانلود MST به فرمت ورودی Index
        [HttpGet]
        public IActionResult DownloadMstJson(int graphId)
        {
            var graph = _db.Graphs.FirstOrDefault(g => g.Id == graphId);
            var nodes = _db.Nodes.Where(n => n.GraphId == graphId).Select(n => new { id = n.Id, label = n.Label }).ToList();
            var nodeIdToLabel = nodes.ToDictionary(n => n.id, n => n.label);
            var mstEdges = _db.MSTEdges.Where(me => me.Edge.Node1.GraphId == graphId && me.Edge.Node2.GraphId == graphId)
                .Select(me => new { source = me.Edge.Node1.Label, target = me.Edge.Node2.Label, weight = me.Edge.Weight }).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(new { graphName = graph?.Name, nodes, edges = mstEdges });
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"mst_{graphId}.json");
        }
    }
}
