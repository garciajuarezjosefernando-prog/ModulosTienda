using Microsoft.AspNetCore.Mvc;

namespace ModulosTienda.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuario, string password)
        {
            if (usuario == "admin" && password == "1234")
            {
                HttpContext.Session.SetString("usuario", usuario);

                return RedirectToAction("Index", "Home");
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