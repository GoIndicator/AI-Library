using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;

namespace AILibrary.Pages.Admin.Users
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DatabaseUser DatabaseUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            DatabaseUser = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Usuário '{user.Username}' excluído com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
