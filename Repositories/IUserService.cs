using backend_api.Models;

namespace backend_api.Repositories;

public interface IUserService
{
    User CreateUser(User user);
    User GetUserProfile(int userId);
    string SignIn(string email, string password);
    IEnumerable<Post>? GetPostByUserId(int userId);
}