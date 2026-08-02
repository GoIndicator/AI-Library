using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AILibrary.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserInputModel Input { get; set; } = new();

        public bool IsEditMode => Input.Id > 0;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id.HasValue)
            {
                var user = await _context.Users.FindAsync(id.Value);
                if (user == null)
                {
                    return NotFound();
                }

                Input = new UserInputModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    UserType = user.UserType
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Email) || !System.Text.RegularExpressions.Regex.IsMatch(Input.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ModelState.AddModelError("Input.Email", "O email deve conter um domínio válido (ex: usuario@dominio.com).");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Id > 0)
            {
                var user = await _context.Users.FindAsync(Input.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Username = Input.Username.Trim();
                user.Email = Input.Email.Trim().ToLowerInvariant();
                user.UserType = Input.UserType.Trim();

                _context.Users.Update(user);
                TempData["SuccessMessage"] = $"Usuário '{user.Username}' atualizado com sucesso!";
            }
            else
            {
                var user = new DatabaseUser
                {
                    Username = Input.Username.Trim(),
                    Email = Input.Email.Trim().ToLowerInvariant(),
                    UserType = Input.UserType.Trim()
                };

                _context.Users.Add(user);
                TempData["SuccessMessage"] = $"Usuário '{user.Username}' criado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        public class UserInputModel
        {
            public long Id { get; set; }

            [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
            [StringLength(100, ErrorMessage = "O username deve ter no máximo 100 caracteres.")]
            [Display(Name = "Nome de Usuário")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email deve conter um domínio válido (ex: usuario@dominio.com).")]
            [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres.")]
            [Display(Name = "Endereço de Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "O tipo do usuário é obrigatório.")]
            [StringLength(50, ErrorMessage = "O tipo deve ter no máximo 50 caracteres.")]
            [Display(Name = "Tipo de Usuário (ex: owner, admin, user, reader)")]
            public string UserType { get; set; } = "user";
        }
    }
}
