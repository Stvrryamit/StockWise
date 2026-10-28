using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWise.Data;
using System.Text;

namespace StockWise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var start = (from ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
            var end = (to ?? DateTime.UtcNow.Date).Date.AddDays(1);

            if (end <= start)
            {
                ModelState.AddModelError("", "End date must be on or after start date.");
                end = start.AddDays(1);
            }

            ViewBag.From = start.ToString("yyyy-MM-dd");
            ViewBag.To = end.AddDays(-1).ToString("yyyy-MM-dd");

            ViewBag.Sales = await _db.Sales
                .Where(s => s.CreatedAtUtc >= start && s.CreatedAtUtc < end)
                .OrderByDescending(s => s.CreatedAtUtc)
                .ToListAsync();

            ViewBag.StockValue = await _db.Products
                .SumAsync(p => (decimal?)(p.UnitPrice * p.Quantity)) ?? 0m;

            return View();
        }

        public async Task<IActionResult> Csv(DateTime? from, DateTime? to)
        {
            var start = (from ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
            var end = (to ?? DateTime.UtcNow.Date).Date.AddDays(1);

            if (end <= start)
            {
                return BadRequest("End date must be on or after start date.");
            }

            var rows = await _db.Sales
                .Where(s => s.CreatedAtUtc >= start && s.CreatedAtUtc < end)
                .OrderByDescending(s => s.CreatedAtUtc)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Invoice,Customer,Date,Subtotal,Discount,Tax,Total");

            foreach (var sale in rows)
            {
                sb.AppendLine(
                    $"{Escape(sale.InvoiceNumber)},{Escape(sale.CustomerName)}," +
                    $"{sale.CreatedAtUtc:yyyy-MM-dd},{sale.Subtotal:F2}," +
                    $"{sale.DiscountAmount:F2},{sale.TaxAmount:F2},{sale.TotalAmount:F2}");
            }

            return File(
                Encoding.UTF8.GetBytes(sb.ToString()),
                "text/csv",
                "stockwise-sales-report.csv");
        }

        private static string Escape(string? value)
        {
            value ??= string.Empty;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
