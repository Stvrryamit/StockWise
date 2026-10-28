using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockWise.Data;
using StockWise.Models;

namespace StockWise.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Supplier).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            HttpContext.Session.SetString("ProductSearch", search.Trim());
            query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
        }
        else if (HttpContext.Session.TryGetValue("ProductSearch", out _))
        {
            HttpContext.Session.Remove("ProductSearch");
        }
        ViewBag.Search = search;
        return View(await query.OrderBy(p => p.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var product = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : View(product);
    }

    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View(new Product());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (await _db.Products.AnyAsync(p => p.SKU == product.SKU))
            ModelState.AddModelError(nameof(product.SKU), "This SKU already exists.");
        if (ModelState.IsValid)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product created.";
            return RedirectToAction(nameof(Index));
        }
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();
        if (await _db.Products.AnyAsync(p => p.SKU == product.SKU && p.Id != id))
            ModelState.AddModelError(nameof(product.SKU), "This SKU is already used by another product.");
        if (ModelState.IsValid)
        {
            _db.Update(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product updated.";
            return RedirectToAction(nameof(Index));
        }
        await LoadLists(product.CategoryId, product.SupplierId);
        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        if (await _db.SaleItems.AnyAsync(i => i.ProductId == id))
        {
            TempData["Error"] = "This product is part of a sale and cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLists(int? categoryId = null, int? supplierId = null)
    {
        ViewBag.CategoryId = new SelectList(await _db.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId);
        ViewBag.SupplierId = new SelectList(await _db.Suppliers.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", supplierId);
    }
}
