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

        public IActionResult CambioUsuario(string id)
        {
            
            Usuario user = new Usuario();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = "SELECT * FROM Usuarios WHERE usuario = @usuario";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", id);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user.usuario = reader["usuario"].ToString();
                        user.nombre = reader["nombre"].ToString();
                        user.contrasena = reader["contrasena"].ToString();
                        user.correo = reader["correo"].ToString();
                        user.tipo = reader["tipo"].ToString();
                        user.activo = Convert.ToBoolean(reader["activo"]);
                    }
                }
            }

            return View(user);
       
        }

        public IActionResult ListadoUsuarioEditar(string buscar)
        {
            List<Usuario> lista = new List<Usuario>();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"SELECT usuario, nombre, contrasena, correo, tipo, activo 
                         FROM Usuarios
                         WHERE usuario LIKE @buscar OR correo LIKE @buscar";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buscar", "%" + buscar + "%");

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Usuario
                        {
                            usuario = reader["usuario"].ToString(),
                            nombre = reader["nombre"].ToString(),
                            correo = reader["correo"].ToString(),
                            tipo = reader["tipo"].ToString(),
                            activo = Convert.ToBoolean(reader["activo"])
                        });
                    }
                }
            }

            return View(lista);
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

        [HttpPost]
        public IActionResult GuardarCambios(Usuario model)
        {
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"UPDATE Usuarios 
                         SET nombre=@nombre,
                             correo=@correo,
                             tipo=@tipo,
                             activo=@activo
                         WHERE usuario=@usuario";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@usuario", model.usuario);
                    cmd.Parameters.AddWithValue("@nombre", model.nombre);
                    cmd.Parameters.AddWithValue("@correo", model.correo);
                    cmd.Parameters.AddWithValue("@tipo", model.tipo);
                    cmd.Parameters.AddWithValue("@activo", model.activo);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["ExitoCambio"] = "Usuario actualizado correctamente";
            return RedirectToAction("ListadoUsuarioEditar");
        }

    }
}
