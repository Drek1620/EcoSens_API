using EcoSens_API.Data;
using EcoSens_API.Models;
using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace EcoSens_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly mongoDbService _mongoDbService;

        public AuthController(AppDbContext context, IConfiguration config, mongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginRequest request)
        {
            var user = _context.Usuarios.Include(u => u.Tipo_).SingleOrDefault(u => u.Correo == request.Correo && u.Contrasena == u.Contrasena);
            if (user == null)
                return Unauthorized(new { message = "Credenciales invalidas" });

            var token = GenerarToken(user);

            //Si es ADMIN, devuelve la lista de trabajadores
            if(user.Tipo_.Nombre_Tipo == "Administrador")
            {
                var empleados= _context.Empleados.ToList();
                return Ok(new
                {
                    token,
                    tipo = user.Tipo_.Nombre_Tipo,
                    empleados
                });
            }

            //**Si es un empleado muestra los conjuntos de contenedores
            if (user.Tipo_.Nombre_Tipo == "Recolector")
            {
                var conjuntos = _context.Conjuntos.ToList();
                return Ok(new
                {
                    token,
                    tipo = user.Tipo_.Nombre_Tipo,
                    conjuntos
                });
            }

            return BadRequest(new {message = "Rol no reconocido"});
        }

        private string GenerarToken(Usuario user)
        {
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:secretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Correo),
                    new Claim(ClaimTypes.Role, "admin")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
