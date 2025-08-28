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
        private readonly MongoDbService _mongoDbService;

        public AuthController(AppDbContext context, IConfiguration config, MongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;

        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioLoginRequest request)
        {
            var user = _context.Usuarios.Include(u => u.Tipo_).SingleOrDefault(u => u.Correo == request.Correo && u.Contrasena == u.Contrasena);
            var name = _context.Empleados.SingleOrDefault(x => x.Usuario_Id == user.Id);
            if (user == null)
                return Unauthorized(new { message = "Credenciales invalidas" });

            var token = GenerarToken(user); 

            //Si es ADMIN, devuelve la lista de trabajadores
            return Ok(new
            {
                token,
                user.Id,
                user.Tipo_id,
                //name.Nombre
            });

        }

        [HttpPost("esp32")]
        public IActionResult LoginEsp32([FromBody] UsuarioLoginRequest request)
        {
            var esp = _context.Conjuntos.SingleOrDefault(u => u.Mac_ESP32 == request.Correo && u.Clavesecreta == request.Contrasena);
            if (esp == null)
                return Unauthorized(new { message = "Credenciales invalidas" });

            var token = GenerarTokenEsp(esp);

            return Ok(new
            {
                token
            });
        }

        private string GenerarTokenEsp(Conjuntos esp)
        {
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:secretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,esp.Id.ToString()),
                    new Claim(ClaimTypes.Email, esp.Mac_ESP32),
                    new Claim(ClaimTypes.Role, "esp32")
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
