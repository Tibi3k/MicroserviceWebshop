using AuthService.DAL.EfDbContext;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using AuthService.Controllers.DTO;

namespace AuthService.DAL;

public class UserRepository : IUserRepository   
{
    private readonly UserDbContext userDbContext;
    public UserRepository(UserDbContext db)
    {
        this.userDbContext = db;
    }

    public async Task<User?> FindUserByIdAsync(int id) {
        var user = await userDbContext.Users
            .SingleOrDefaultAsync(user => user.Id == id);
        return user?.ToUser();
    }

    //TODO send email on successful registration
    public async Task<User> AddUserAsync(CreateUser user) {
        Console.WriteLine("start");
        var a = userDbContext.Users.ToList();
        Console.WriteLine("length:"+ a.Count.ToString());

        //using (var tran = userDbContext.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
        //{
            var dbUser = new DbUser
            {
                Username = user.Username,
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
            Console.WriteLine("dbUser:" + dbUser.ToString() + dbUser.Password);
            dbUser = CreateHashedPassword(dbUser);
            Console.WriteLine("dbUser2:" + dbUser.ToString() + dbUser.Salt.ToString());
            await userDbContext.Users.AddAsync(dbUser);
        try
        {
            userDbContext.SaveChanges();
        }
        catch (Exception e) {
            Console.WriteLine("Message:" + e.Message + "  " + e.InnerException?.Message);
        }
            //await tran.CommitAsync();
            return dbUser.ToUser();
        //}
    }

    public async Task<User?> AuthenticateUserAsync(LoginUser user) {
        var dbUser = await userDbContext.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
        if (dbUser == null)
            return null;
        var hashedPassword = HashPassword(user.Password, dbUser.Salt);
        if (hashedPassword != dbUser.Password)
            return null;
        return dbUser.ToUser();
    }

    private string HashPassword(string password, byte[] salt) {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
            ));
    }

    private DbUser CreateHashedPassword(DbUser user) {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        user.Salt = saltBytes;
        user.Password = HashPassword(user.Password, saltBytes);
        return user;
    }


}
