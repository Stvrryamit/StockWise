using System.ComponentModel.DataAnnotations;

namespace StockWise.Models;

public class Sale
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string CustomerName { get; set; } = "Walk-in Customer";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string InvoiceNumber { get; set; } = "";
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
