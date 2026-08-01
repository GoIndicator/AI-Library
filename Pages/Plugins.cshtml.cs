using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Services;

namespace AILibrary.Pages
{
    public class PluginsModel : PageModel
    {
        private readonly LibraryService _libraryService;

        public PluginsModel(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public List<LibraryItem> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        public List<string> Categories { get; set; } = new();

        public void OnGet()
        {
            var allPlugins = _libraryService.GetItemsByType("Plugin");
            
            // Extract distinct categories
            Categories = allPlugins
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            // Run search query
            Items = _libraryService.SearchItems(Q ?? "", "Plugin", Category);
        }
    }
}
