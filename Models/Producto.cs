namespace ModulosTienda.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Product { get; set; }
         public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string  UM { get; set; }
        public int Maximo { get; set; }
        public int Minimo { get; set; }
        public string Detalle { get; set; }
    }
}
