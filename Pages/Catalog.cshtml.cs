using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Services;

namespace AILibrary.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly LibraryService _libraryService;

        public CatalogModel(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public string CatalogType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string SearchPlaceholder { get; set; } = string.Empty;

        public List<LibraryItem> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        public List<string> Categories { get; set; } = new();

        public void OnGet(string catalogType)
        {
            CatalogType = catalogType ?? "ferramentas";
            
            // Map types & meta
            string serviceType;
            switch (CatalogType.ToLowerInvariant())
            {
                case "ferramentas":
                    serviceType = "Ferramenta";
                    DisplayName = "Ferramentas";
                    Subtitle = "Projetos e aplicativos de IA para testar, instalar e usar no dia a dia.";
                    SearchPlaceholder = "Buscar em ferramentas...";
                    break;
                case "tutoriais":
                    serviceType = "Tutorial";
                    DisplayName = "Tutoriais";
                    Subtitle = "Guias passo a passo e tutoriais práticos de inteligência artificial.";
                    SearchPlaceholder = "Buscar em tutoriais...";
                    break;
                case "cursos":
                    serviceType = "Curso";
                    DisplayName = "Cursos Gratuitos";
                    Subtitle = "Cursos completos gratuitos para dominar engenharia de prompts e agentes.";
                    SearchPlaceholder = "Buscar em cursos...";
                    break;

                default:
                    serviceType = "Ferramenta";
                    DisplayName = "Recursos";
                    Subtitle = "Recursos adicionais de IA.";
                    SearchPlaceholder = "Buscar...";
                    break;
            }

            var allItemsOfType = _libraryService.GetItemsByType(serviceType);
            
            // Extract distinct categories
            Categories = allItemsOfType
                .Select(i => i.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            // Run search query
            Items = _libraryService.SearchItems(Q ?? "", serviceType, Category);
        }
    }
}
