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

        public IActionResult ListadoProductoEditar(string buscar)
        {
            List<ProductoSTRUC> lista = new List<ProductoSTRUC>();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"SELECT * FROM Productos
                         WHERE producto LIKE @buscar
                         OR descripcion LIKE @buscar";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buscar", "%" + (buscar ?? "") + "%");

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new ProductoSTRUC
                        {
                            producto = reader["producto"].ToString(),
                            descripcion = reader["descripcion"].ToString(),
                            UM = reader["UM"].ToString(),
                            precio = Convert.ToDecimal(reader["precio"]),
                            maximoAlm = Convert.ToInt32(reader["maximoAlm"]),
                            minimoAlm = Convert.ToInt32(reader["minimoAlm"]),
                            porComision = Convert.ToDecimal(reader["porComision"]),
                            activo = Convert.ToBoolean(reader["activo"])

                        });
                    }
                }
            }

            return View(lista);
        }

        public IActionResult EditarProducto(string id)
        {
            ProductoSTRUC model = new ProductoSTRUC();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = "SELECT * FROM Productos WHERE producto = @producto";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@producto", id);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.producto = reader["producto"].ToString();
                        model.descripcion = reader["descripcion"].ToString();
                        model.UM = reader["UM"].ToString();
                        model.precio = Convert.ToDecimal(reader["precio"]);
                        model.maximoAlm = Convert.ToInt32(reader["maximoAlm"]);
                        model.minimoAlm = Convert.ToInt32(reader["minimoAlm"]);
                        model.porComision = Convert.ToDecimal(reader["porComision"]);
                        model.activo = Convert.ToBoolean(reader["activo"]);
                    }
                }
            }

            return View(model);
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

        [HttpPost]
        public IActionResult EditarProducto(ProductoSTRUC
            model)
        {
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"UPDATE Productos SET
                        descripcion=@descripcion,
                        UM=@UM,
                        precio=@precio,
                        maximoAlm=@maximoAlm,
                        minimoAlm=@minimoAlm,
                        porComision=@porComision,
                        activo = @activo
                        WHERE producto=@producto";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@producto", model.producto);
                    cmd.Parameters.AddWithValue("@descripcion", model.descripcion);
                    cmd.Parameters.AddWithValue("@UM", model.UM);
                    cmd.Parameters.AddWithValue("@precio", model.precio);
                    cmd.Parameters.AddWithValue("@maximoAlm", model.maximoAlm);
                    cmd.Parameters.AddWithValue("@minimoAlm", model.minimoAlm);
                    cmd.Parameters.AddWithValue("@porComision", model.porComision);
                    cmd.Parameters.AddWithValue("@activo", model.activo);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "✅ Producto actualizado correctamente";
            return RedirectToAction("ListadoProductoEditar");
        }
    }
}
