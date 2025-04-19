using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoSens_API.Controllers
{
    [Route("api/registros")]
    [ApiController]
    public class RegistroController : Controller
    {
        
        private readonly MongoDbService _mongoDbService;
        public RegistroController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet("{idTrabajador}")]
        public async Task<IActionResult> ObtenerPagosPorTrabajador(int idTrabajador)
        {
            var pagos = await _mongoDbService.GetRegistroContenedores(idTrabajador);
            if (pagos == null || pagos.Count == 0)
                return NotFound(new { mensaje = "No se encontraron registros para este contenedor." });

            return Ok(pagos);
        }
    }
}