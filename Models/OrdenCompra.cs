namespace Sistema_de_Gestión_de_Inventarios.Models
{
    public class OrdenCompra
    {
        public int OrdenId { get; set; }
        public int ProveedorId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Estado { get; set; } = "Pendiente";
        public decimal Total { get; set; }

        public Proveedor? Proveedor { get; set; }
        public ICollection<DetalleOrden> Detalles { get; set; } = [];
    }
}
