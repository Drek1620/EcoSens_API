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

namespace EcoSens_API.Controllers
{ 
    
    [Route("api/cont")]
        [ApiController]
    public class ContController : ControllerBase
    {
       

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly mongoDbService _mongoDbService;

        public ContController(AppDbContext context, IConfiguration config, mongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;

        }

        [HttpPost("addcont")]

        public IActionResult agregarcontenedor(Contenedores contenedores)
        {
            try
            {
                var contenedor = new Contenedores()
                {
                    Dimensiones = contenedores.Dimensiones,
                    Peso_T = contenedores.Peso_T,
                    Estado = contenedores.Estado,
                    Id_Tipo= contenedores.Id_Tipo,
                    Id_Conjunto = contenedores. Id_Conjunto


                };

                _context.Contenedores.Add(contenedor);
                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion");
            }

            return Ok(contenedores);
        }


      
    }
}
