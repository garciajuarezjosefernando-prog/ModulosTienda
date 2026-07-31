using Org.BouncyCastle.Crypto.Utilities;

namespace ModulosTienda.Models
{
    

    public class Usuario
    {
        public string usuario { get; set; }
        public string nombre { get; set; }
        public string contrasena { get; set; }
        public string correo { get; set; }
        public string tipo { get; set; }
        public bool activo { get; set; }
    }


    public class ProductoSTRUC
    {
        public string producto { get; set; }
        public string descripcion { get; set; }
        public string UM { get; set; }
        public decimal precio { get; set; }
        public int maximoAlm { get; set; }
        public int minimoAlm { get; set; }
        public decimal porComision { get; set; }
        public bool activo { get; set; }
        public decimal costo { get; set; }

    }

    public class Cliente
    {
        public int idCliente { get; set; }
        public string nombre { get; set; }
        public string RFC { get; set; }
        public string tipoCliente { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public string estado { get; set; }
        public string cp { get; set; }
        public string pais { get; set; }
        public DateTime fechaRegistro { get; set; }
        public bool activo { get; set; }
    }

    public class ConsultaProductoVenta
    {
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public int EnNota { get; set; }
        public decimal ImporteVenta { get; set; }
        public decimal Comision { get; set; }
        public decimal ImporteNeto { get; set; }
    }

    public class DetalleVentaSTRUC
    {
        public int IdVenta { get; set; }
        public string Producto { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Total { get; set; }
        public string UsuarioVenta { get; set; }
    }

}
