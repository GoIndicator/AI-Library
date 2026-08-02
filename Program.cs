using AILibrary.Data;
using AILibrary.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure EF Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("developerCN")));

// Register LibraryService as Scoped
builder.Services.AddScoped<LibraryService>();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

// Ensure Database is Created & Seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Force recreate database with correct int8 identity id column and lowercase tables mappings
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed if empty
        if (!context.tblLibrary.Any())
        {
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var jsonPath = Path.Combine(env.WebRootPath, "data", "library.json");
            if (File.Exists(jsonPath))
            {
                var jsonContent = File.ReadAllText(jsonPath);
                var jsonItems = JsonSerializer.Deserialize<List<JsonLibraryItem>>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (jsonItems != null)
                {
                    var typeMap = new Dictionary<string, LibraryType>(StringComparer.OrdinalIgnoreCase);
                    var categoryMap = new Dictionary<string, LibraryCategory>(StringComparer.OrdinalIgnoreCase);

                    foreach (var jsonItem in jsonItems)
                    {
                        var typeName = string.IsNullOrWhiteSpace(jsonItem.Type) ? "Recurso" : jsonItem.Type.Trim();
                        var categoryName = string.IsNullOrWhiteSpace(jsonItem.Category) ? "Geral" : jsonItem.Category.Trim();

                        if (!typeMap.TryGetValue(typeName, out var libType))
                        {
                            libType = new LibraryType { Name = typeName };
                            context.LibraryTypes.Add(libType);
                            context.SaveChanges();
                            typeMap[typeName] = libType;
                        }

                        if (!categoryMap.TryGetValue(categoryName, out var libCategory))
                        {
                            libCategory = new LibraryCategory { Name = categoryName };
                            context.LibraryCategories.Add(libCategory);
                            context.SaveChanges();
                            categoryMap[categoryName] = libCategory;
                        }

                        var dbItem = new LibraryItem
                        {
                            ItemCode = jsonItem.Id,
                            Slug = jsonItem.Slug,
                            Title = jsonItem.Title,
                            TypeId = libType.Id,
                            CategoryId = libCategory.Id,
                            ShortDescription = jsonItem.ShortDescription,
                            Description = jsonItem.Description,
                            IsNew = jsonItem.IsNew,
                            Tags = jsonItem.Tags,
                            OfficialLink = jsonItem.OfficialLink,
                            MethodSteps = jsonItem.MethodSteps,
                            Prompt = jsonItem.Prompt,
                            SourceContentTitle = jsonItem.SourceContentTitle,
                            SourceContentLink = jsonItem.SourceContentLink
                        };

                        context.tblLibrary.Add(dbItem);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing or seeding database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public class JsonLibraryItem
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public List<string> Tags { get; set; } = new();
    public string OfficialLink { get; set; } = string.Empty;
    public List<string> MethodSteps { get; set; } = new();
    public string Prompt { get; set; } = string.Empty;
    public string SourceContentTitle { get; set; } = string.Empty;
    public string SourceContentLink { get; set; } = string.Empty;
}
