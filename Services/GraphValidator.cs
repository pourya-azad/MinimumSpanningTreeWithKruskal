using MinimumSpanningTreeWithKruskal.Models;
using System.Collections.Generic;
using System.Linq;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphValidator
    {
        public List<string> Validate(GraphData input)
        {
            var errors = new List<string>();
            if (input == null)
            {
                errors.Add("فرمت JSON نامعتبر است.");
                return errors;
            }
            if (string.IsNullOrWhiteSpace(input.GraphName))
                errors.Add("نام گراف الزامی است.");
            if (input.Nodes == null || input.Nodes.Count < 1)
                errors.Add("حداقل یک نود باید وارد شود.");
            if (input.Edges == null || input.Edges.Count < 1)
                errors.Add("حداقل یک یال باید وارد شود.");
            if (input.Nodes != null)
            {
                foreach (var node in input.Nodes)
                {
                    if (string.IsNullOrWhiteSpace(node.Label))
                        errors.Add("برچسب نود الزامی است.");
                }
            }
            if (input.Edges != null)
            {
                foreach (var edge in input.Edges)
                {
                    if (string.IsNullOrWhiteSpace(edge.Source))
                        errors.Add("مبدأ یال الزامی است.");
                    if (string.IsNullOrWhiteSpace(edge.Target))
                        errors.Add("مقصد یال الزامی است.");
                    if (edge.Weight < 1)
                        errors.Add("وزن یال باید مثبت باشد.");
                }
            }
            // اعتبارسنجی متصل بودن گراف
            if (input.Nodes != null && input.Edges != null && input.Nodes.Count > 0 && input.Edges.Count > 0)
            {
                if (!IsGraphConnected(input.Nodes, input.Edges))
                    errors.Add("گراف باید متصل باشد (تمام نودها باید به هم راه داشته باشند).");
            }
            return errors;
        }

        private bool IsGraphConnected(List<NodeInput> nodes, List<EdgeInput> edges)
        {
            if (nodes.Count == 0) return false;
            var labelSet = new HashSet<string>(nodes.Select(n => n.Label));
            var adj = nodes.ToDictionary(n => n.Label, n => new List<string>());
            foreach (var e in edges)
            {
                if (labelSet.Contains(e.Source) && labelSet.Contains(e.Target))
                {
                    adj[e.Source].Add(e.Target);
                    adj[e.Target].Add(e.Source);
                }
            }
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(nodes[0].Label);
            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                if (!visited.Add(curr)) continue;
                foreach (var next in adj[curr])
                    if (!visited.Contains(next))
                        queue.Enqueue(next);
            }
            return visited.Count == nodes.Count;
        }
    }
} 