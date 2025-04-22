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
    
    [Route("api/edit")]
        [ApiController]
    public class EditController : ControllerBase
    {
       

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly mongoDbService _mongoDbService;

        public EditController(AppDbContext context, IConfiguration config, mongoDbService mongoDbService)
        {
            _context = context;
            _config = config;
            _mongoDbService = mongoDbService;

        }

        [HttpPut("editcont")]

        public IActionResult editarcontenedor(Contenedores contenedores)
        {
            try
            {
                var contenedor = new Contenedores()
                {
                    Id = contenedores.Id,
                    Dimensiones = contenedores.Dimensiones,
                    Peso_T = contenedores.Peso_T,
                    Estado = contenedores.Estado,
                    Id_Tipo = contenedores.Id_Tipo,
                    Id_Conjunto= contenedores.Id_Conjunto


                };

                _context.Update(contenedor);
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
