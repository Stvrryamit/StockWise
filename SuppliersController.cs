using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWise.Data;
using StockWise.Models;

namespace StockWise.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly AppDbContext _db;
    public SuppliersController(AppDbContext db) => _db=db;
    public async Task<IActionResult> Index() => View(await _db.Suppliers.Include(s=>s.Products).OrderBy(s=>s.Name).ToListAsync());
    public IActionResult Create() => View(new Supplier());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Supplier supplier)
    {
        if(!ModelState.IsValid) return View(supplier);
        _db.Add(supplier); await _db.SaveChangesAsync(); TempData["Success"]="Supplier created."; return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if(id is null) return NotFound(); var item=await _db.Suppliers.FindAsync(id); return item is null ? NotFound() : View(item);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Supplier supplier)
    {
        if(id!=supplier.Id) return NotFound();
        if(!ModelState.IsValid) return View(supplier);
        _db.Update(supplier); await _db.SaveChangesAsync(); TempData["Success"]="Supplier updated."; return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item=await _db.Suppliers.FindAsync(id); if(item is null) return NotFound();
        _db.Remove(item); await _db.SaveChangesAsync(); TempData["Success"]="Supplier deleted."; return RedirectToAction(nameof(Index));
    }
}
