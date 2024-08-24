using System.Security.Claims;
using backend_api.Models;
using backend_api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostRepository _postRepository;

    public PostController(ILogger<PostController> logger, IPostRepository repository)
    {
        _logger = logger;
        _postRepository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Post>> GetPosts()
    {
        return Ok(_postRepository.GetAllPosts());
    }

    [HttpGet]
    [Route("{postId:int}")]
    public ActionResult<Post> GetPostById(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpGet]
    [Route("user/{userId:int}")]
    public ActionResult<IEnumerable<Post>> GetPostsByCurrentUser(int userId)
    {
        var userPosts = _postRepository.GetPostsByUserId(userId);
        return Ok(userPosts);
    }

    [HttpPost]
    [Authorize]
    public ActionResult<Post> CreatePost(Post post)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Post_UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid user");
        }

        post.UserId = userId;

        if (!ModelState.IsValid || post == null)
        {
            return BadRequest();
        }

        post.UserName = User.Identity.Name;
        post.CreatedAt = DateTime.UtcNow;

        _postRepository.CreatePost(post);

        return Created(nameof(GetPostById), post);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        throw new InvalidOperationException("User ID not found in claims.");
    }

    [HttpPut]
    [Route("{postId:int}")]
    [Authorize]
    public ActionResult<Post> EditPost(int postId, Post updatedPost)
    {
        if (!ModelState.IsValid || updatedPost == null)
        {
            return BadRequest();
        }

        var existingPost = _postRepository.GetPostById(postId);
        if (existingPost == null)
        {
            return NotFound();
        }

        if (existingPost.UserName != User.Identity.Name)
        {
            return Forbid();
        }

        if (existingPost.Status != updatedPost.Status)
        {
            existingPost.Status = updatedPost.Status;
            existingPost.IsEdited = true;
        }
        else
        {
            existingPost.Status = updatedPost.Status;
        }

        _postRepository.EditPost(existingPost);

        return Ok(existingPost);
    }

    [HttpDelete]
    [Route("{postId:int}")]
    [Authorize]
    public ActionResult DeletePost(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post.UserName != User.Identity.Name)
        {
            return Forbid();
        }

        _postRepository.DeletePostById(postId);
        return NoContent();
    }
}
