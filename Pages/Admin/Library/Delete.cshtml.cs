using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin.Library
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LibraryItem Item { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var item = await _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            Item = item;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var item = await _context.tblLibrary.FindAsync(id);
            if (item != null)
            {
                _context.tblLibrary.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Recurso '{item.Title}' excluído com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
