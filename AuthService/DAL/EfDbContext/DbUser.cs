using AuthService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.DAL.EfDbContext;

public class DbUser
{
    public DbUser() { }
    public DbUser(int id, string username, string emailAddress, string password)
    {
        Id = id;
        Username = username;
        EmailAddress = emailAddress;
        Password = password;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Username { get; set; }
    public string EmailAddress { get; set; }
    public string Password { get; set; }
    public byte[] Salt { get; set; }

    public override string? ToString()
    {
        return
            "Username:" + Username.ToString() + "\n" +
            "EmailAddress:" + EmailAddress.ToString() + "\n" +
            "Password:" + Password.ToString() + "\n";
    }

    public User ToUser()
    {
        return new User(Id, Username, EmailAddress, "");
    }
}
