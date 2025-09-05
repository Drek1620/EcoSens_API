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

        [HttpPost("conjunto-con-contenedores")]
        public async Task<IActionResult> CrearConjuntoConContenedores([FromBody] ConjuntoConContenedoresDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear el conjunto
                var nuevoConjunto = new Conjuntos
                {
                    Mac_ESP32 = dto.Mac_ESP32,
                    Clavesecreta = dto.Clavesecreta,
                    Area_Id = dto.Area_Id
                };

                _context.Conjuntos.Add(nuevoConjunto);
                int result = _context.SaveChanges();

                // 2. Crear los contenedores con la relación al nuevo conjunto
                foreach (var cont in dto.Contenedores)
                {
                    cont.Conjunto_Id = nuevoConjunto.Id;
                    _context.Contenedores.Add(cont);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    conjuntoId = nuevoConjunto.Id,
                    contenedoresAgregados = dto.Contenedores.Count
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error al insertar datos.", error = ex.Message });
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

        [HttpGet("area/{areaId}/conjuntos-estados")]
        public async Task<IActionResult> ObtenerConjuntosConEstadosPorArea(int areaId)
        {
            try
            {
                // Obtener el área primero
                var area = await _context.Area.FindAsync(areaId);
                if (area == null)
                    return NotFound(new { mensaje = "Área no encontrada" });

                // Obtener los conjuntos asociados
                var conjuntos = await _context.Conjuntos
                    .Where(c => c.Area_Id == areaId)
                    .Select(conjunto => new
                    {
                        Id = conjunto.Id,
                        Mac_ESP32 = conjunto.Mac_ESP32,
                        Clavesecreta = conjunto.Clavesecreta,

                        Contenedor_plastico = _context.Contenedores
                            .Where(ct => ct.Conjunto_Id == conjunto.Id && ct.Tipocont_Id == 1)
                            .Select(ct => ct.Estado)
                            .FirstOrDefault(),

                        Contenedor_metal = _context.Contenedores
                            .Where(ct => ct.Conjunto_Id == conjunto.Id && ct.Tipocont_Id == 2)
                            .Select(ct => ct.Estado)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                // Armar respuesta final
                var resultado = new
                {
                    areaId = area.Id,
                    areaNombre = area.Nombre,
                    conjuntos = conjuntos
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener conjuntos.", error = ex.Message });
            }
        }



    }
}
