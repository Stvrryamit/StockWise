using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWise.Data;
using StockWise.Models;

namespace StockWise.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;
    public async Task<IActionResult> Index() => View(await _db.Categories.Include(c => c.Products).OrderBy(c => c.Name).ToListAsync());
    public IActionResult Create() => View(new Category());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (await _db.Categories.AnyAsync(c => c.Name == category.Name)) ModelState.AddModelError(nameof(category.Name), "Category already exists.");
        if (!ModelState.IsValid) return View(category);
        _db.Add(category); await _db.SaveChangesAsync(); TempData["Success"]="Category created."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound(); var item=await _db.Categories.FindAsync(id); return item is null ? NotFound() : View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(category); await _db.SaveChangesAsync(); TempData["Success"]="Category updated."; return RedirectToAction(nameof(Index)); }
        return View(category);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item=await _db.Categories.FindAsync(id); if(item is null) return NotFound();
        if(await _db.Products.AnyAsync(p=>p.CategoryId==id)) { TempData["Error"]="Category has products; reassign them first."; return RedirectToAction(nameof(Index)); }
        _db.Remove(item); await _db.SaveChangesAsync(); TempData["Success"]="Category deleted."; return RedirectToAction(nameof(Index));
    }
}
