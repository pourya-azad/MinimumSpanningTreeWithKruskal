using System;
using System.Collections.Generic;
using System.Text.Json;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;

namespace MinimumSpanningTreeWithKruskal.Services
{
    public class GraphInputHandlerService : IGraphInputHandlerService
    {
        private readonly IGraphValidator _validator;
        public GraphInputHandlerService(IGraphValidator validator)
        {
            _validator = validator;
        }

        public (GraphData? graphData, List<string> errors) ParseAndValidate(string jsonData)
        {
            GraphData? input = null;
            var errors = new List<string>();
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                input = JsonSerializer.Deserialize<GraphData>(jsonData, options);
            }
            catch (Exception ex)
            {
                errors.Add($"خطا در پردازش JSON: {ex.Message}");
                return (null, errors);
            }

            if (input == null)
            {
                errors.Add("ورودی نامعتبر است.");
                return (null, errors);
            }

            errors.AddRange(_validator.Validate(input));
            return (input, errors);
        }
    }
} 