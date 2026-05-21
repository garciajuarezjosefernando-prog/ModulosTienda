namespace ModulosTienda.Models
{
    public class VentaCompleta
    {
        public int IdCliente { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Iva { get; set; }

        public decimal CantidadTotl { get; set; }

        public List<DetalleVenta> productos { get; set; }
    }
}