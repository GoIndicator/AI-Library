using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AILibrary.Data;
using AILibrary.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;

namespace AILibrary.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Insira um e-mail válido com domínio (ex: voce@email.com).")]
        public string Email { get; set; } = string.Empty;

        public IActionResult OnGet(string? handler)
        {
            if (handler == "Logout")
            {
                Response.Cookies.Delete("UserEmail");
                return RedirectToPage("/Login");
            }

            // If already logged in, redirect to Index
            if (Request.Cookies.TryGetValue("UserEmail", out var emailVal) && !string.IsNullOrWhiteSpace(emailVal))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Strict server-side regex validation check
            if (string.IsNullOrWhiteSpace(Email) || !System.Text.RegularExpressions.Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ModelState.AddModelError("Email", "O email deve conter um domínio válido (ex: usuario@dominio.com).");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cleanEmail = Email.Trim().ToLowerInvariant();
            var cleanUsername = Username.Trim();

            // Check if user exists in tblusers
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail);
            if (user == null)
            {
                // Create user with provided Username
                user = new DatabaseUser
                {
                    Username = cleanUsername,
                    Email = cleanEmail,
                    UserType = "reader"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Set cookie for 30 days
            Response.Cookies.Append("UserEmail", user.Email, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToPage("/Index");
        }
    }
}
