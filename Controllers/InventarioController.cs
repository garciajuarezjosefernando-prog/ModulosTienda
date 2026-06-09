using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class InventarioController : Controller
    {
        private readonly IConfiguration _configuration;

        public InventarioController(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Inventario()
        {
            return View();
        }
        public IActionResult Editar(

            string producto)
        {
            ViewBag.Producto =
                producto;
            return View();
        }

        [HttpGet]
public JsonResult ObtenerProducto(
    string producto)
{
    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using(MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query =
        @"SELECT

            producto,
            descripcion,
            UM,
            precio,
            existencia,
            maximoAlm,
            minimoAlm,
            porComision

        FROM Productos

        WHERE producto = @producto";

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        cmd.Parameters.AddWithValue(
            "@producto",
            producto
        );

        MySqlDataReader reader =
            cmd.ExecuteReader();

        if(reader.Read())
        {
            return Json(
                new
                {
                    producto =
                        reader["producto"],

                    descripcion =
                        reader["descripcion"],

                    um =
                        reader["UM"],

                    precio =
                        reader["precio"],

                    existencia =
                        reader["existencia"],

                    maximo =
                        reader["maximoAlm"],

                    minimo =
                        reader["minimoAlm"],

                    porComision =
                        reader["porComision"]
                }
            );
        }
    }

    return Json(null);
}
[HttpPost]
public JsonResult GuardarProducto(
    [FromBody] Producto producto)
{
    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using(MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query =
        @"UPDATE Productos

        SET

            descripcion = @descripcion,
            UM = @UM,
            precio = @precio,
            existencia = @existencia,
            maximoAlm = @maximo,
            minimoAlm = @minimo,
            porComision = @comision

        WHERE producto = @producto";

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        cmd.Parameters.AddWithValue(
            "@producto",
            producto.Product
        );

        cmd.Parameters.AddWithValue(
            "@descripcion",
            producto.Descripcion
        );

        cmd.Parameters.AddWithValue(
            "@UM",
            producto.UM
        );

        cmd.Parameters.AddWithValue(
            "@precio",
            producto.Precio
        );

        cmd.Parameters.AddWithValue(
            "@existencia",
            producto.Existencia
        );

        cmd.Parameters.AddWithValue(
            "@maximo",
            producto.Maximo
        );

        cmd.Parameters.AddWithValue(
            "@minimo",
            producto.Minimo
        );

        cmd.Parameters.AddWithValue(
            "@comision",
            producto.PorComision
        );

        cmd.ExecuteNonQuery();
    }

    return Json(new
    {
        ok = true
    });
}
        [HttpGet]
public JsonResult BuscarProductosInventario(
    string producto,
    string descripcion)
{
    List<Producto> lista =
        new List<Producto>();

    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using(MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query = "";

        if(!string.IsNullOrWhiteSpace(producto))
        {
            query =
            @"SELECT

                producto,
                descripcion,
                precio,
                existencia,
                maximoAlm,
                minimoAlm

            FROM Productos

            WHERE Estatus = 1

            AND producto LIKE @texto

            ORDER BY descripcion";
        }
        else if(!string.IsNullOrWhiteSpace(descripcion))
        {
            query =
            @"SELECT

                producto,
                descripcion,
                precio,
                existencia,
                maximoAlm,
                minimoAlm

            FROM Productos

            WHERE Estatus = 1

            AND descripcion LIKE @texto

            ORDER BY descripcion";
        }
        else
        {
            query =
            @"SELECT

                producto,
                descripcion,
                precio,
                existencia,
                maximoAlm,
                minimoAlm

            FROM Productos

            WHERE Estatus = 1

            ORDER BY descripcion

            LIMIT 50";
        }

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        if(
            !string.IsNullOrWhiteSpace(producto)
            ||
            !string.IsNullOrWhiteSpace(descripcion)
        )
        {
            cmd.Parameters.AddWithValue(
                "@texto",
                "%" +
                (producto ?? descripcion)
                + "%"
            );
        }

        MySqlDataReader reader =
            cmd.ExecuteReader();

        while(reader.Read())
        {
            lista.Add(
                new Producto
                {
                    Product =
                        reader["producto"]?.ToString() ?? "",

                    Descripcion =
                        reader["descripcion"]?.ToString() ?? "",

                    Precio =
                        Convert.ToDecimal(
                            reader["precio"]
                        ),

                    Existencia =
                        Convert.ToInt32(
                            reader["existencia"]
                        ),

                    Maximo =
                        Convert.ToInt32(
                            reader["maximoAlm"]
                        ),

                    Minimo =
                        Convert.ToInt32(
                            reader["minimoAlm"]
                        )
                }
            );
        }
    }

    return Json(lista);
}
[HttpPost]
public JsonResult EliminarProducto(
    string producto)
{
    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using(MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query =
        @"UPDATE Productos

        SET Estatus = 0

        WHERE producto = @producto";

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        cmd.Parameters.AddWithValue(
            "@producto",
            producto
        );

        cmd.ExecuteNonQuery();
    }

    return Json(new
    {
        ok = true
    });
}

        [HttpGet]
public JsonResult ObtenerProductos()
{
    List<Producto> lista =
        new List<Producto>();

    string conexion =
        _configuration.GetConnectionString(
            "MySqlConnection"
        );

    using(MySqlConnection conn =
        new MySqlConnection(conexion))
    {
        conn.Open();

        string query =
        @"SELECT

            producto,
            descripcion,
            precio,
            existencia,
            maximoAlm,
            minimoAlm

        FROM Productos

        WHERE Estatus = 1

        ORDER BY descripcion

        LIMIT 50";

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        MySqlDataReader reader =
            cmd.ExecuteReader();

        while(reader.Read())
        {
            lista.Add(
                new Producto
                {
                    Product =
                        reader["producto"]?.ToString() ?? "",

                    Descripcion =
                        reader["descripcion"]?.ToString() ?? "",

                    Precio =
                        Convert.ToDecimal(
                            reader["precio"]
                        ),

                    Existencia =
                        Convert.ToInt32(
                            reader["existencia"]
                        ),

                    Maximo =
                        Convert.ToInt32(
                            reader["maximoAlm"]
                        ),

                    Minimo =
                        Convert.ToInt32(
                            reader["minimoAlm"]
                        )
                }
            );
        }
    }

    return Json(lista);
}

        
    }
    
}