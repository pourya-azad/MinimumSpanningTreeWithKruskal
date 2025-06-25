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
        private readonly IGraphPersistenceService _persistenceService;

        public GraphController(IGraphRepository graphRepository, IMSTRepository mstRepository, IGraphService service, IGraphValidator validator, UserManager<ApplicationUser> userManager, IGraphInputHandlerService inputHandler, IGraphPersistenceService persistenceService)
        {
            _graphRepository = graphRepository;
            _mstRepository = mstRepository;
            _service = service;
            _validator = validator;
            _userManager = userManager;
            _inputHandler = inputHandler;
            _persistenceService = persistenceService;
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
            try
            {
                _persistenceService.SaveGraph(input!, userIdStr);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
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
            try
            {
                _persistenceService.DeleteGraph(graphId, userIdStr);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("GraphName", ex.Message);
            }
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
