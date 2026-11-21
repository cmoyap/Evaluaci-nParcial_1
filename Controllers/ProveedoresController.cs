using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestión_de_Inventarios.Data;
using Sistema_de_Gestión_de_Inventarios.Models;

namespace Sistema_de_Gestión_de_Inventarios.Controllers;

public class ProveedoresController : Controller
{
    private readonly InventarioDbContext _context;

    public ProveedoresController(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var proveedores = await _context.Proveedores
            .OrderBy(p => p.Nombre)
            .ToListAsync();
        return View(proveedores);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var proveedor = await _context.Proveedores
            .Include(p => p.Productos)
            .FirstOrDefaultAsync(p => p.ProveedorId == id);

        if (proveedor is null) return NotFound();
        return View(proveedor);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Proveedor proveedor)
    {
        if (!ModelState.IsValid) return View(proveedor);

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor is null) return NotFound();
        return View(proveedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Proveedor proveedor)
    {
        if (id != proveedor.ProveedorId) return NotFound();

        if (!ModelState.IsValid) return View(proveedor);

        try
        {
            _context.Update(proveedor);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Proveedores.Any(e => e.ProveedorId == id)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var proveedor = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.ProveedorId == id);

        if (proveedor is null) return NotFound();
        return View(proveedor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor is not null)
        {
            proveedor.Activo = false;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
