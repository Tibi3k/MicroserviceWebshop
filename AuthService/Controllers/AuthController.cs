using AuthService.Controllers.DTO;
using AuthService.DAL;
using AuthService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers;
[ApiController]
[Route("/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration configuration;
    private readonly IUserRepository repository;

    public AuthController(
        ILogger<AuthController> logger, IConfiguration configuration, IUserRepository repository)
    {
        _logger = logger;
        this.configuration = configuration;
        this.repository = repository;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult Get([FromHeader] string Authorization)
    {
        var token = Authorization.Split(" ").Last();
        var userId = ValidateToken(token);
        if (userId == null || userId == "")
            return Unauthorized();
        //Console.WriteLine("get");
        //IEnumerable<string> keyValues = HttpContext.Request.Headers.Keys.Select(key => key + ": " + string.Join(",", HttpContext.Request.Headers[key]));
        //string requestHeaders = string.Join(System.Environment.NewLine, keyValues);
        //Console.WriteLine("Headers:" + requestHeaders);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<User>> Login([FromBody] LoginUser user) {
        var authUser = await repository.AuthenticateUserAsync(user);
        if (authUser == null)
            return Unauthorized();
        var token = GenerateJSONWebToken(authUser);
        HttpContext.Response.Headers.Authorization = "Bearer: " + token;
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<User>> Register([FromBody] CreateUser user) {
        var addedUser = await repository.AddUserAsync(user);
        return Ok(addedUser);
    }

    private string GenerateJSONWebToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { 
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("email", user.EmailAddress)
            }),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "id").Value;
            if (userId == null || userId == "")
                return null;
            return userId;
        }
        catch
        {
            return null;
        }
    }
}
