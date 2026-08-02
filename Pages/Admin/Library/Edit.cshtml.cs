using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AILibrary.Data;
using AILibrary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Pages.Admin.Library
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LibraryItemInputModel Input { get; set; } = new();

        public List<SelectListItem> TypesList { get; set; } = new();
        public List<SelectListItem> CategoriesList { get; set; } = new();

        public bool IsEditMode => Input.Id > 0;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            await LoadSelectListsAsync();

            if (id.HasValue)
            {
                var item = await _context.tblLibrary.FindAsync(id.Value);
                if (item == null)
                {
                    return NotFound();
                }

                Input = new LibraryItemInputModel
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    Slug = item.Slug,
                    Title = item.Title,
                    TypeId = item.TypeId,
                    CategoryId = item.CategoryId,
                    ShortDescription = item.ShortDescription,
                    Description = item.Description,
                    IsNew = item.IsNew,
                    TagsInput = string.Join(", ", item.Tags),
                    OfficialLink = item.OfficialLink,
                    MethodStepsInput = string.Join(Environment.NewLine, item.MethodSteps),
                    Prompt = item.Prompt,
                    SourceContentTitle = item.SourceContentTitle,
                    SourceContentLink = item.SourceContentLink
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            var tags = string.IsNullOrWhiteSpace(Input.TagsInput)
                ? new List<string>()
                : Input.TagsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

            var steps = string.IsNullOrWhiteSpace(Input.MethodStepsInput)
                ? new List<string>()
                : Input.MethodStepsInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

            if (Input.Id > 0)
            {
                var item = await _context.tblLibrary.FindAsync(Input.Id);
                if (item == null)
                {
                    return NotFound();
                }

                item.ItemCode = Input.ItemCode.Trim();
                item.Slug = Input.Slug.Trim().ToLowerInvariant();
                item.Title = Input.Title.Trim();
                item.TypeId = Input.TypeId;
                item.CategoryId = Input.CategoryId;
                item.ShortDescription = Input.ShortDescription.Trim();
                item.Description = Input.Description.Trim();
                item.IsNew = Input.IsNew;
                item.Tags = tags;
                item.OfficialLink = Input.OfficialLink?.Trim() ?? string.Empty;
                item.MethodSteps = steps;
                item.Prompt = Input.Prompt?.Trim() ?? string.Empty;
                item.SourceContentTitle = Input.SourceContentTitle?.Trim() ?? string.Empty;
                item.SourceContentLink = Input.SourceContentLink?.Trim() ?? string.Empty;

                _context.tblLibrary.Update(item);
                TempData["SuccessMessage"] = $"Recurso '{item.Title}' atualizado com sucesso!";
            }
            else
            {
                var item = new LibraryItem
                {
                    ItemCode = Input.ItemCode.Trim(),
                    Slug = Input.Slug.Trim().ToLowerInvariant(),
                    Title = Input.Title.Trim(),
                    TypeId = Input.TypeId,
                    CategoryId = Input.CategoryId,
                    ShortDescription = Input.ShortDescription.Trim(),
                    Description = Input.Description.Trim(),
                    IsNew = Input.IsNew,
                    Tags = tags,
                    OfficialLink = Input.OfficialLink?.Trim() ?? string.Empty,
                    MethodSteps = steps,
                    Prompt = Input.Prompt?.Trim() ?? string.Empty,
                    SourceContentTitle = Input.SourceContentTitle?.Trim() ?? string.Empty,
                    SourceContentLink = Input.SourceContentLink?.Trim() ?? string.Empty
                };

                _context.tblLibrary.Add(item);
                TempData["SuccessMessage"] = $"Recurso '{item.Title}' criado com sucesso!";
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Input.Slug", "Já existe um recurso com este Slug. O Slug deve ser único.");
                await LoadSelectListsAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadSelectListsAsync()
        {
            var types = await _context.LibraryTypes.OrderBy(t => t.Name).ToListAsync();
            TypesList = types.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            }).ToList();

            var categories = await _context.LibraryCategories.OrderBy(c => c.Name).ToListAsync();
            CategoriesList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }

        public class LibraryItemInputModel
        {
            public long Id { get; set; }

            [Required(ErrorMessage = "O código do item (JSON) é obrigatório.")]
            [StringLength(100, ErrorMessage = "O código deve ter no máximo 100 caracteres.")]
            [Display(Name = "Código Identificador (ItemCode)")]
            public string ItemCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "O Slug é obrigatório para as URLs.")]
            [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "O slug deve conter apenas letras minúsculas, números e hifens.")]
            [StringLength(100, ErrorMessage = "O slug deve ter no máximo 100 caracteres.")]
            [Display(Name = "URL Slug (ex: notion-mcp)")]
            public string Slug { get; set; } = string.Empty;

            [Required(ErrorMessage = "O título do recurso é obrigatório.")]
            [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
            [Display(Name = "Título")]
            public string Title { get; set; } = string.Empty;

            [Required(ErrorMessage = "Selecione o tipo do recurso.")]
            [Display(Name = "Tipo")]
            public long TypeId { get; set; }

            [Required(ErrorMessage = "Selecione a categoria do recurso.")]
            [Display(Name = "Categoria")]
            public long CategoryId { get; set; }

            [Required(ErrorMessage = "A descrição curta é obrigatória.")]
            [StringLength(300, ErrorMessage = "A descrição curta deve ter no máximo 300 caracteres.")]
            [Display(Name = "Descrição Curta (Resumo do Card)")]
            public string ShortDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "A descrição detalhada é obrigatória.")]
            [Display(Name = "Descrição Detalhada (Markdown)")]
            public string Description { get; set; } = string.Empty;

            [Display(Name = "Marcar como Recurso Novo?")]
            public bool IsNew { get; set; }

            [Display(Name = "Tags (Separadas por vírgula)")]
            public string TagsInput { get; set; } = string.Empty;

            [Display(Name = "Link Oficial (URL)")]
            public string? OfficialLink { get; set; }

            [Display(Name = "Passos do Método (Um por linha)")]
            public string? MethodStepsInput { get; set; }

            [Display(Name = "Pronto Prompt / Código de Integração")]
            public string? Prompt { get; set; }

            [Display(Name = "Título da Fonte de Origem")]
            public string? SourceContentTitle { get; set; }

            [Display(Name = "Link da Fonte de Origem (URL)")]
            public string? SourceContentLink { get; set; }
        }
    }
}
