using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/Usuario
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _usuarioService.GetUsuarios();
            return Ok(usuarios);
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public IActionResult GetUsuarioById(int id)
        {
            var usuario = _usuarioService.GetUsuarioById(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }
            return Ok(usuario);
        }

        // POST: api/Usuario
        [HttpPost]
        public IActionResult InsertUsuario([FromBody] UsuarioAdmin usuario)
        {
            var exito = _usuarioService.InsertUsuario(usuario);
            if (exito)
            {
                return Ok(new { mensaje = "Usuario creado exitosamente" });
            }
            return BadRequest(new { mensaje = "Error al crear el usuario" });
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] UsuarioAdmin usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest(new { mensaje = "El ID de la ruta no coincide con el ID del usuario" });
            }

            var exito = _usuarioService.UpdateUsuario(usuario);
            if (exito)
            {
                return Ok(new { mensaje = "Usuario actualizado exitosamente" });
            }
            return BadRequest(new { mensaje = "Error al actualizar el usuario" });
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            var exito = _usuarioService.DeleteUsuario(id);
            if (exito)
            {
                return Ok(new { mensaje = "Usuario eliminado exitosamente" });
            }
            return BadRequest(new { mensaje = "Error al eliminar el usuario" });
        }


        // POST: api/Usuario/5/Matricular/10
        [HttpPost("{idUsuario}/Matricular/{idCurso}")]
        public IActionResult MatricularAlumno(int idUsuario, int idCurso)
        {
            try
            {
                var exito = _usuarioService.MatricularAlumno(idUsuario, idCurso);

                if (exito)
                {
                    return Ok(new { mensaje = "Alumno matriculado exitosamente en el curso." });
                }

                return BadRequest(new { mensaje = "El alumno ya se encuentra matriculado en este curso o hubo un error." });
            }
            catch (Exception ex)
            {
                // Capturamos cualquier error (como si la tabla se llama diferente)
                return StatusCode(500, new { mensaje = "Error interno del servidor: " + ex.Message });
            }
        } 
        // GET: api/Usuario/5/MisCursos
        [HttpGet("{idUsuario}/MisCursos")]
        public IActionResult GetCursosAlumno(int idUsuario)
        {
            try
            {
                var cursos = _usuarioService.ObtenerCursosDeAlumno(idUsuario);
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los cursos: " + ex.Message });
            }
        }
    }
}