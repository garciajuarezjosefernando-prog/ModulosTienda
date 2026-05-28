using Org.BouncyCastle.Crypto.Utilities;

namespace ModulosTienda.Models
{
    public class ModelosModulosTienda
    {
    }

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
        
    }


}
