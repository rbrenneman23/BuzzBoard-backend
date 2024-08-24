using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_api.Migrations;
using backend_api.Models;
using Microsoft.IdentityModel.Tokens;
using bcrypt = BCrypt.Net.BCrypt;

namespace backend_api.Repositories;

public class UserService : IUserService
{
    private static PostDbContext _context;
    private static IConfiguration _config;

    public UserService(PostDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public User CreateUser(User user)
    {
        var passwordHash = bcrypt.HashPassword(user.Password);
        user.Password = passwordHash;

        _context.Add(user);
        _context.SaveChanges();
        return user;
    }

    public IEnumerable<Post>? GetPostByUserId(int userId)
    {
        throw new NotImplementedException();
    }

    public User GetUserProfile(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return null;
        }

        var userProfile = new User
        {
            UserId = user.UserId,
            UserName = user.UserName
        };

        return userProfile;
    }

    public string SignIn(string emailOrUsername, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == emailOrUsername || u.UserName == emailOrUsername);
        var verified = false;

        if (user != null)
        {
            verified = bcrypt.Verify(password, user.Password);
        }

        if (user == null || !verified)
        {
            return String.Empty;
        }

        return BuildToken(user);
    }

    private string BuildToken(User user)
    {
        var secret = _config.GetValue<String>("TokenSecret");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new Claim("Post_UserId", user.UserId.ToString()),
            new Claim("firstName", user.FirstName ?? ""),
            new Claim("lastName", user.LastName ?? ""),
        };

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: signingCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }
}