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
                Id = reader["idCliente"],
                Nombre = reader["nombre"]?.ToString()
            });
        }
    }

    return Json(lista);
}
[HttpPost]
public JsonResult GuardarNota(
    [FromBody] VentaCompleta venta)
{
    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using (MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        MySqlTransaction trans =
            conn.BeginTransaction();

        try
        {
            string queryVenta =
            @"INSERT INTO Ventas
            (idCliente, subTotal, iva,
             cantidadTotl)

            VALUES

            (@idCliente, @subTotal,
             @iva, @cantidadTotl);

            SELECT LAST_INSERT_ID();";

            MySqlCommand cmdVenta =
                new MySqlCommand(
                    queryVenta,
                    conn,
                    trans
                );

            cmdVenta.Parameters.AddWithValue(
                "@idCliente",
                venta.IdCliente
            );

            cmdVenta.Parameters.AddWithValue(
                "@subTotal",
                venta.SubTotal
            );

            cmdVenta.Parameters.AddWithValue(
                "@iva",
                venta.Iva
            );

            cmdVenta.Parameters.AddWithValue(
                "@cantidadTotl",
                venta.CantidadTotl
            );

            int idVenta =
                Convert.ToInt32(
                    cmdVenta.ExecuteScalar()
                );

            foreach(var p in venta.productos)
{
    try
    {
        string queryDetalle =
        @"INSERT INTO VentaDetalle
        (idVenta, producto,
         cantidad, total)

        VALUES

        (@idVenta, @producto,
         @cantidad, @total)";

        MySqlCommand cmdDetalle =
            new MySqlCommand(
                queryDetalle,
                conn,
                trans
            );

        cmdDetalle.Parameters.AddWithValue(
            "@idVenta",
            idVenta.ToString()
        );

        cmdDetalle.Parameters.AddWithValue(
            "@producto",
            p.Producto
        );

        cmdDetalle.Parameters.AddWithValue(
            "@cantidad",
            p.Cantidad
        );

        cmdDetalle.Parameters.AddWithValue(
            "@total",
            p.Total
        );

        cmdDetalle.ExecuteNonQuery();
    }
    catch(Exception ex2)
    {
        throw;
    }
}

            trans.Commit();

            return Json(new
            {
                ok = true
            });
        }
        catch(Exception ex)
{
    trans.Rollback();

    return Json(new
    {
        ok = false,
        mensaje = ex.Message
    });
}
        }
    }
}
    }
