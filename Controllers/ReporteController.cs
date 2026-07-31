using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace ModulosTienda.Controllers
{
    public class ReporteController : Controller
    {

        private readonly IConfiguration _configuration;

        public ReporteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult ReporteVentas(DateTime? fechaI, DateTime? fechaF, string buscar)
        {
            List<ConsultaProductoVenta> lista = new List<ConsultaProductoVenta>();

            if (fechaI == null && string.IsNullOrEmpty(buscar))
            {
                TempData["Mensaje"] =
                    "Debe capturar una fecha inicial o un producto.";

                return View(lista);
            }

            using (MySqlConnection conn =
                new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"SELECT 
                                       VD.producto,
                                       P.descripcion,
                                       VD.idVenta,
                                       VD.precioVentas,
                                      (VD.precioVentas * (P.porComision/100)) AS Comision,
                                      (VD.precioVentas + (VD.precioVentas * (P.porComision/100))) AS ImporteNeto
	                             FROM ModuloTienda.Ventas V
                                 INNER JOIN ModuloTienda.VentaDetalle VD ON VD.idVenta = V.idVenta
                                 INNER JOIN ModuloTienda.Productos P ON VD.producto = P.producto
                                 WHERE V.Estatus= 'PA'";

                if (fechaI.HasValue)
                {
                    query +=
                        " AND V.FechaCierre BETWEEN @fechaI ";
                }

                if (fechaF.HasValue)
                {
                    query +=
                        " AND V.FechaCierre <= @fechaF ";
                }
                else if (fechaI.HasValue)
                {
                    query +=
                        " AND V.FechaCierre <= CURDATE() ";
                }

                if (!string.IsNullOrEmpty(buscar))
                {
                    query += @"
                AND (
                    P.producto LIKE @buscar
                    OR P.descripcion LIKE @buscar
                ) ";
                }

                query += @"
        
        ORDER BY
            p.producto";

                using (MySqlCommand cmd =
                    new MySqlCommand(query, conn))
                {
                    if (fechaI.HasValue)
                        cmd.Parameters.AddWithValue(
                            "@fechaI",
                            fechaI.Value.Date);

                    if (fechaF.HasValue)
                        cmd.Parameters.AddWithValue(
                            "@fechaF",
                            fechaF.Value.Date);

                    if (!string.IsNullOrEmpty(buscar))
                        cmd.Parameters.AddWithValue(
                            "@buscar",
                            "%" + buscar + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new ConsultaProductoVenta
                                {
                                    Producto =
                                        reader["producto"].ToString(),

                                    Descripcion =
                                        reader["descripcion"].ToString(),

                                    EnNota =
                                        Convert.ToInt32(
                                            reader["idVenta"]),

                                    ImporteVenta =
                                        Convert.ToDecimal(
                                            reader["precioVentas"]),

                                    Comision =
                                        Convert.ToDecimal(
                                            reader["Comision"]),

                                    ImporteNeto =
                                        Convert.ToDecimal(
                                            reader["ImporteNeto"])
                                });
                        }
                    }
                }
            }

            return View(lista);
        }

        public IActionResult DetalleVenta(int idVenta)
        {
            List<DetalleVentaSTRUC> lista =
                new List<DetalleVentaSTRUC>();

            using (MySqlConnection conn =
                new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"
            SELECT VD.idVenta,
                    VD.producto,
                P.descripcion,
                VD.cantidad,
                VD.precioVentas,
                VD.total,
                VD.usuarioVenta
            FROM VentaDetalle VD
            INNER JOIN Productos P
                ON VD.producto = P.producto
            WHERE VD.idVenta = @idVenta";

                using (MySqlCommand cmd =
                    new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idVenta", idVenta);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new DetalleVentaSTRUC
                            {
                                IdVenta = Convert.ToInt32(reader["idVenta"]),
                                Producto = reader["producto"].ToString(),
                                Descripcion = reader["descripcion"].ToString(),
                                Cantidad = Convert.ToInt32(reader["cantidad"]),
                                PrecioVenta = Convert.ToDecimal(reader["precioVentas"]),
                                Total = Convert.ToDecimal(reader["total"]),
                                UsuarioVenta = reader["usuarioVenta"].ToString()
                            });
                        }
                    }
                }
            }

            return View(lista);
        }


    }
}
