using System.ComponentModel.DataAnnotations;

namespace StockWise.Models;

public class Supplier
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = "";
    [Phone, StringLength(25)]
    public string? Phone { get; set; }
    [EmailAddress, StringLength(120)]
    public string? Email { get; set; }
    [StringLength(250)]
    public string? Address { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
