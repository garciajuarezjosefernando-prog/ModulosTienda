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

        [HttpPost]
        public IActionResult ListadoEditarCliente(Cliente model)
        {
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = @"UPDATE Clientes SET
                        nombre=@nombre,
                        tipoCliente=@tipoCliente,
                        telefono=@telefono,
                        email=@email,
                        direccion=@direccion,
                        ciudad=@ciudad,
                        estado=@estado,
                        cp=@cp,
                        pais=@pais,
                        activo=@activo
                        WHERE idCliente=@id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", model.idCliente);
                    cmd.Parameters.AddWithValue("@nombre", model.nombre);
                    cmd.Parameters.AddWithValue("@tipoCliente", model.tipoCliente);
                    cmd.Parameters.AddWithValue("@telefono", model.telefono);
                    cmd.Parameters.AddWithValue("@email", model.email);
                    cmd.Parameters.AddWithValue("@direccion", model.direccion);
                    cmd.Parameters.AddWithValue("@ciudad", model.ciudad);
                    cmd.Parameters.AddWithValue("@estado", model.estado);
                    cmd.Parameters.AddWithValue("@cp", model.cp);
                    cmd.Parameters.AddWithValue("@pais", model.pais);
                    cmd.Parameters.AddWithValue("@activo", model.activo);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Mensaje"] = "Cliente actualizado correctamente";
            return RedirectToAction("EditarCliente", new { id = model.idCliente });
        }

        public IActionResult ListadoClientes()
        {
            List<Cliente> lista = new List<Cliente>();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = "SELECT * FROM Clientes";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Cliente
                        {
                            idCliente = Convert.ToInt32(reader["idCliente"]),
                            nombre = reader["nombre"].ToString(),
                            RFC = reader["RFC"].ToString(),
                            email = reader["email"].ToString(),
                            activo = Convert.ToBoolean(reader["activo"])
                        });
                    }
                }
            }

            return View(lista);
        }

        public IActionResult EditarCliente(int id)
        {
            Cliente model = new Cliente();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection")))
            {
                conn.Open();

                string query = "SELECT * FROM Clientes WHERE idCliente = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.idCliente = Convert.ToInt32(reader["idCliente"]);
                        model.nombre = reader["nombre"].ToString();
                        model.RFC = reader["RFC"].ToString();
                        model.tipoCliente = reader["tipoCliente"].ToString();
                        model.telefono = reader["telefono"].ToString();
                        model.email = reader["email"].ToString();
                        model.direccion = reader["direccion"].ToString();
                        model.ciudad = reader["ciudad"].ToString();
                        model.estado = reader["estado"].ToString();
                        model.cp = reader["cp"].ToString();
                        model.pais = reader["pais"].ToString();
                        model.activo = Convert.ToBoolean(reader["activo"]);
                    }
                }
            }

            return View(model);
        }

    }
}
