using EcoSens_API.Data;
using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Controllers
{
    [Route("api/Notificaciones")]
    [ApiController]
    public class NotificacionesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public NotificacionesController(AppDbContext context, IConfiguration config, MongoDbService mongoDbService)
        {
            _context = context;
            _config = config;

        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerNotificacionesPorUsuario(int usuarioId)
        {
            try
            {
                var notificaciones = await _context.Notificaciones
                    .Where(n => n.UsuarioId == usuarioId)
                    .OrderByDescending(n => n.Fecha)
                    .Select(n => new
                    {
                        n.Id,
                        n.Mensaje,
                        n.Fecha,
                        n.Leido
                    })
                    .ToListAsync();

                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener notificaciones.", error = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}/noleidos")]
        public async Task<IActionResult> ObtenerNoLeidas(int usuarioId)
        {
            try
            {
                var notificaciones = await _context.Notificaciones
                    .Where(n => n.UsuarioId == usuarioId && n.Leido == false)
                    .OrderByDescending(n => n.Fecha)
                    .Select(n => new
                    {
                        n.Id,
                        n.Mensaje,
                        n.Fecha,
                        n.Leido
                    })
                    .ToListAsync();

                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener notificaciones.", error = ex.Message });
            }
        }

        [HttpPut("{id}/leido")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FindAsync(id);

                if (notificacion == null)
                    return NotFound(new { mensaje = "Notificación no encontrada." });

                notificacion.Leido = true;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Notificación marcada como leída." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la notificación.", error = ex.Message });
            }
        }


    }
}
