using System.Collections.Generic;
using MinimumSpanningTreeWithKruskal.Models;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IGraphValidator
    {
        List<string> Validate(GraphData input);
    }
} 