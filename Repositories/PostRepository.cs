using backend_api.Migrations;
using backend_api.Models;
using SQLitePCL;

namespace backend_api.Repositories;

public class PostRepository : IPostRepository
{
    private readonly PostDbContext _context;
    public PostRepository(PostDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Post> GetAllPosts()
    {
        return _context.Posts.ToList();
    }

    public IEnumerable<Post> GetPostsByUserId(int userId)
    {
        return _context.Posts.Where(c => c.UserId == userId).ToList();
    }

    public Post? GetPostById(int postId)
    {
        return _context.Posts.SingleOrDefault(c => c.PostId == postId);
    }

    public Post CreatePost(Post newPost)
    {
        _context.Posts.Add(newPost);
        _context.SaveChanges();
        return newPost;
    }

    public Post? EditPost(Post newPost)
    {
        var originalPost = _context.Posts.Find(newPost.PostId);
        if (originalPost != null) 
        {
            originalPost.Status = newPost.Status;
            _context.SaveChanges();
        }
        return originalPost;
    }

    public void DeletePostById(int postId)
    {
        var post = _context.Posts.Find(postId);
        if (post != null)
        {
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }
    }
}