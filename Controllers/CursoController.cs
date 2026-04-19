using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _service;

        public CursoController(ICursoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetCursos());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetCursoById(id));
        }

        [HttpPost]
        public IActionResult Insert(Curso curso)
        {
            return Ok(_service.InsertCurso(curso));
        }

        [HttpPut]
        public IActionResult Update(Curso curso)
        {
            return Ok(_service.UpdateCurso(curso));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(_service.DeleteCurso(id));
        }

        // 🔍 FILTRO
        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(int? nivel, int? categoria)
        {
            var data = await _service.FiltrarCursos(nivel, categoria);
            return Ok(data);
        }
    }
}
