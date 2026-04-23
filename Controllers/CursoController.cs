using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet] // Público: Cualquiera puede ver el catálogo
        public IActionResult Get() => Ok(_service.GetCursos());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_service.GetCursoById(id));
        }

        [Authorize] // Protegido: Solo usuarios logueados pueden insertar
        [HttpPost]
        public IActionResult Insert(Curso curso) => Ok(_service.InsertCurso(curso));

        

        [Authorize(Roles = "1")] // Generalmente, solo el Admin (Rol 1) o el Instructor pueden editar
        [HttpPut]
        public IActionResult Update(Curso curso)
        {
            //Verificación básica: El ID no puede ser 0
            if (curso.idCurso <= 0)
            {
                return BadRequest(new { mensaje = "ID de curso no válido para actualización." });
            }

            var resultado = _service.UpdateCurso(curso);

            //Si el servicio devuelve 0 o false, significa que el curso no existía
            if (resultado == false)
            {
                return NotFound(new { mensaje = "No se encontró el curso para actualizar." });
            }

            return Ok(new { mensaje = "Curso actualizado correctamente", data = resultado });
        }

        [Authorize(Roles = "1")] // Muy Protegido: Solo Administradores pueden borrar
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) => Ok(_service.DeleteCurso(id));

        //FILTRO
        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar(int? nivel, int? categoria)
        {
            var data = await _service.FiltrarCursos(nivel, categoria);
            return Ok(data);
        }
    }
}
