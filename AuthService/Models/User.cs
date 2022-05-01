using AuthService.DAL.EfDbContext;

namespace AuthService.Models;

public class User
{
    public User(int id, string username, string emailAddress, string? password = null)
    {
        Id = id;
        Username = username;
        EmailAddress = emailAddress;
        Password = password;
    }

    public int Id { get; set; }
    public string Username { get; set; }
    public string EmailAddress { get; set; }
    public string? Password { get; set; }

    public DbUser ToDbUser() {
        return new DbUser(Id, Username, EmailAddress, Password);
    }
}
