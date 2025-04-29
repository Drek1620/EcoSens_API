using EcoSens_API.Data;
using EcoSens_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Controllers
{
    [Route("api/Conjuntos")]
    [ApiController]
    public class ConjuntosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConjuntosController> _logger;

        public ConjuntosController(AppDbContext context, ILogger<ConjuntosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("createagregar")]
        public async Task<IActionResult> createConjunto([FromBody] Conjuntos conjunto)
        {
            try
            {
                if (conjunto == null)
                    return BadRequest(new { mensaje = "Datos incompletos" });
                _context.Conjuntos.Add(conjunto);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetConjuntosById), new { id = conjunto.Id }, conjunto);


            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<Conjuntos>> GetConjuntosById(int id)
        {

            try
            {
                var conjunto = await _context.Conjuntos
                    .Include(c => c.Area_) 
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (conjunto == null)
                {
                    return NotFound(new { mensaje = "Conjunto no encontrado." });
                }

                return Ok(new
                {
                    conjunto.Id,
                    conjunto.Mac_ESP32,
                    conjunto.Clavesecreta,
                    Area = conjunto.Area_ != null ? conjunto.Area_.Nombre : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpGet("area/{areaId}")]
        public async Task<IActionResult> ObtenerConjuntosPorArea(int areaId)
        {
            try
            {
                var conjuntos = await _context.Conjuntos
                    .Where(c => c.Area_id == areaId)
                    .Include(c => c.Area_)
                    .Select(c => new
                    {
                        c.Id,
                        c.Mac_ESP32,
                        c.Clavesecreta,
                        Area = c.Area_ != null ? c.Area_.Nombre : null
                    })
                    .ToListAsync();

                if (conjuntos == null || !conjuntos.Any())
                    return NotFound(new { mensaje = "No hay conjuntos asociados a esta área." });

                return Ok(conjuntos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

    }
}
