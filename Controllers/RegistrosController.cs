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

        [HttpGet("peso-por-tipo")]
        public async Task<IActionResult> ObtenerPesoTotalPorTipo()
        {
            try
            {
                var contenedores = await _context.Contenedores.ToListAsync();
                var registros = await _mongoDbService.GetTodosRegistros();

                var registrosConTipo = from registro in registros
                                       join contenedor in contenedores
                                       on registro.Id_contenedor equals contenedor.Id
                                       select new
                                       {
                                           Tipo = contenedor.Tipocont_id,
                                           Peso = registro.Peso
                                       };

                var agrupados = registrosConTipo
                    .GroupBy(r => r.Tipo)
                    .Select(g => new
                    {
                        Tipo = g.Key,
                        PesoTotal = g.Sum(x => x.Peso)
                    })
                    .ToList();

                // Convertir a objeto plano con nombres conocidos
                var resultado = new
                {
                    plastico = agrupados.FirstOrDefault(g => g.Tipo == 1)?.PesoTotal ?? 0,
                    metal = agrupados.FirstOrDefault(g => g.Tipo == 2)?.PesoTotal ?? 0
                };

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

        [HttpGet("estadisticas/pesos-mensuales")]
        public async Task<IActionResult> ObtenerPesosPorMes()
        {
            try
            {
                var contenedores = await _context.Contenedores.ToListAsync();
                var registros = await _mongoDbService.GetTodosRegistros();

                // Vincular registros con tipo de contenedor
                var registrosConTipo = from registro in registros
                                       join cont in contenedores on registro.Id_contenedor equals cont.Id
                                       select new
                                       {
                                           Fecha = registro.FechaYHora,
                                           Peso = registro.Peso,
                                           Tipo = cont.Tipocont_id // 1 = Plástico, 2 = Metal
                                       };

                // Limitar a los últimos 6 meses
                var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5);
                var fechaFin = fechaInicio.AddMonths(6);

                // Agrupar por mes y tipo
                var datosAgrupados = registrosConTipo
                    .Where(r => r.Fecha >= fechaInicio && r.Fecha < fechaFin)
                    .GroupBy(r => new { r.Fecha.Year, r.Fecha.Month, r.Tipo })
                    .Select(g => new
                    {
                        Año = g.Key.Year,
                        Mes = g.Key.Month,
                        Tipo = g.Key.Tipo,
                        Peso = g.Sum(x => x.Peso)
                    })
                    .ToList();

                // Preparar etiquetas de mes
                var etiquetasMes = Enumerable.Range(0, 6)
                    .Select(i => fechaInicio.AddMonths(i))
                    .ToList();

                var labels = etiquetasMes.Select(m => m.ToString("MMM")).ToArray();

                var plastico = new double[6];
                var metal = new double[6];

                for (int i = 0; i < etiquetasMes.Count; i++)
                {
                    var mes = etiquetasMes[i].Month;
                    var año = etiquetasMes[i].Year;

                    plastico[i] = datosAgrupados
                        .Where(x => x.Mes == mes && x.Año == año && x.Tipo == 1)
                        .Sum(x => x.Peso);

                    metal[i] = datosAgrupados
                        .Where(x => x.Mes == mes && x.Año == año && x.Tipo == 2)
                        .Sum(x => x.Peso);
                }

                var resultado = new
                {
                    labels = labels,
                    series = new[]
                    {
                new { name = "Plasticos", data = plastico },
                new { name = "Metales", data = metal }
            }
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al generar estadísticas.", error = ex.Message });
            }
        }


        [HttpGet("peso-total-por-area")]
        public async Task<IActionResult> ObtenerPesoTotalPorArea()
        {
            try
            {
                var contenedores = await _context.Contenedores
                    .Include(c => c.Conjunto_)
                    .ThenInclude(conj => conj.Area_)
                    .ToListAsync();

                var registros = await _mongoDbService.GetTodosRegistros(); // Mongo

                // Join y agrupación por área
                var datosAgrupados = registros
                    .Join(contenedores,
                        reg => reg.Id_contenedor,
                        cont => cont.Id,
                        (reg, cont) => new
                        {
                            Peso = reg.Peso,
                            Area = cont.Conjunto_?.Area_?.Nombre ?? "Sin área"
                        })
                    .GroupBy(x => x.Area)
                    .Select(g => new
                    {
                        Area = g.Key,
                        PesoTotal = g.Sum(x => x.Peso)
                    })
                    .OrderByDescending(x => x.PesoTotal)
                    .ToList();

                // Formato para gráfico
                var response = new
                {
                    categories = datosAgrupados.Select(x => x.Area).ToArray(),
                    data = datosAgrupados.Select(x => x.PesoTotal).ToArray()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular el peso por área.", error = ex.Message });
            }
        }

        [HttpGet("contenedor/{id}/peso-desde-ultimo-vacio-parcial")]
        public async Task<IActionResult> ObtenerPesoDesdeUltimoParcialVacio(int id)
        {
            try
            {
                var registros = await _mongoDbService.GetRegistrosPorContenedor(id);

                if (registros == null || !registros.Any())
                    return NotFound(new { mensaje = "No se encontraron registros para el contenedor." });

                // Buscar el último con estado "Parcialmente Vacío"
                var ultimoParcial = registros
                    .Where(r => r.Estado.Equals("Vacio", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(r => r.FechaYHora)
                    .FirstOrDefault();

                DateTime fechaInicio = ultimoParcial?.FechaYHora ?? DateTime.MinValue;

                // Sumar pesos después de esa fecha
                var pesoTotal = registros
                    .Where(r => r.FechaYHora > fechaInicio)
                    .Sum(r => r.Peso);

                return Ok(new
                {
                    contenedorId = id,
                    desde = fechaInicio,
                    pesoAcumulado = pesoTotal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al calcular el peso.", error = ex.Message });
            }
        }

    }
}
