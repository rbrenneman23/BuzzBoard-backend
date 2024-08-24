using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend_api.Models;

public class User
{
    [JsonIgnore]
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}