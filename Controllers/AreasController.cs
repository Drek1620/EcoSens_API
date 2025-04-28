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


    //agregar areas

    [Route("Area")]
    [ApiController]

    public class AreasController : ControllerBase
    {


        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AreasController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        [HttpPost]
        public IActionResult agregararea(Area area)
        {
            try
            {
                var areas = new Area()
                {
                    Nombre = area.Nombre,
                    Descripcion = area.Descripcion,




                };

                _context.Area.Add(area);
                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion");
            }

            return Ok(area);
        }

        [HttpDelete("{id}")]
        public IActionResult editararea(int id)
        {
            try
            {
                var area = _context.Area.FirstOrDefault(area => area.Id == id);


                _context.Remove(area);
                _context.SaveChanges();


                if (area == null)
                {
                    return NotFound("Contenedor no encontrado.");
                }

                return Ok(area);

            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion");
            }


        }

        [HttpPut("{id}")]
        public IActionResult editarArea(int id, Area areas)
        {
            try
            {

                var area = _context.Area.FirstOrDefault(area => area.Id == id);

                area.Nombre = areas.Nombre;
                area.Descripcion = areas.Descripcion;


                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepcion - " + ex);
            }

            return Ok(areas);
        }


        [HttpGet("area")]
        public List<Area> obtenerareas()
        {
         
                return _context.Area.OrderBy(c => c.Id).ToList();

          
        }


        [HttpGet("{id}")]
        public IActionResult datosarea(int id)
        {
            try
            {
                var area = _context.Area.FirstOrDefault(area=> area.Id == id);

                if (area == null)
                {
                    return NotFound("Contenedor no encontrado.");
                }

                return Ok(area);
            }
            catch (Exception ex)
            {
                return BadRequest("Ha ocurrido una excepción");
            }
        }

    }







}



