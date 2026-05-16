using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers

{
    public class VentasController : Controller
    {
          private readonly IConfiguration _configuration;

        public VentasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ventas()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BuscarProductos(string texto)
        {
            List<Producto> lista =
                new List<Producto>();

            string conexion =
                _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn =
                new MySqlConnection(conexion))
            {
                conn.Open();

                string query =
                @"SELECT * FROM Productos
                  WHERE producto LIKE @texto";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue(
                    "@texto",
                    "%" + texto + "%"
                );

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Producto
                    {
                       // Id = Convert.ToInt32(reader["id"]),

                        Product =
                            reader["producto"]?.ToString() ?? "",

                        Precio =
                            Convert.ToDecimal(reader["precio"])
                    });
                }
            }

            return Json(lista);
        }
        [HttpGet]
public JsonResult BuscarClientes(string texto)
{
    List<object> lista =
        new List<object>();

    string conexion =
        _configuration.GetConnectionString("MySqlConnection");

    using (MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query =
        @"SELECT * FROM clientes
          WHERE nombre LIKE @texto";

        MySqlCommand cmd =
            new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue(
            "@texto",
            "%" + texto + "%"
        );

        MySqlDataReader reader =
            cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new
            {
                Id = reader["id"],
                Nombre = reader["nombre"]?.ToString()
            });
        }
    }

    return Json(lista);
}
    }
}