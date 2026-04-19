using API_AprendeYa.Models;
using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            var resultado = await _usuarioService.RegistrarUsuarioAsync(request);
            if (resultado) return Ok(new { mensaje = "Cuenta creada con éxito" });
            return BadRequest(new { mensaje = "Error al crear la cuenta" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioService.LoginAsync(request);
            if (usuario == null) return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
            return Ok(usuario);
        }
    }
}