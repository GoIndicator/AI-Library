using Microsoft.EntityFrameworkCore;
using AILibrary.Services;

namespace AILibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<LibraryItem> tblLibrary { get; set; }
        public DbSet<LibraryType> LibraryTypes { get; set; }
        public DbSet<LibraryCategory> LibraryCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure tblLibrary_type
            modelBuilder.Entity<LibraryType>(entity =>
            {
                entity.ToTable("tbllibrary_type");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idtype")
                      .UseIdentityAlwaysColumn()
                      .HasIdentityOptions(startValue: 1);
                entity.Property(e => e.Name).HasColumnName("type");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure tblLibrary_category
            modelBuilder.Entity<LibraryCategory>(entity =>
            {
                entity.ToTable("tbllibrary_category");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("idcategory")
                      .UseIdentityAlwaysColumn()
                      .HasIdentityOptions(startValue: 1);
                entity.Property(e => e.Name).HasColumnName("category");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure tblLibrary
            modelBuilder.Entity<LibraryItem>(entity =>
            {
                entity.ToTable("tblLibrary");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id")
                      .UseIdentityAlwaysColumn()
                      .HasIdentityOptions(startValue: 1);
                entity.Property(e => e.TypeId).HasColumnName("typeid");
                entity.Property(e => e.CategoryId).HasColumnName("categoryid");
                entity.HasIndex(e => e.Slug).IsUnique();

                // Relationships
                entity.HasOne(e => e.TypeNavigation)
                      .WithMany()
                      .HasForeignKey(e => e.TypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CategoryNavigation)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Map primitive collections
                entity.Property(e => e.Tags);
                entity.Property(e => e.MethodSteps);
            });
        }
    }
}
