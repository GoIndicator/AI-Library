using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AILibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace AILibrary.Services
{
    public class LibraryType
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LibraryCategory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LibraryItem
    {
        public long Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public long TypeId { get; set; }
        public LibraryType TypeNavigation { get; set; } = null!;

        public long CategoryId { get; set; }
        public LibraryCategory CategoryNavigation { get; set; } = null!;

        // Expose string properties to preserve existing page rendering code without any modifications!
        [NotMapped]
        public string Type 
        { 
            get => TypeNavigation?.Name ?? string.Empty;
            set { }
        }

        [NotMapped]
        public string Category 
        { 
            get => CategoryNavigation?.Name ?? string.Empty;
            set { }
        }

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

    public class LibraryService
    {
        private readonly AppDbContext _context;

        public LibraryService(AppDbContext context)
        {
            _context = context;
        }

        public List<LibraryItem> GetAllItems()
        {
            return _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .ToList();
        }

        public List<LibraryItem> GetItemsByType(string type)
        {
            return _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .Where(i => i.TypeNavigation.Name.ToLower() == type.ToLower())
                .ToList();
        }

        public LibraryItem? GetItemBySlug(string slug)
        {
            return _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .FirstOrDefault(i => i.Slug.ToLower() == slug.ToLower());
        }

        public List<LibraryItem> GetRecentItems(int count)
        {
            return _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .Take(count)
                .ToList();
        }

        public List<LibraryItem> SearchItems(string query, string? type = null, string? category = null)
        {
            var result = _context.tblLibrary
                .Include(i => i.TypeNavigation)
                .Include(i => i.CategoryNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(type))
            {
                result = result.Where(i => i.TypeNavigation.Name.ToLower() == type.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                result = result.Where(i => i.CategoryNavigation.Name.ToLower() == category.ToLower());
            }

            var items = result.ToList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var cleanQuery = query.Trim().ToLowerInvariant();
                items = items.Where(i => 
                    i.Title.ToLowerInvariant().Contains(cleanQuery) ||
                    i.ShortDescription.ToLowerInvariant().Contains(cleanQuery) ||
                    i.Description.ToLowerInvariant().Contains(cleanQuery) ||
                    i.Tags.Any(t => t.ToLowerInvariant().Contains(cleanQuery))
                ).ToList();
            }

            return items;
        }
    }
}
