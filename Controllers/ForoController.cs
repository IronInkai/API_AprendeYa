using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForoController : ControllerBase
    {
        private readonly IForoService _foroService;

        public ForoController(IForoService foroService)
        {
            _foroService = foroService;
        }


        [HttpGet("preguntas")]
        public IActionResult GetPreguntas()
        {
            var data = _foroService.GetPreguntas();
            return Ok(data);
        }


        [HttpGet("pregunta/{id}")]
        public IActionResult GetPregunta(int id)
        {
            var data = _foroService.GetPreguntaById(id);
            if (data == null) return NotFound("Pregunta no encontrada");

            return Ok(data);
        }


        [HttpPost("pregunta")]
        public IActionResult CrearPregunta([FromBody] TemaForo pregunta)
        {
            if (pregunta == null)
                return BadRequest("Datos inválidos");

            var result = _foroService.CrearPregunta(pregunta);

            if (!result)
                return BadRequest("No se pudo crear la pregunta");

            return Ok("Pregunta creada correctamente");
        }


        [HttpGet("respuestas/{idTema}")]
        public IActionResult GetRespuestas(int idTema)
        {
            var data = _foroService.GetRespuestas(idTema);
            return Ok(data);
        }


        [HttpPost("respuesta")]
        public IActionResult CrearRespuesta([FromBody] RespuestaForo respuesta)
        {
            if (respuesta == null)
                return BadRequest("Datos inválidos");

            var result = _foroService.CrearRespuesta(respuesta);

            if (!result)
                return BadRequest("No se pudo crear la respuesta");

            return Ok("Respuesta creada correctamente");
        }


        [HttpPost("like")]
        public IActionResult DarLike(
            [FromQuery] int idRespuesta,
            [FromQuery] int idUsuario,
            [FromQuery] bool tipo)
        {
            var result = _foroService.DarLike(idRespuesta, idUsuario, tipo);

            if (!result)
                return BadRequest("No se pudo registrar el voto");

            return Ok("Voto registrado");
        }


        [HttpPost("mejor-respuesta")]
        public IActionResult MejorRespuesta([FromQuery] int idRespuesta)
        {
            var result = _foroService.MarcarMejorRespuesta(idRespuesta);

            if (!result)
                return BadRequest("No se pudo marcar como mejor respuesta");

            return Ok("Mejor respuesta asignada");
        }


        [HttpGet("preguntas-completo")]
        public IActionResult GetPreguntasCompleto()
        {
            var data = _foroService.GetPreguntasConRespuestas();
            return Ok(data);
        }
    }
}