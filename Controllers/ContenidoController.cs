using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContenidoController : ControllerBase
    {
        private readonly IContenidoService _service;

        public ContenidoController(IContenidoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetContenidos());

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetContenidoById(id));

        [HttpPost]
        public IActionResult Insert(Contenido contenido) => Ok(_service.InsertContenido(contenido));

        [HttpPut]
        public IActionResult Update(Contenido contenido) => Ok(_service.UpdateContenido(contenido));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_service.DeleteContenido(id));
    }
}
