using System.ComponentModel.DataAnnotations;

namespace StockWise.Models;

public class Product
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string Name { get; set; } = "";
    [Required, StringLength(40)]
    public string SKU { get; set; } = "";
    [Range(0.01, 99999999)]
    public decimal UnitPrice { get; set; }
    [Range(0, 1000000)]
    public int Quantity { get; set; }
    [Range(0, 1000000)]
    public int ReorderLevel { get; set; } = 5;
    [Required]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
