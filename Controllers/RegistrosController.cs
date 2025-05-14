using EcoSens_API.Data;
using EcoSens_API.Models;
using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Controllers
{
    [Route("api/Registros")]
    [ApiController]
    public class RegistrosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly MongoDbService _mongoDbService;

        public RegistrosController(AppDbContext context, IConfiguration config, MongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerRegistros(int id)
        {

            try
            {


                var registros = await _mongoDbService.GetRegistroContenedoresPorFecha(id);

                return Ok(new { registros });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al guardar el registro.", error = ex.Message });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> AgregarRegistro([FromBody] RegistroContenedor registro)
        {
            
            try
            {
                if (registro == null)
                    return BadRequest(new { mensaje = "Datos de registro." });

                registro.FechaYHora = DateTime.UtcNow; // Registrar la fecha actual
                var contenedor = await _context.Contenedores.FindAsync(registro.Id_contenedor);
                if (contenedor == null)
                    return NotFound(new { mensaje = "Contenedor no encontrado" });

                if (contenedor.Estado != registro.Estado)
                {
                    await _context.Database.ExecuteSqlRawAsync("UPDATE Contenedores SET estado = {0} WHERE id = {1}", registro.Estado, contenedor.Id);

                }


                await _mongoDbService.AgregarRegistros(registro);
                return Ok(new { mensaje = "Registro agregado correctamente.", registro });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al guardar el registro.", error = ex.Message });
            }
        }

        [HttpGet("peso-total-tipo")]
        public async Task<IActionResult> ObtenerPesoTotalPorTipo()
        {
            try
            {
                // Traer contenedores de SQL Server
                var contenedores = await _context.Contenedores.ToListAsync();

                // Traer registros de MongoDB
                var registros = await _mongoDbService.GetTodosRegistros(); // 

                // Juntar en memoria
                var registrosConTipo = from registro in registros
                                       join contenedor in contenedores
                                       on registro.Id_contenedor equals contenedor.Id
                                       select new
                                       {
                                           Tipo = contenedor.Tipocont_id, //(plástico o metal)
                                           Peso = registro.Peso
                                       };

                // Agrupar y sumar
                var resultado = registrosConTipo
                    .GroupBy(r => r.Tipo)
                    .Select(g => new
                    {
                        Tipo = g.Key,
                        PesoTotal = g.Sum(x => x.Peso)
                    })
                    .ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular el peso total.", error = ex.Message });
            }
        }

        [HttpGet("contenedores/peso-total")]
        public async Task<IActionResult> ObtenerPesoTotalPorContenedor()
        {
            try
            {
                var registros = await _mongoDbService.GetTodosRegistros();

                var resultado = registros
                    .GroupBy(r => r.Id_contenedor)
                    .Select(g => new
                    {
                        ContenedorId = g.Key,
                        PesoTotal = g.Sum(r => r.Peso)
                    })
                    .OrderByDescending(r => r.PesoTotal)
                    .ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular el peso total.", error = ex.Message });
            }
        }


    }
}
