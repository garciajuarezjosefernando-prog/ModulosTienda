using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers

{
    public class VentasController : Controller
    {
        public IActionResult Ventas()
        {
            return View();
        }

         [HttpGet]
        public JsonResult BuscarProductos(string texto)
        {
            List<Producto> lista = new List<Producto>();

            string conexion =
    "server=localhost;database=ModuloTienda;user=root;password=12345678;";
            using (MySqlConnection con = new MySqlConnection(conexion))
            {
                con.Open();

                string sql = @"SELECT * 
                               FROM productos
                               WHERE nombre LIKE @texto";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Producto
                    {
                        //Id = Convert.ToInt32(reader["id"]),
                        Product = reader["producto"].ToString(),
                        Precio = Convert.ToDecimal(reader["precio"]),
                       // Descripcion = reader["descripcion"].ToString(),
                        //UM = reader["UM"].ToString(),
                       // Maximo = Convert.ToInt32(reader["maximoAlm"]),
                        //Minimo = Convert.ToInt32(reader["minimoAlm"])

                    });
                }
            }

            return Json(lista);
        }
    }
}