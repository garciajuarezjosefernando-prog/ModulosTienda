using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ModulosTienda.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuario, string password)
        {
            string conexion =
                _configuration.GetConnectionString("MySqlConnection");

            using (MySqlConnection conn =
                new MySqlConnection(conexion))
            {
                conn.Open();

                string query =
                @"SELECT * FROM usuarios
                  WHERE usuario=@usuario
                  AND contrasena=@password";

                MySqlCommand cmd =
                    new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@usuario", usuario);

                cmd.Parameters.AddWithValue("@password", password);

                MySqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    string rol = reader["tipo"]?.ToString() ?? "";
                    HttpContext.Session.SetString(
                        "rol",
                        rol
                    );
                    string nombre = reader["nombre"]?.ToString() ?? "";
                    HttpContext.Session.SetString(
                        "nombre",
                        nombre
                    );
                    HttpContext.Session.SetString(
                        "usuario",
                        usuario
                    );

                    return RedirectToAction(
                        "Index",
                        "Home"
                    );
                }
            }

            ViewBag.Error = "Usuario incorrecto";

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}