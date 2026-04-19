using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    public class CursoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
