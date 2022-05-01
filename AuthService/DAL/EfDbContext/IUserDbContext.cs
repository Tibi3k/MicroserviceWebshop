using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AuthService.DAL.EfDbContext;

public interface IUserDbContext
{
    DbSet<DbUser> Users { get; }
    public DatabaseFacade SqlDatabase { get; }
    Task<int> SaveChangesAsync();
}
