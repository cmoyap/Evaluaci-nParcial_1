using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestión_de_Inventarios.Models;

public class Proveedor
{
    public int ProveedorId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string? Direccion { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string? Email { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Producto> Productos { get; set; } = [];
    public ICollection<OrdenCompra> OrdenesCompra { get; set; } = [];
}
