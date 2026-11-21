using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestión_de_Inventarios.Models;

namespace Sistema_de_Gestión_de_Inventarios.Data;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
    public DbSet<DetalleOrden> DetallesOrden => Set<DetalleOrden>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(e =>
        {
            e.HasKey(p => p.ProductoId);
            e.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Proveedor)
             .WithMany(p => p.Productos)
             .HasForeignKey(p => p.ProveedorId);
        });

        modelBuilder.Entity<Proveedor>(e =>
        {
            e.HasKey(p => p.ProveedorId);
        });

        modelBuilder.Entity<OrdenCompra>(e =>
        {
            e.HasKey(o => o.OrdenId);
            e.Property(o => o.Total).HasColumnType("decimal(18,2)");
            e.HasOne(o => o.Proveedor)
             .WithMany(p => p.OrdenesCompra)
             .HasForeignKey(o => o.ProveedorId);
        });

        modelBuilder.Entity<DetalleOrden>(e =>
        {
            e.HasKey(d => d.DetalleId);
            e.Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
            e.HasOne(d => d.Orden)
             .WithMany(o => o.Detalles)
             .HasForeignKey(d => d.OrdenId);
            e.HasOne(d => d.Producto)
             .WithMany(p => p.Detalles)
             .HasForeignKey(d => d.ProductoId);
        });
    }
}
