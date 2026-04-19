using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemaController : ControllerBase
    {
        private readonly ITemaService _service;

        public TemaController(ITemaService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetTemas());

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetTemaById(id));

        [HttpPost]
        public IActionResult Insert(Tema tema) => Ok(_service.InsertTema(tema));

        [HttpPut]
        public IActionResult Update(Tema tema) => Ok(_service.UpdateTema(tema));

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_service.DeleteTema(id));
    }
}
