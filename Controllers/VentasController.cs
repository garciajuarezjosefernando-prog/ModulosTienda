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

        public IActionResult ventas(int? idVenta)
            {
                ViewBag.idVenta = idVenta;

                return View();
            }
            public IActionResult BuscarClientes()
            {
                return View();
            }
            public IActionResult ConsultaVentas()
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
                             Detalle =
                                reader["descripcion"]?.ToString() ?? "",
                            Precio =
                                Convert.ToDecimal(reader["precio"])
                        });
                    }
                }

                return Json(lista);
            }
            [HttpGet]

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
                int idVenta = 0;

    if(venta.IdVenta > 0)
    {

        string estatusAnterior = "";

string queryEstatus =
@"SELECT Estatus
FROM Ventas
WHERE idVenta = @idVenta";

        MySqlCommand cmdEstatus =
            new MySqlCommand(
                queryEstatus,
                conn,
                trans
            );

        cmdEstatus.Parameters.AddWithValue(
            "@idVenta",
            venta.IdVenta
        );

        estatusAnterior =
            cmdEstatus.ExecuteScalar()
            ?.ToString() ?? "";

        string updateVenta =
        @"UPDATE Ventas SET

            idCliente = @idCliente,
            subTotal = @subTotal,
            iva = @iva,
            cantidadTotl = @cantidadTotl,
            Estatus = @Estatus,
            UsuarioCierra = @UsuarioCierra,
            FechaCierre = @FechaCierre
            

        WHERE idVenta = @idVenta";

        MySqlCommand cmdUpdate =
            new MySqlCommand(
                updateVenta,
                conn,
                trans
            );

        cmdUpdate.Parameters.AddWithValue(
            "@idVenta",
            venta.IdVenta
        );

        cmdUpdate.Parameters.AddWithValue(
            "@idCliente",
            venta.IdCliente
        );

        cmdUpdate.Parameters.AddWithValue(
            "@subTotal",
            venta.SubTotal
        );

        cmdUpdate.Parameters.AddWithValue(
            "@iva",
            venta.Iva
        );

        cmdUpdate.Parameters.AddWithValue(
            "@cantidadTotl",
            venta.CantidadTotl
        );
        cmdUpdate.Parameters.AddWithValue(
            "@Estatus",
            venta.Estatus
        );

        cmdUpdate.Parameters.AddWithValue(
            "@UsuarioCierra",
            venta.Estatus == "PA"
            ? HttpContext.Session.GetString("usuario")
            : ""
        );

        cmdUpdate.Parameters.AddWithValue(
            "@FechaCierre",
            venta.Estatus == "PA"
            ? DateTime.Now
            : DBNull.Value
        );
        cmdUpdate.ExecuteNonQuery();

        idVenta = venta.IdVenta;

        string borrarDetalle =
        @"DELETE FROM VentaDetalle
        WHERE TRIM(idVenta)=@idVenta";

        MySqlCommand cmdDelete =
            new MySqlCommand(
                borrarDetalle,
                conn,
                trans
            );

        cmdDelete.Parameters.AddWithValue(
            "@idVenta",
            idVenta.ToString()
        );

        cmdDelete.ExecuteNonQuery();
    }
    else
    {
        string queryVenta =
    @"INSERT INTO Ventas
    (
        idCliente,
        subTotal,
        iva,
        cantidadTotl,
        fechaModif,
        Estatus,
        usuario,
        UsuarioCierra,
        FechaCierre
    )

    VALUES

    (
        @idCliente,
        @subTotal,
        @iva,
        @cantidadTotl,
        @fechaModif,
        @Estatus,
        @usuario,
        @UsuarioCierra,
        @FechaCierre
    );

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

        cmdVenta.Parameters.AddWithValue(
            "@fechaModif",
            DateTime.Now
        );

        cmdVenta.Parameters.AddWithValue(
            "@Estatus",
            venta.Estatus
        );

        cmdVenta.Parameters.AddWithValue(
            "@usuario",
            HttpContext.Session.GetString("usuario")
        );

        cmdVenta.Parameters.AddWithValue(
            "@UsuarioCierra",
            venta.Estatus == "PA"
            ? HttpContext.Session.GetString("usuario")
            : ""
                );

        cmdVenta.Parameters.AddWithValue(
            "@FechaCierre",
        venta.Estatus == "PA"
            ? DateTime.Now
            : DBNull.Value
        );

        idVenta =
            Convert.ToInt32(
                cmdVenta.ExecuteScalar()
            );
    }


                foreach(var p in venta.productos)
    {
        try
        {
            string queryDetalle =
            @"INSERT INTO VentaDetalle
            ( idVenta,
            producto,
            cantidad,
            total,
            precioVentas,
            usuarioVenta
            )

            VALUES

            ( @idVenta,
        @producto,
        @cantidad,
        @total,
        @precioVentas,
        @usuarioVenta)";

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

            decimal precioConIva =
            p.Precio;

            decimal subtotalConIva =
            p.Cantidad * precioConIva;

            cmdDetalle.Parameters.AddWithValue(
                "@total",
                precioConIva
            );

            cmdDetalle.Parameters.AddWithValue(
                "@precioVentas",
                subtotalConIva
            );
            
            cmdDetalle.Parameters.AddWithValue(
            "@usuarioVenta",
            HttpContext.Session.GetString("usuario")
    );

            cmdDetalle.ExecuteNonQuery();
        }
        catch(Exception ex2)
        {
            throw;
        }
    }
    bool descontarInventario = false;

        if(venta.Estatus == "PA")
        {
            if(venta.IdVenta == 0)
            {
                descontarInventario = true;
            }
        }
        if(descontarInventario)
{
    // VALIDAR EXISTENCIAS

    foreach(var p in venta.productos)

    {

        string queryExistencia =
        @"SELECT existencia
          FROM Productos
          WHERE producto = @producto";

        MySqlCommand cmdExistencia =
            new MySqlCommand(
                queryExistencia,
                conn,
                trans
            );
        cmdExistencia.Parameters.AddWithValue(
            "@producto",
            p.Producto
        );
        int existenciaActual =
            Convert.ToInt32(
                cmdExistencia.ExecuteScalar()
            );
        if(existenciaActual < p.Cantidad)
        {
            throw new Exception(
                "Inventario insuficiente para: "
                + p.Producto
            );
        }
    }

    // DESCONTAR INVENTARIO
    foreach(var p in venta.productos)
    {
        string queryInventario =
        @"UPDATE Productos

        SET existencia =
            existencia - @cantidad

        WHERE producto =
            @producto";

        MySqlCommand cmdInv =
            new MySqlCommand(
                queryInventario,
                conn,
                trans
            );

        cmdInv.Parameters.AddWithValue(
            "@cantidad",
            p.Cantidad
        );

        cmdInv.Parameters.AddWithValue(
            "@producto",
            p.Producto
        );

        cmdInv.ExecuteNonQuery();
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
        
    [HttpGet]
    [HttpGet]
    public JsonResult Buscar(
        string folio,
        string cliente,
        string estado,
        string desde,
        string hasta)
    {
        List<object> lista =
            new List<object>();

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

                v.idVenta,
                c.nombre AS cliente,
                v.Estatus,
                v.cantidadTotl,
                v.FechaVenta

            FROM Ventas v

            INNER JOIN clientes c
                ON c.idCliente = v.idCliente

            WHERE

            (
                @folio = ''
                OR v.idVenta = @folio
            )

          AND
            (
                @cliente = ''
                OR c.nombre LIKE @cliente2
            )

            AND
            (
                @estado = ''
                OR v.Estatus = @estado
            )

            AND
        (
                LENGTH(@desde) = 0
                OR DATE(v.fechaVenta) >= STR_TO_DATE(@desde,'%Y-%m-%d')
            )

            AND
            (
                LENGTH(@hasta) = 0
                OR DATE(v.fechaVenta) <= STR_TO_DATE(@hasta,'%Y-%m-%d')
            )

            ORDER BY v.idVenta DESC";

            MySqlCommand cmd =
                new MySqlCommand(
                    query,
                    conn
                );

            cmd.Parameters.AddWithValue(
                "@folio",
                folio ?? ""
            );

            cmd.Parameters.AddWithValue(
                "@cliente",
                cliente ?? ""
            );

            cmd.Parameters.AddWithValue(
                "@cliente2",
                "%" + cliente + "%"
            );

            cmd.Parameters.AddWithValue(
                "@estado",
                estado ?? ""
            );

            cmd.Parameters.AddWithValue(
                "@desde",
                desde ?? ""
            );

            cmd.Parameters.AddWithValue(
                "@hasta",
                hasta ?? ""
            );

            MySqlDataReader reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                int idVenta =
                    Convert.ToInt32(
                        reader["idVenta"]
                    );

                List<object> detalle =
                    new List<object>();

                using(MySqlConnection conn2 =
                    new MySqlConnection(conexion))
                {
                    conn2.Open();

                    string q2 =
                    @"SELECT

                        producto,
                        cantidad,
                        total

                    FROM VentaDetalle

                    WHERE TRIM(idVenta) = @idVenta";

                    MySqlCommand cmd2 =
                        new MySqlCommand(
                            q2,
                            conn2
                        );

                    cmd2.Parameters.AddWithValue(
                        "@idVenta",
                        idVenta.ToString()
                    );

                    MySqlDataReader r2 =
                        cmd2.ExecuteReader();

                    while(r2.Read())
                    {
                        detalle.Add(new
                        {
                            producto =
                                r2["producto"],

                            cantidad =
                                r2["cantidad"],

                            total =
                                r2["total"]
                        });
                    }
                }

                lista.Add(new
                {
                    folio =
                        idVenta,

                    cliente =
                        reader["cliente"],

                    estado =
                        reader["Estatus"],

                    total =
                        reader["cantidadTotl"],
                    estatus =

                        reader["Estatus"],
                    fecha =
                    reader["FechaVenta"] == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(
                        reader["FechaVenta"]
                    ).ToString("dd/MM/yyyy"),

                    detalle =
                        detalle
                });
            }
        }

        return Json(lista);
    }
    [HttpGet]
    public JsonResult ObtenerVentas( string folio, string cliente, string estado, string desde, string hasta)
    {
        List<object> lista =
            new List<object>();

        string usuarioSesion =
            HttpContext.Session.GetString("usuario");

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

        v.idVenta,
        c.nombre AS cliente,
        v.cantidadTotl,
        v.Estatus,
        v.usuario

        FROM Ventas v

        INNER JOIN clientes c
            ON c.idCliente = v.idCliente

        WHERE

        (
            @folio = ''
            OR v.idVenta = @folio
        )

        AND
        (
            @cliente = ''
            OR c.nombre LIKE @cliente2
        )

        AND
        (
            @estado = ''
            OR v.Estatus = @estado
        )

        AND
        (
            LENGTH(@desde) = 0
            OR DATE(v.fechaVenta) >= STR_TO_DATE(@desde,'%Y-%m-%d')
        )

        AND
        (
            LENGTH(@hasta) = 0
            OR DATE(v.fechaVenta) <= STR_TO_DATE(@hasta,'%Y-%m-%d')
        )

        ORDER BY v.idVenta DESC

        LIMIT 20";

            MySqlCommand cmd =
                new MySqlCommand(query, conn);
    cmd.Parameters.AddWithValue(
        "@folio",
        folio ?? ""
    );

    cmd.Parameters.AddWithValue(
        "@cliente",
        cliente ?? ""
    );

    cmd.Parameters.AddWithValue(
        "@cliente2",
        "%" + cliente + "%"
    );

    cmd.Parameters.AddWithValue(
        "@estado",
        estado ?? ""
    );

    cmd.Parameters.AddWithValue(
        "@desde",
        desde ?? ""
    );

    cmd.Parameters.AddWithValue(
        "@hasta",
        hasta ?? ""
    );
            MySqlDataReader reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                string usuarioCreador =
                    reader["usuario"]?.ToString() ?? "";

                lista.Add(new
                {
                    folio =
                        reader["idVenta"],

                    cliente =
                        reader["cliente"],

                    importe =
                        reader["cantidadTotl"],

                    estado =
                        reader["Estatus"],

                    puedeEliminar =
                    usuarioSesion ==
                    usuarioCreador
                    &&
                    reader["Estatus"]?.ToString() != "PA"
                });
            }
        }

        return Json(lista);
    }

    [HttpPost]
    public JsonResult EliminarVenta(int idVenta)
    {
        string usuarioSesion =
            HttpContext.Session.GetString("usuario");

        string conexion =
            _configuration.GetConnectionString(
                "MySqlConnection"
            );

        using(MySqlConnection conn =
            new MySqlConnection(conexion))
        {
            conn.Open();

            string validar =
            @"SELECT usuario
            FROM Ventas
            WHERE idVenta = @idVenta";

            MySqlCommand cmdValidar =
                new MySqlCommand(validar, conn);

            cmdValidar.Parameters.AddWithValue(
                "@idVenta",
                idVenta
            );

            string usuarioCreador =
                cmdValidar.ExecuteScalar()?.ToString() ?? "";

            if(usuarioSesion != usuarioCreador)
            {
                return Json(new
                {
                    ok = false,
                    mensaje =
                    "Solo el usuario que creó la nota puede eliminarla"
                });
            }

            string eliminarDetalle =
            @"DELETE FROM VentaDetalle
            WHERE TRIM(idVenta) = @idVenta";

            MySqlCommand cmdDetalle =
                new MySqlCommand(
                    eliminarDetalle,
                    conn
                );

            cmdDetalle.Parameters.AddWithValue(
                "@idVenta",
                idVenta.ToString()
            );

            cmdDetalle.ExecuteNonQuery();

            string eliminarVenta =
            @"DELETE FROM Ventas
            WHERE idVenta = @idVenta";

            MySqlCommand cmdVenta =
                new MySqlCommand(
                    eliminarVenta,
                    conn
                );

            cmdVenta.Parameters.AddWithValue(
                "@idVenta",
                idVenta
            );

            cmdVenta.ExecuteNonQuery();

            return Json(new
            {
                ok = true
            });
        }
    }
    [HttpGet]
    public JsonResult BuscarClientesJson(string texto)
    {
        List<object> lista =
            new List<object>();

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

                idCliente,
                nombre

            FROM clientes

            WHERE nombre LIKE @texto

            LIMIT 10";

            MySqlCommand cmd =
                new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue(
                "@texto",
                "%" + texto + "%"
            );

            MySqlDataReader reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                lista.Add(new
                {
                    id =
                        reader["idCliente"],

                    nombre =
                        reader["nombre"]
                });
            }
        }

        return Json(lista);
    }
    [HttpGet]
    public JsonResult ObtenerVenta(int idVenta)
    {
        string conexion =
            _configuration.GetConnectionString(
                "MySqlConnection"
            );

        object venta = null;

        using(MySqlConnection conn =
            new MySqlConnection(conexion))
        {
            conn.Open();

            string query =
            @"SELECT

                v.idVenta,
                v.idCliente,
                c.nombre,
                v.cantidadTotl,
                v.Estatus

            FROM Ventas v

            INNER JOIN clientes c
                ON c.idCliente = v.idCliente

            WHERE v.idVenta = @idVenta";

            MySqlCommand cmd =
                new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue(
                "@idVenta",
                idVenta
            );

            MySqlDataReader reader =
                cmd.ExecuteReader();

            if(reader.Read())
            {
                venta = new
                {
                    idVenta =
                        reader["idVenta"],

                    idCliente =
                        reader["idCliente"],

                    cliente =
                        reader["nombre"],

                   total =
                    reader["cantidadTotl"],

                estatus =
                    reader["Estatus"]

                };
            }

            reader.Close();

            List<object> detalle =
                new List<object>();

            string q2 =
            @"SELECT

                vd.producto,
                p.descripcion,
                vd.cantidad,
                vd.precioVentas,
                vd.total

            FROM VentaDetalle vd

            LEFT JOIN Productos p
                ON p.producto = vd.producto

            WHERE TRIM(idVenta)=@idVenta";

            MySqlCommand cmd2 =
                new MySqlCommand(q2, conn);

            cmd2.Parameters.AddWithValue(
                "@idVenta",
                idVenta.ToString()
            );

            MySqlDataReader r2 =
                cmd2.ExecuteReader();

            while(r2.Read())
            {
                detalle.Add(new
                {
                    producto =
                        r2["producto"],
                    detalle =
                        r2["descripcion"],
                    cantidad =
                        r2["cantidad"],

                    precio =
                        r2["precioVentas"],

                    total =
                        r2["total"]
                });
            }

            return Json(new
            {
                venta,
                detalle
            });
        }
    }
    }
        }
