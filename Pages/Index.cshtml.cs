using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Services;

namespace AILibrary.Pages
{
    public class IndexModel : PageModel
    {
        private readonly LibraryService _libraryService;

        public IndexModel(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public List<LibraryItem> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TypeFilter { get; set; }

        // Counts
        public int TotalCount { get; set; }
        public int FerramentasCount { get; set; } = 29;
        public int EdicoesCount { get; set; }
        public int SkillsCount { get; set; }
        public int CursosCount { get; set; } = 7;
        public int PluginsCount { get; set; }
        public int TutoriaisCount { get; set; } = 2;

        public void OnGet()
        {
            var allItems = _libraryService.GetAllItems();
            
            // Calculate dynamic counts
            SkillsCount = allItems.Count(i => i.Type.Equals("Skill", StringComparison.OrdinalIgnoreCase));
            PluginsCount = allItems.Count(i => i.Type.Equals("Plugin", StringComparison.OrdinalIgnoreCase));
            EdicoesCount = allItems.Count(i => i.Type.Equals("Edicao", StringComparison.OrdinalIgnoreCase));
            
            TotalCount = FerramentasCount + EdicoesCount + SkillsCount + CursosCount + PluginsCount + TutoriaisCount;

            // Filter items based on criteria
            if (!string.IsNullOrWhiteSpace(Q) || !string.IsNullOrWhiteSpace(TypeFilter))
            {
                string? searchType = null;
                if (!string.IsNullOrWhiteSpace(TypeFilter))
                {
                    if (TypeFilter.Equals("edicoes", StringComparison.OrdinalIgnoreCase))
                        searchType = "Edicao";
                    else if (TypeFilter.Equals("skills", StringComparison.OrdinalIgnoreCase))
                        searchType = "Skill";
                    else if (TypeFilter.Equals("plugins", StringComparison.OrdinalIgnoreCase))
                        searchType = "Plugin";
                }

                Items = _libraryService.SearchItems(Q ?? "", searchType);
            }
            else
            {
                // Default view (RECÉM ADICIONADO) shows recent editions
                Items = _libraryService.GetItemsByType("Edicao");
            }
        }
    }
}
