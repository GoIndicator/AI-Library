using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AILibrary.Pages.Admin.Types
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TypeInputModel Input { get; set; } = new();

        public bool IsEditMode => Input.Id > 0;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id.HasValue)
            {
                var type = await _context.LibraryTypes.FindAsync(id.Value);
                if (type == null)
                {
                    return NotFound();
                }

                Input = new TypeInputModel
                {
                    Id = type.Id,
                    Name = type.Name
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
                var type = await _context.LibraryTypes.FindAsync(Input.Id);
                if (type == null)
                {
                    return NotFound();
                }

                type.Name = Input.Name.Trim();
                _context.LibraryTypes.Update(type);
                TempData["SuccessMessage"] = $"Tipo '{type.Name}' atualizado com sucesso!";
            }
            else
            {
                var type = new LibraryType
                {
                    Name = Input.Name.Trim()
                };
                _context.LibraryTypes.Add(type);
                TempData["SuccessMessage"] = $"Tipo '{type.Name}' criado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public class TypeInputModel
        {
            public long Id { get; set; }

            [Required(ErrorMessage = "O nome do tipo é obrigatório.")]
            [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
            [Display(Name = "Nome do Tipo")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
