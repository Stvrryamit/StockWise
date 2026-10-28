using Microsoft.EntityFrameworkCore;
using StockWise.Models;

namespace StockWise.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();
        modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Sale>().HasIndex(s => s.InvoiceNumber).IsUnique();
        modelBuilder.Entity<Product>().Property(p => p.UnitPrice).HasPrecision(10, 2);
        modelBuilder.Entity<Sale>().Property(s => s.TotalAmount).HasPrecision(12, 2);
        modelBuilder.Entity<SaleItem>().Property(s => s.UnitPrice).HasPrecision(10, 2);
        modelBuilder.Entity<SaleItem>().Property(s => s.LineTotal).HasPrecision(12, 2);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category).WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier).WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<SaleItem>()
            .HasOne(i => i.Product).WithMany(p => p.SaleItems)
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
