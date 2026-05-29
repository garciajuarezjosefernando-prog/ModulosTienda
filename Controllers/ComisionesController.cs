using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class ComisionesController : Controller
    {
        private readonly IConfiguration _configuration;

        public ComisionesController(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        public IActionResult Comisiones()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Buscar(
            string usuario,
            string desde,
            string hasta
        )
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

                vd.producto,
                p.descripcion,
                vd.cantidad,
                vd.precioVentas,
                v.FechaCierre,
                p.porComision,

                (
                    vd.precioVentas *
                    (p.porComision / 100)
                ) AS comision,

                vd.stsComision

                FROM VentaDetalle vd

                INNER JOIN Productos p
                    ON p.producto = vd.producto

               INNER JOIN Ventas v
                ON TRIM(vd.idVenta) = CAST(v.idVenta AS CHAR)

                WHERE

                vd.usuarioVenta = @usuario

                AND TRIM(v.Estatus) = 'PA'

                AND
                (
                    LENGTH(@desde)=0
                    OR DATE(v.FechaCierre) >=
                    STR_TO_DATE(@desde,'%Y-%m-%d')
                )

                AND
                (
                    LENGTH(@hasta)=0
                    OR DATE(v.FechaCierre) <=
                    STR_TO_DATE(@hasta,'%Y-%m-%d')
                )

                ORDER BY v.FechaCierre DESC";

                MySqlCommand cmd =
                    new MySqlCommand(
                        query,
                        conn
                    );

                cmd.Parameters.AddWithValue(
                    "@usuario",
                    usuario
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
                    lista.Add(new
                    {
                        producto =
                            reader["producto"],

                        descripcion =
                            reader["descripcion"],
                            
                        cantidad =
                            reader["cantidad"],

                        totalVenta =
                            reader["precioVentas"],

                        fechaVenta =
                        Convert.ToDateTime(
                            reader["FechaCierre"]
                        ).ToString("dd/MM/yyyy"),

                        porcentaje =
                            reader["porComision"],

                        comision =
                            reader["comision"],

                        status =
                            reader["stsComision"]
                    });
                }
            }

            return Json(lista);
        }
        [HttpPost]
public JsonResult PagarComisiones(
    string usuario,
    string desde,
    string hasta
)
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
        @"UPDATE VentaDetalle vd

        INNER JOIN Ventas v
            ON TRIM(vd.idVenta) =
            CAST(v.idVenta AS CHAR)

        SET vd.stsComision = 'PA'

        WHERE

        vd.usuarioVenta = @usuario

        AND vd.stsComision = 'PE'

        AND TRIM(v.Estatus) = 'PA'

        AND
        (
            LENGTH(@desde)=0
            OR DATE(v.FechaCierre) >=
            STR_TO_DATE(@desde,'%Y-%m-%d')
        )

        AND
        (
            LENGTH(@hasta)=0
            OR DATE(v.FechaCierre) <=
            STR_TO_DATE(@hasta,'%Y-%m-%d')
        )";

        MySqlCommand cmd =
            new MySqlCommand(
                query,
                conn
            );

        cmd.Parameters.AddWithValue(
            "@usuario",
            usuario
        );

        cmd.Parameters.AddWithValue(
            "@desde",
            desde ?? ""
        );

        cmd.Parameters.AddWithValue(
            "@hasta",
            hasta ?? ""
        );

        cmd.ExecuteNonQuery();
    }

    return Json(new
    {
        ok = true
    });
}
    }
}