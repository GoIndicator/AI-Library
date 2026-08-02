using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AILibrary.Pages.Admin.Categories
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CategoryInputModel Input { get; set; } = new();

        public bool IsEditMode => Input.Id > 0;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id.HasValue)
            {
                var category = await _context.LibraryCategories.FindAsync(id.Value);
                if (category == null)
                {
                    return NotFound();
                }

                Input = new CategoryInputModel
                {
                    Id = category.Id,
                    Name = category.Name
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Id > 0)
            {
                var category = await _context.LibraryCategories.FindAsync(Input.Id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = Input.Name.Trim();
                _context.LibraryCategories.Update(category);
                TempData["SuccessMessage"] = $"Categoria '{category.Name}' atualizada com sucesso!";
            }
            else
            {
                var category = new LibraryCategory
                {
                    Name = Input.Name.Trim()
                };
                _context.LibraryCategories.Add(category);
                TempData["SuccessMessage"] = $"Categoria '{category.Name}' criada com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public class CategoryInputModel
        {
            public long Id { get; set; }

            [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
            [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
            [Display(Name = "Nome da Categoria")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
