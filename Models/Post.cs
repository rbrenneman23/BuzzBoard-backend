using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend_api.Models;

public class Post
{
    public int PostId { get; set; }

    [Required]
    public string? Status { get; set; }

    public string? UserName { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsEdited { get; set; }

    public int UserId { get; set; }
}