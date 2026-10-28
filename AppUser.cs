using System.ComponentModel.DataAnnotations;
namespace StockWise.Models;
public class AppUser { public int Id { get; set; } [Required, StringLength(60)] public string Username { get; set; } = ""; [Required] public string PasswordHash { get; set; } = ""; [Required, StringLength(20)] public string Role { get; set; } = "Staff"; public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; }
