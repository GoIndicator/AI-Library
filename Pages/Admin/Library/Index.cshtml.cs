using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin.Library
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<LibraryItem> Items { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;

        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNum > 1;
        public bool HasNextPage => PageNum < TotalPages;

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var clean = SearchQuery.Trim().ToLower();
                query = query.Where(i => 
                    i.Title.ToLower().Contains(clean) || 
                    i.ItemCode.ToLower().Contains(clean) ||
                    i.Slug.ToLower().Contains(clean)
                );
            }

            if (PageNum < 1) PageNum = 1;

            var totalItems = await query.CountAsync();
            TotalPages = (int)System.Math.Ceiling(totalItems / 15.0);

            Items = await query
                .OrderByDescending(i => i.Id)
                .Skip((PageNum - 1) * 15)
                .Take(15)
                .ToListAsync();
        }
    }
}
