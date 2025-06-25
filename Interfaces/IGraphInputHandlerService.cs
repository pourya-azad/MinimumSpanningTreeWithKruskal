using MinimumSpanningTreeWithKruskal.Models;
using System.Collections.Generic;

namespace MinimumSpanningTreeWithKruskal.Interfaces
{
    public interface IGraphInputHandlerService
    {
        (GraphData? graphData, List<string> errors) ParseAndValidate(string jsonData);
    }
} 