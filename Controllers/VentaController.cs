using API_AprendeYa.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_AprendeYa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // GET: api/Venta/Reporte
        [HttpGet("Reporte")]
        public IActionResult GetReporte()
        {
            try
            {
                var reporte = _ventaService.ObtenerReporteDeVentas();
                // FÍJATE AQUÍ: El backend retorna un Ok() con los datos crudos. No sabe qué es una Vista.
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }
    }
}