using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Services;

namespace AILibrary.Pages.Skills
{
    public class DetailModel : PageModel
    {
        private readonly LibraryService _libraryService;

        public DetailModel(LibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public LibraryItem Item { get; set; } = new();

        public IActionResult OnGet(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToPage("/Skills");
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
