using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin.Types
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<LibraryType> Types { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Types = await _context.LibraryTypes
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var type = await _context.LibraryTypes.FindAsync(id);
            if (type == null)
            {
                ErrorMessage = "Tipo não encontrado.";
                return RedirectToPage();
            }

            // Check if there are any associated library items
            var hasAssociatedItems = await _context.tblLibrary.AnyAsync(i => i.TypeId == id);
            if (hasAssociatedItems)
            {
                ErrorMessage = $"Não é possível excluir o tipo '{type.Name}' pois existem itens na biblioteca vinculados a ele.";
                return RedirectToPage();
            }

            _context.LibraryTypes.Remove(type);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Tipo '{type.Name}' excluído com sucesso!";
            return RedirectToPage();
        }
    }
}
