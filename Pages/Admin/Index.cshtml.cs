using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int LibraryCount { get; set; }
        public int TypesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int UsersCount { get; set; }

        public async Task OnGetAsync()
        {
            LibraryCount = await _context.tblLibrary.CountAsync();
            TypesCount = await _context.LibraryTypes.CountAsync();
            CategoriesCount = await _context.LibraryCategories.CountAsync();
            UsersCount = await _context.Users.CountAsync();
        }
    }
}
