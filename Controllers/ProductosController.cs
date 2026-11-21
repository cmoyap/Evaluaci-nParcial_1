using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestión_de_Inventarios.Data;
using Sistema_de_Gestión_de_Inventarios.Models;

namespace Sistema_de_Gestión_de_Inventarios.Controllers;

public class ProductosController : Controller
{
    private readonly InventarioDbContext _context;

    public ProductosController(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _context.Productos
            .Include(p => p.Proveedor)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
        return View(productos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var producto = await _context.Productos
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.ProductoId == id);

        if (producto is null) return NotFound();
        return View(producto);
    }

    public IActionResult Create()
    {
        ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
            "ProveedorId", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
                "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var producto = await _context.Productos.FindAsync(id);
        if (producto is null) return NotFound();

        ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
            "ProveedorId", "Nombre", producto.ProveedorId);
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Producto producto)
    {
        if (id != producto.ProductoId) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
                "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        try
        {
            _context.Update(producto);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductoExists(producto.ProductoId)) return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var producto = await _context.Productos
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.ProductoId == id);

        if (producto is null) return NotFound();
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is not null)
        {
            producto.Activo = false;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductoExists(int id) =>
        _context.Productos.Any(e => e.ProductoId == id);
}
