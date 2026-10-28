using System.ComponentModel.DataAnnotations;

namespace StockWise.Models;

public class Category
{
    public int Id { get; set; }
    [Required, StringLength(80)]
    public string Name { get; set; } = "";
    [StringLength(250)]
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
