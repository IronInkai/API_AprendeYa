using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NivelController : ControllerBase
    {
        private readonly INivelService _service;

        public NivelController(INivelService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetNiveles());

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetNivelById(id));

        [HttpPost]
        public IActionResult Insert(Nivel nivel) => Ok(_service.InsertNivel(nivel));

        [HttpPut]
        public IActionResult Update(Nivel nivel) => Ok(_service.UpdateNivel(nivel));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_service.DeleteNivel(id));
    }
}
