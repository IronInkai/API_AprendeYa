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
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroRequest request)
        {
            try
            {
                var exito = await _usuarioService.RegistrarUsuarioAsync(request);
                if (exito)
                {
                    return Ok(new { mensaje = "Cuenta creada exitosamente" });
                }

                // Si exito es falso pero no hubo excepción
                return BadRequest(new { mensaje = "El usuario o correo ya existen" });
            }
            catch (Exception ex)
            {
                // ¡AQUÍ ESTÁ LA TRAMPA! Ahora imprimiremos el error exacto de SQL
                return BadRequest(new { mensaje = "Error SQL: " + ex.Message });
            }
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