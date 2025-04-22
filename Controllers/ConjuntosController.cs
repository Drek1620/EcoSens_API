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

    [Route("Contenedores")]
    [ApiController]
    public class ContController : ControllerBase
    {


        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ContController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        [HttpPost("addcont")]

        public IActionResult agregarcontenedor(Contenedores contenedores)
        {
            try
            {
                var contenedor = new Contenedores()
                {
                    Dimensiones = contenedores.Dimensiones,
                    Peso_Total = contenedores.Peso_Total,
                    Estado = contenedores.Estado,
                    Tipocont_id = contenedores.Tipocont_id,
                    Conjunto_id = contenedores.Conjunto_id


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




    //borrar contenedor


    [Route("contenedores")]
    [ApiController]
    public class DelController : ControllerBase
    {


        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public DelController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpDelete("{id}")]

        public IActionResult editarcontenedor(int id)
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



    }






    //editar contenedor


    [Route("contenedores")]
    [ApiController]
    public class EditController : ControllerBase
    {


        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public EditController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

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
                return BadRequest("Ha ocurrido una excepcion");
            }

            return Ok(contenedores);
        }



    }








    //obtener contenedores de un conjunto

    [Route("contenedores")]
    [ApiController]
    public class ConjuntosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ConjuntosController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("conjunto/{conjunto_id}")]
        public IActionResult datosconjunto(int conjunto_id)
        {
            try
            {
                var contenedor = _context.Contenedores.Where(contenedor => contenedor.Conjunto_id == conjunto_id).ToList();


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











    //obtener datos de contenedor

    [Route("contenedores")]
    [ApiController]
    public class DatosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public DatosController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
    }







}
