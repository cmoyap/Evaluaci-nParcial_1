using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestión_de_Inventarios.Models;

public class Producto
{
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Required]
    public int ProveedorId { get; set; }

    public bool Activo { get; set; } = true;

    public Proveedor? Proveedor { get; set; }
    public ICollection<DetalleOrden> Detalles { get; set; } = [];
}
