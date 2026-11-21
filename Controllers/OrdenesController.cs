using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestión_de_Inventarios.Data;
using Sistema_de_Gestión_de_Inventarios.Models;

namespace Sistema_de_Gestión_de_Inventarios.Controllers;

public class OrdenesController : Controller
{
    private readonly InventarioDbContext _context;

    public OrdenesController(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ordenes = await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Detalles)
            .OrderByDescending(o => o.Fecha)
            .ToListAsync();
        return View(ordenes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var orden = await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(o => o.OrdenId == id);

        if (orden is null) return NotFound();
        return View(orden);
    }

    public IActionResult Create()
    {
        ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
            "ProveedorId", "Nombre");
        ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            _context.Productos.Where(p => p.Activo).OrderBy(p => p.Nombre),
            "ProductoId", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int ProveedorId, int ProductoId, int Cantidad, decimal PrecioUnitario)
    {
        if (Cantidad <= 0)
        {
            ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a cero.");
        }
        if (PrecioUnitario <= 0)
        {
            ModelState.AddModelError("PrecioUnitario", "El precio debe ser mayor a cero.");
        }

        var producto = await _context.Productos.FindAsync(ProductoId);
        if (producto is null)
        {
            ModelState.AddModelError("ProductoId", "Producto no encontrado.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["ProveedorId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Proveedores.Where(p => p.Activo).OrderBy(p => p.Nombre),
                "ProveedorId", "Nombre", ProveedorId);
            ViewData["ProductoId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.Productos.Where(p => p.Activo).OrderBy(p => p.Nombre),
                "ProductoId", "Nombre", ProductoId);
            return View();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orden = new OrdenCompra
            {
                ProveedorId = ProveedorId,
                Fecha = DateTime.UtcNow,
                Estado = "Pendiente",
                Total = Cantidad * PrecioUnitario
            };
            _context.OrdenesCompra.Add(orden);
            await _context.SaveChangesAsync();

            var detalle = new DetalleOrden
            {
                OrdenId = orden.OrdenId,
                ProductoId = ProductoId,
                Cantidad = Cantidad,
                PrecioUnitario = PrecioUnitario
            };
            _context.DetallesOrden.Add(detalle);

            if (producto is not null)
            {
                producto.Stock += Cantidad;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var orden = await _context.OrdenesCompra
            .Include(o => o.Proveedor)
            .Include(o => o.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(o => o.OrdenId == id);

        if (orden is null) return NotFound();
        return View(orden);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orden = await _context.OrdenesCompra
            .Include(o => o.Detalles)
            .FirstOrDefaultAsync(o => o.OrdenId == id);

        if (orden is not null)
        {
            foreach (var detalle in orden.Detalles)
            {
                var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                if (producto is not null)
                {
                    producto.Stock -= detalle.Cantidad;
                    if (producto.Stock < 0) producto.Stock = 0;
                }
            }
            _context.OrdenesCompra.Remove(orden);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
