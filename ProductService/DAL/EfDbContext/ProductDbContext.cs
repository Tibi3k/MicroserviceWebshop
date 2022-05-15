using Microsoft.EntityFrameworkCore;

namespace ProductService.DAL.EfDbContext {

    public class ProductDbContext : DbContext {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) {
            Database.EnsureCreated();
            Database.SetCommandTimeout(5);
        }

        public DbSet<DbProduct> Products { get; set; }

        public DbSet<DbCategory> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbProduct>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<DbProduct>()
                .ToTable("Products");
            modelBuilder.Entity<DbProduct>()
                .Property(p => p.Name).HasMaxLength(100)
                .IsRequired(required: true).IsUnicode(unicode: true);
            modelBuilder.Entity<DbProduct>()
                .Property(p => p.Price).IsRequired(required: true);
            modelBuilder.Entity<DbProduct>()
                .Property(p => p.Description)
                .HasMaxLength(1000).IsUnicode(unicode: true);
            modelBuilder.Entity<DbProduct>()
                .Property(p => p.Quantity);
            modelBuilder.Entity<DbProduct>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products);

            modelBuilder.Entity<DbCategory>()
                .ToTable("Categories");
            modelBuilder.Entity<DbCategory>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<DbCategory>()
                .Property(c => c.Name).HasMaxLength(100)
                .IsRequired(required: true).IsUnicode(unicode: true);
        }
    }
    
}