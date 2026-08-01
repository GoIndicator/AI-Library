using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace AILibrary.Services
{
    public class LibraryItem
    {
        public string Id { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Skill", "Plugin", "Edicao", etc.
        public string Category { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public List<string> Tags { get; set; } = new();
        public string OfficialLink { get; set; } = string.Empty;
        public List<string> MethodSteps { get; set; } = new();
        public string Prompt { get; set; } = string.Empty;
        public string SourceContentTitle { get; set; } = string.Empty;
        public string SourceContentLink { get; set; } = string.Empty;
    }

    public class LibraryService
    {
        private readonly List<LibraryItem> _items = new();

        public LibraryService(IWebHostEnvironment webHostEnvironment)
        {
            var jsonPath = Path.Combine(webHostEnvironment.WebRootPath, "data", "library.json");
            if (File.Exists(jsonPath))
            {
                try
                {
                    var jsonContent = File.ReadAllText(jsonPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _items = JsonSerializer.Deserialize<List<LibraryItem>>(jsonContent, options) ?? new();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading library JSON: {ex.Message}");
                }
            }
        }

        public List<LibraryItem> GetAllItems() => _items;

        public List<LibraryItem> GetItemsByType(string type)
        {
            return _items.Where(i => i.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public LibraryItem? GetItemBySlug(string slug)
        {
            return _items.FirstOrDefault(i => i.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        }

        public List<LibraryItem> SearchItems(string query, string? type = null, string? category = null)
        {
            var result = _items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                result = result.Where(i => i.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                result = result.Where(i => i.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var cleanQuery = query.Trim().ToLowerInvariant();
                result = result.Where(i => 
                    i.Title.ToLowerInvariant().Contains(cleanQuery) ||
                    i.ShortDescription.ToLowerInvariant().Contains(cleanQuery) ||
                    i.Description.ToLowerInvariant().Contains(cleanQuery) ||
                    i.Tags.Any(t => t.ToLowerInvariant().Contains(cleanQuery))
                );
            }

            return result.ToList();
        }
    }
}
