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
        private readonly IGraphRepository _graphRepository;
        private readonly IMSTRepository _mstRepository;
        private readonly IGraphService _service;
        private readonly IGraphValidator _validator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGraphInputHandlerService _inputHandler;

        public GraphController(IGraphRepository graphRepository, IMSTRepository mstRepository, IGraphService service, IGraphValidator validator, UserManager<ApplicationUser> userManager, IGraphInputHandlerService inputHandler)
        {
            _graphRepository = graphRepository;
            _mstRepository = mstRepository;
            _service = service;
            _validator = validator;
            _userManager = userManager;
            _inputHandler = inputHandler;
        }

        public ActionResult Index() => View();

        [HttpPost]
        public ActionResult Index(GraphInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (input, errors) = _inputHandler.ParseAndValidate(model.JsonData);
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
            var user = _graphRepository.GetUserById(userIdStr);
            if (user == null)
            {
                return Challenge();
            }
            if (_graphRepository.GraphExists(input!.GraphName, userIdStr))
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
            _graphRepository.AddGraph(graph);
            _graphRepository.SaveChanges(); // تا Graph.Id مقداردهی شود

            // ذخیره نودها
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

            // ذخیره یال‌ها
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

            return RedirectToAction("Show");
        }

        public ActionResult Show(int? graphId = null, bool showMST = false)
        {
            if (!graphId.HasValue)
            {
                return RedirectToAction("History");
            }
            var nodes = _graphRepository.GetNodes(graphId.Value)?.Where(n => n != null).ToList() ?? new List<Node>();
            var edges = _graphRepository.GetEdges(graphId.Value)?.Where(e => e != null).ToList() ?? new List<Edge>();
            var mstEdges = _mstRepository.GetMSTEdges(graphId.Value)?.Where(me => me?.Edge != null).Select(me => me.Edge!).ToList() ?? new List<Edge>();

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
            var hasMST = _mstRepository.MSTExists(GraphId);
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
            var graphs = _graphRepository.GetGraphs(userId).ToList();
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
            var graph = _graphRepository.GetGraph(graphId, userIdStr);
            if (graph == null)
            {
                ModelState.AddModelError("GraphName", "گرافی با این آیدی برای حذف یافت نشد.");
                return RedirectToAction("History");
            }

            // حذف MSTEdges
            var mstEdges = _mstRepository.GetMSTEdges(graphId).ToList();
            _mstRepository.RemoveMSTEdges(mstEdges);

            // حذف Edges
            var edges = _graphRepository.GetEdges(graphId).ToList();
            _graphRepository.RemoveEdges(edges);

            // حذف Nodes
            var nodes = _graphRepository.GetNodes(graphId).ToList();
            _graphRepository.RemoveNodes(nodes);

            // حذف خود گراف
            _graphRepository.RemoveGraph(graph);

            _graphRepository.SaveChanges();
            return RedirectToAction("History");
        }

        // دانلود گراف کامل به فرمت ورودی Index
        [HttpGet]
        public IActionResult DownloadGraphJson(int graphId)
        {
            var graph = _graphRepository.GetGraph(graphId);
            var nodes = _graphRepository.GetNodes(graphId).Select(n => new { id = n.Id, label = n.Label }).ToList();
            var nodeIdToLabel = nodes.ToDictionary(n => n.id, n => n.label);
            var edges = _graphRepository.GetEdges(graphId)
                .Select(e => new { source = e.Node1.Label, target = e.Node2.Label, weight = e.Weight }).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(new { graphName = graph?.Name, nodes, edges });
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"graph_{graphId}.json");
        }

        // دانلود MST به فرمت ورودی Index
        [HttpGet]
        public IActionResult DownloadMstJson(int graphId)
        {
            var graph = _graphRepository.GetGraph(graphId);
            var nodes = _graphRepository.GetNodes(graphId).Select(n => new { id = n.Id, label = n.Label }).ToList();
            var nodeIdToLabel = nodes.ToDictionary(n => n.id, n => n.label);
            var mstEdges = _mstRepository.GetMSTEdges(graphId)
                .Where(me => me != null && me.Edge != null && me.Edge.Node1 != null && me.Edge.Node2 != null)
                .Select(me => new { 
                    source = me.Edge.Node1.Label, 
                    target = me.Edge.Node2.Label, 
                    weight = me.Edge.Weight 
                }).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(new { graphName = graph?.Name, nodes, edges = mstEdges });
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", $"mst_{graphId}.json");
        }
    }
}
