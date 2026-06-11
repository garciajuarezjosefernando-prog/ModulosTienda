using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Productos()
        {
            return View();
        }

        public IActionResult NuevoProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GuardarProducto(ProductoSTRUC model)
        {
            string conexion = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn = new MySqlConnection(conexion))
            {
                conn.Open();

                // ✅ Validar duplicado
                string queryValidar = "SELECT COUNT(*) FROM Productos WHERE producto = @producto";

                using (MySqlCommand cmd = new MySqlCommand(queryValidar, conn))
                {
                    cmd.Parameters.AddWithValue("@producto", model.producto);

                    int existe = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        TempData["Error"] = "⚠️ El producto ya existe.";
                        return RedirectToAction("NuevoProducto");
                    }
                }

                // ✅ Insertar
                string queryInsert = @"INSERT INTO Productos
        (producto, descripcion, UM, precio, maximoAlm, minimoAlm, porComision)
        VALUES
        (@producto, @descripcion, @UM, @precio, @maximoAlm, @minimoAlm, @porComision)";

                using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@producto", model.producto);
                    cmd.Parameters.AddWithValue("@descripcion", model.descripcion);
                    cmd.Parameters.AddWithValue("@UM", model.UM);
                    cmd.Parameters.AddWithValue("@precio", model.precio);
                    cmd.Parameters.AddWithValue("@maximoAlm", model.maximoAlm);
                    cmd.Parameters.AddWithValue("@minimoAlm", model.minimoAlm);
                    cmd.Parameters.AddWithValue("@porComision", model.porComision);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Exito"] = "✅ Producto guardado correctamente";
            return RedirectToAction("NuevoProducto");
        }
    }
}
