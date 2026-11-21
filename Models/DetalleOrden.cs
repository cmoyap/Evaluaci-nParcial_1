namespace Sistema_de_Gestión_de_Inventarios.Models
{
    public class DetalleOrden
    {
        public int DetalleId { get; set; }
        public int OrdenId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public OrdenCompra? Orden { get; set; }
        public Producto? Producto { get; set; }
    }
}
