using EcoSens_API.Data;
using EcoSens_API.Models;
using EcoSens_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EcoSens_API.Controllers
{

    //agregar contenedores 

    [Route("api/Contenedores")]
    [ApiController]
    public class ContenedoresController : ControllerBase
    {


        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ContenedoresController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        [HttpPost]
        public IActionResult agregarcontenedor(Contenedores contenedores)
        {
            try
            {
                var contenedor = new Contenedores()
                {
                    Dimensiones = contenedores.Dimensiones,
                    Peso_Total = contenedores.Peso_Total,
                    Estado = contenedores.Estado,
                    Tipocont_Id = contenedores.Tipocont_Id,
                    Conjunto_Id = contenedores.Conjunto_Id


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

        [HttpDelete("{id}")]
        public IActionResult eliminarcontenedor(int id)
        {
            try
            {
                var contenedor = _context.Contenedores.FirstOrDefault(contenedor => contenedor.Id == id);


                _context.Remove(contenedor);
                _context.SaveChanges();


                if (contenedor == null)
                {
                    return NotFound("Contenedor no encontrado.");
                }

                return Ok(contenedor);

            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion");
            }


        }

        [HttpGet("conjunto/{conjunto_id}")]
        public IActionResult datosconjunto(int conjunto_id)
        {
            try
            {
                var contenedor = _context.Contenedores.Where(contenedor => contenedor.Conjunto_Id == conjunto_id).ToList();

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

        [HttpPut("{id}")]
        public IActionResult editarcontenedor(int id, Contenedores contenedores)
        {
            try
            {

                var contenedor = _context.Contenedores.FirstOrDefault(contenedor => contenedor.Id == id);

                contenedor.Dimensiones = contenedores.Dimensiones;
                contenedor.Peso_Total = contenedores.Peso_Total;
                contenedor.Estado = contenedores.Estado;


                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion - " + ex);
            }

            return Ok(contenedores);
        }

        [HttpGet("{id}")]
        public IActionResult datoscontenedor(int id)
        {
            try
            {
                var contenedor = _context.Contenedores.FirstOrDefault(contenedor => contenedor.Id == id);

                if (contenedor == null)
                {
                    return NotFound("Contenedor no encontrado.");
                }

                return Ok(contenedor);
            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepción");
            }
        }

        [HttpGet("conjunto/{id}/contenedores")]
        public async Task<IActionResult> ObtenerContenedoresPorConjunto(int id)
        {
            try
            {
                var plastico = await _context.Contenedores
                    .Where(c => c.Conjunto_Id == id && c.Tipocont_Id == 1)
                    .Select(c => new
                    {
                        c.Id,
                        c.Dimensiones,
                        c.Peso_Total,
                        c.Estado
                    })
                    .FirstOrDefaultAsync();

                var metal = await _context.Contenedores
                    .Where(c => c.Conjunto_Id == id && c.Tipocont_Id == 2)
                    .Select(c => new
                    {
                        c.Id,
                        c.Dimensiones,
                        c.Peso_Total,
                        c.Estado
                    })
                    .FirstOrDefaultAsync();

                if (plastico == null && metal == null)
                    return NotFound(new { mensaje = "No se encontraron contenedores para este conjunto." });

                return Ok(new
                {
                    conjuntoId = id,
                    contenedorPlastico = plastico,
                    contenedorMetal = metal
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener contenedores.", error = ex.Message });
            }
        }


    }


}
