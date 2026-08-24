using CatalogApiMinimalApi.Domains;
using Microsoft.EntityFrameworkCore;

namespace CatalogApiMinimalApi.Context
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Name)
                                           .HasMaxLength(100)
                                           .IsRequired();
            modelBuilder.Entity<Category>().Property(c => c.Description)
                                           .HasMaxLength(200)
                                           .IsRequired();

            // Product
            modelBuilder.Entity<Product>().ToTable("products");
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(c => c.Name)
                                           .HasMaxLength(100)
                                           .IsRequired();
            modelBuilder.Entity<Product>().Property(c => c.Description)
                                           .HasMaxLength(200)
                                           .IsRequired();
            modelBuilder.Entity<Product>().Property(c => c.Price).HasPrecision(14, 2).IsRequired();

            // Relationship
            modelBuilder.Entity<Product>().HasOne<Category>(c => c.Category)
                                          .WithMany(p => p.Products)
                                          .HasForeignKey(c => c.Id);

        }
    }
}
