using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AuthService.DAL.EfDbContext;


public class UserDbContext : DbContext, IUserDbContext {
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    { }

    public DbSet<DbUser> Users { get; set; }

    public DatabaseFacade SqlDatabase { get => this.Database; }

    public async Task<int> SaveChangesAsync()
    {
        return await this.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbUser>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<DbUser>()
            .ToTable("Users");
        modelBuilder.Entity<DbUser>()
            .Property(p => p.Username).HasMaxLength(40)
            .IsRequired(required: true).IsUnicode(unicode: true);
        modelBuilder.Entity<DbUser>()
            .Property(p => p.EmailAddress)
            .IsRequired(required: true).HasMaxLength(100);
        modelBuilder.Entity<DbUser>()
            .Property(p => p.Password).IsRequired(required: true)
            .HasMaxLength(1000).IsUnicode(unicode: true);
        modelBuilder.Entity<DbUser>()
            .Property(p => p.Salt)
            .HasMaxLength(1000);
    }
}
