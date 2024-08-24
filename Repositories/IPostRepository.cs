using backend_api.Models;

namespace backend_api.Repositories;

public interface IPostRepository
{
    IEnumerable<Post> GetAllPosts();
    IEnumerable<Post> GetPostsByUserId(int userId);
    Post? GetPostById(int postId);
    Post? CreatePost(Post newPost);
    Post? EditPost(Post newPost);
    void DeletePostById(int postId);
}