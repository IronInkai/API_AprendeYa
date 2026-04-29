using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        // ==========================================
        // 1. OBTENER EL CARRITO (El que estaba dando 404)
        // GET: api/Carrito/5
        // ==========================================
        [HttpGet("{idUsuario}")]
        public IActionResult ObtenerCarrito(int idUsuario)
        {
            try
            {
                var carrito = _carritoService.ObtenerCarrito(idUsuario);
                return Ok(carrito);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el carrito: " + ex.Message });
            }
        }

        // ==========================================
        // 2. AGREGAR AL CARRITO
        // POST: api/Carrito/Agregar/5/Curso/12
        // ==========================================
        [HttpPost("Agregar/{idUsuario}/Curso/{idCurso}")]
        public IActionResult Agregar(int idUsuario, int idCurso)
        {
            var respuesta = _carritoService.AgregarAlCarrito(idUsuario, idCurso);

            if (respuesta.Exito)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }
        // DELETE: api/Carrito/Eliminar/5/Detalle/10
        [HttpDelete("Eliminar/{idUsuario}/Detalle/{idDetalle}")]
        public IActionResult Eliminar(int idUsuario, int idDetalle)
        {
            var respuesta = _carritoService.EliminarDelCarrito(idUsuario, idDetalle);

            if (respuesta.Exito) return Ok(respuesta);
            return BadRequest(respuesta);
        }
        // POST: api/Carrito/Pagar/5
        [HttpPost("Pagar/{idUsuario}")]
        public IActionResult Pagar(int idUsuario)
        {
            var respuesta = _carritoService.PagarCarrito(idUsuario);

            if (respuesta.Exito) return Ok(respuesta);
            return BadRequest(respuesta);
        }
    }
}