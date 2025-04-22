using EcoSens_API.Data;
using EcoSens_API.Models;
using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore.Internal;

namespace EcoSens_API.Controllers
{
    [Route("api/edit")]
    [ApiController]
    public class ConjuntosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly mongoDbService _mongoDbService;

        public ConjuntosController(AppDbContext context, IConfiguration config, mongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;
        }

        [HttpGet("datosconj/{Id_Conjunto}")]
        public IActionResult datoscontenedor(int conjunto_id)
        {
            try
            {
                var contenedor = _context.Contenedores.Where(contenedor => contenedor.Id_Conjunto == conjunto_id).ToList();


                if (contenedor == null)
                {
                    return NotFound("Conjunto no encontrado.");
                }

                return Ok(contenedor);
            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepción");
            }
        }
    }
}
