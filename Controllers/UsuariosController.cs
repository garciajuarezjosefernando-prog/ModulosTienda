using Microsoft.AspNetCore.Mvc;

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
    }
}
