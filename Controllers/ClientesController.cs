using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Clientes()
        {
            return View();
        }


        private readonly IConfiguration _configuration;

        public ClientesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ Mostrar vista
        public IActionResult CrearCliente()
        {
            return View();
        }

        // ✅ Guardar cliente
        [HttpPost]
        public IActionResult GuardarCliente(Cliente model)
        {
            string conexion = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn = new MySqlConnection(conexion))
            {
                conn.Open();

                // 🔍 Validar duplicado RFC o Email
                string queryValidar = @"SELECT COUNT(*) 
                                   FROM Clientes 
                                   WHERE RFC = @RFC OR email = @email";

                using (MySqlCommand cmd = new MySqlCommand(queryValidar, conn))
                {
                    cmd.Parameters.AddWithValue("@RFC", model.RFC);
                    cmd.Parameters.AddWithValue("@email", model.email);

                    int existe = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        TempData["Error"] = "⚠️ El RFC o el correo ya están registrados.";
                        return RedirectToAction("CrearCliente");
                    }
                }

                // ✅ Insertar cliente
                string queryInsert = @"INSERT INTO Clientes
            (nombre, RFC, tipoCliente, telefono, email, direccion, ciudad, estado, cp, pais, fechaRegistro)
            VALUES
            (@nombre, @RFC, @tipoCliente, @telefono, @email, @direccion, @ciudad, @estado, @cp, @pais, NOW())";

                using (MySqlCommand cmd = new MySqlCommand(queryInsert, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", model.nombre);
                    cmd.Parameters.AddWithValue("@RFC", model.RFC);
                    cmd.Parameters.AddWithValue("@tipoCliente", model.tipoCliente);
                    cmd.Parameters.AddWithValue("@telefono", model.telefono);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@direccion", model.direccion);
                    cmd.Parameters.AddWithValue("@ciudad", model.ciudad);
                    cmd.Parameters.AddWithValue("@estado", model.estado);
                    cmd.Parameters.AddWithValue("@cp", model.cp);
                    cmd.Parameters.AddWithValue("@pais", model.pais);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Exito"] = "✅ Cliente guardado correctamente";
            return RedirectToAction("CrearCliente");
        }

    }
}
