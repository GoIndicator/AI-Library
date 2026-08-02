using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<LibraryCategory> Categories { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _context.LibraryCategories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var category = await _context.LibraryCategories.FindAsync(id);
            if (category == null)
            {
                ErrorMessage = "Categoria não encontrada.";
                return RedirectToPage();
            }

            // Check if there are any associated library items
            var hasAssociatedItems = await _context.tblLibrary.AnyAsync(i => i.CategoryId == id);
            if (hasAssociatedItems)
            {
                ErrorMessage = $"Não é possível excluir a categoria '{category.Name}' pois existem itens na biblioteca vinculados a ela.";
                return RedirectToPage();
            }

            _context.LibraryCategories.Remove(category);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Categoria '{category.Name}' excluída com sucesso!";
            return RedirectToPage();
        }
    }
}
