using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Services;

namespace AILibrary.Pages.Catalog
{
    public class DetailModel : PageModel
    {
        private readonly LibraryService _libraryService;

        public DetailModel(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public string CatalogType { get; set; } = string.Empty;
        public string CatalogDisplayName { get; set; } = string.Empty;
        public LibraryItem Item { get; set; } = new();

        public IActionResult OnGet(string catalogType, string slug)
        {
            CatalogType = catalogType ?? "ferramentas";
            CatalogDisplayName = CatalogType.ToLowerInvariant() switch
            {
                "ferramentas" => "Ferramentas",
                "tutoriais" => "Tutoriais",
                "cursos" => "Cursos Gratuitos",
                _ => "Recursos"
            };

            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToPage("/Catalog", new { catalogType = CatalogType });
            }

            var item = _libraryService.GetItemBySlug(slug);
            if (item == null)
            {
                return NotFound();
            }

            Item = item;
            return Page();
        }
    }
}
