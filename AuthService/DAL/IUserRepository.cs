using AuthService.Controllers.DTO;
using AuthService.Models;

namespace AuthService.DAL;

public interface IUserRepository
{
    public Task<User?> AuthenticateUserAsync(LoginUser user);
    public Task<User> AddUserAsync(CreateUser user);
    public Task<User?> FindUserByIdAsync(int id);
}
