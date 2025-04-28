using EcoSens_API.Data;
using EcoSens_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Controllers
{
    [Route("api/Empleados")]
    [ApiController]
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _context;

        public EmpleadosController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        [HttpGet("Usuario")]
        public async Task<ActionResult<IEnumerable<EmpleadoUsuario>>> ObtenerEmpleadosConUsuario()
        {
            var empleados = await _context.Empleados
                .Include(x => x.Usuario_)
                    .ThenInclude(u => u.Tipo_)
                .Include(x => x.Area)
                .Select(x => new EmpleadoUsuario
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Apellido = x.Apellido,
                    Correo = x.Usuario_.Correo,
                    Telefono = x.Telefono,
                    TipoEmpleado = x.Usuario_.Tipo_ != null ? x.Usuario_.Tipo_.Nombre : "",
                    Area = x.Area != null ? x.Area.Nombre : "",
                    Foto = x.Foto
                })
                .ToListAsync();

            return Ok(empleados);
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuarioYEmpleado([FromBody] UsuarioEmpleadoDatos dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear Usuario
                var usuario = new Usuario
                {
                    Correo = dto.Correo,
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasenia),
                    Tipo_id = dto.TipoId
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Crear Empleado asociado
                var empleado = new Empleados
                {
                    Usuario_id = usuario.Id,
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Telefono = dto.Telefono,
                    AreaId = dto.AreaId,
                    Foto = dto.Foto
                };

                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { UsuarioId = usuario.Id, EmpleadoId = empleado.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerUsuarioYEmpleadoPorId(int usuarioId)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Usuario_)
                        .ThenInclude(u => u.Tipo_)
                    .Include(e => e.Area)
                    .Where(e => e.Usuario_id == usuarioId)
                    .Select(e => new UsuarioEmpleadoDatos
                    {
                        UsuarioId = e.Usuario_id,
                        Correo = e.Usuario_.Correo,
                        Contrasenia = e.Usuario_.Contrasena,
                        TipoId = e.Usuario_.Tipo_id,

                        // Datos del Empleado
                        Nombre = e.Nombre,
                        Apellido = e.Apellido,
                        Telefono = e.Telefono,
                        AreaId = e.AreaId,
                        Foto = e.Foto
                    })
                    .FirstOrDefaultAsync();

                if (empleado == null)
                    return NotFound(new { mensaje = "Usuario o empleado no encontrado." });

                return Ok(empleado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPut("{usuarioId}")]
        public async Task<IActionResult> EditarUsuarioYEmpleado(int usuarioId, [FromBody] UsuarioEmpleadoDatos dto)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            var empleado = await _context.Empleados.FirstOrDefaultAsync(x => x.Usuario_id == usuarioId);
            if (empleado == null)
                return NotFound("Empleado asociado no encontrado");

            // Editar Usuario
            usuario.Correo = dto.Correo ?? usuario.Correo;
            if (!string.IsNullOrWhiteSpace(dto.Contrasenia))
            {
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasenia);
            }
            usuario.Tipo_id = dto.TipoId;

            // Editar Empleado
            empleado.Nombre = dto.Nombre ?? empleado.Nombre;
            empleado.Apellido = dto.Apellido ?? empleado.Apellido;
            empleado.Telefono = dto.Telefono ?? empleado.Telefono;
            empleado.AreaId = dto.AreaId;
            empleado.Foto = dto.Foto ?? empleado.Foto;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario y empleado actualizados correctamente" });
        }

        [HttpDelete("{usuarioId}")]
        public async Task<IActionResult> EliminarUsuarioYEmpleado(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            var empleado = await _context.Empleados.FirstOrDefaultAsync(x => x.Usuario_id == usuarioId);

            // Primero eliminamos el empleado 
            if (empleado != null)
                _context.Empleados.Remove(empleado);

            // Luego eliminamos el usuario
            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario y empleado eliminados correctamente" });
        }


    }
}
