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

                var existe = await _context.Conjuntos.AnyAsync(c => c.Mac_ESP32 == dto.Mac_ESP32);

                if (existe)
                {
                    return BadRequest(new { mensaje = "La MAC ya existe en la base de datos." });
                }

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
        public async Task<ActionResult<ConjuntoConContenedoresDto>> GetConjuntosById(int id)
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

                var ConjuntoConContenedores = await _context.Conjuntos
                    .Where(c => c.Id == id)
                    .Select(c => new ConjuntoConContenedoresDto
                    {
                        Mac_ESP32 = c.Mac_ESP32,
                        Clavesecreta = c.Clavesecreta,
                        Area_Id = c.Area_Id,
                        Contenedores = _context.Contenedores
                            .Where(ct => ct.Conjunto_Id == c.Id)
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                return Ok(ConjuntoConContenedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPut("editar-conjunto-contenedor")]
        public async Task<IActionResult> EditarConjuntoConContenedor([FromBody] ConjuntoConContenedoresDto dto)
        {

            var conjunto = await _context.Conjuntos.FirstOrDefaultAsync(x => x.Id == dto.Id);
            conjunto.Mac_ESP32 = dto.Mac_ESP32;
            conjunto.Clavesecreta = dto.Clavesecreta;
            conjunto.Area_Id = dto.Area_Id;
            _context.SaveChanges();

            var contenedores = await _context.Contenedores.Where(x => x.Conjunto_Id == dto.Id).ToListAsync();
            foreach (var contenedor in contenedores)
            {
                var contenedorDto = dto.Contenedores.FirstOrDefault(c => c.Id == contenedor.Id);
                if (contenedorDto != null)
                {
                    contenedor.Tipocont_Id = contenedorDto.Tipocont_Id;
                    contenedor.Estado = contenedorDto.Estado;
                }
            }
            _context.SaveChanges();
            return Ok(dto);
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
