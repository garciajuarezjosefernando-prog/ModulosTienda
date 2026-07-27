using Microsoft.AspNetCore.Mvc;

namespace ModulosTienda.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult ReporteVentas()
        {
            return View();
        }
    }
}
