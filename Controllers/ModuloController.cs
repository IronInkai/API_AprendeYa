using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloController : ControllerBase
    {
        private readonly IModuloService _service;

        public ModuloController(IModuloService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetModulos());

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetModuloById(id));

        [HttpPost]
        public IActionResult Insert(Modulo modulo) => Ok(_service.InsertModulo(modulo));

        [HttpPut]
        public IActionResult Update(Modulo modulo) => Ok(_service.UpdateModulo(modulo));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_service.DeleteModulo(id));
    }
}
