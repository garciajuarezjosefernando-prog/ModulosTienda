using Microsoft.AspNetCore.Mvc;
using ModulosTienda.Models;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class UsuariosController : Controller
    {

        public IActionResult Usuarios()
        {
            return View();
        }

        public IActionResult NuevoUsuario()
        {
            return View();
        }

        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        public IActionResult Guardar(Usuario model)
        {
            string conexion = _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn = new MySqlConnection(conexion))
            {
                conn.Open();

                
                string queryValidar = @"SELECT COUNT(*) 
                                    FROM Usuarios 
                                    WHERE usuario = @usuario 
                                    OR correo = @correo";

                using (MySqlCommand cmdValidar = new MySqlCommand(queryValidar, conn))
                {
                    cmdValidar.Parameters.AddWithValue("@usuario", model.usuario);
                    cmdValidar.Parameters.AddWithValue("@correo", model.correo);

                    int existe = Convert.ToInt32(cmdValidar.ExecuteScalar());

                    if (existe > 0)
                    {
                        
                        TempData["Error"] = "El usuario o correo ya existe.";
                        return View("NuevoUsuario");
                    }
                }

                
                string queryInsert = @"INSERT INTO Usuarios
                                (usuario, nombre, contrasena, correo, tipo, activo)
                                VALUES
                                (@usuario, @nombre, @contrasena, @correo, @tipo, @activo)";

                using (MySqlCommand cmdInsert = new MySqlCommand(queryInsert, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@usuario", model.usuario);
                    cmdInsert.Parameters.AddWithValue("@nombre", model.nombre);
                    cmdInsert.Parameters.AddWithValue("@contrasena", model.contrasena);
                    cmdInsert.Parameters.AddWithValue("@correo", model.correo);
                    cmdInsert.Parameters.AddWithValue("@tipo", model.tipo);
                    cmdInsert.Parameters.AddWithValue("@activo", 1);

                    cmdInsert.ExecuteNonQuery();
                }
            }




            TempData["Exito"] = "Usuario guardado correctamente";

            return RedirectToAction("Usuarios", "Usuarios");

        }

    }
}
