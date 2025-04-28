using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
namespace EcoSens_API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required,EmailAddress]
        public string Correo { get; set; }
        [Required]
        public string Contrasena { get; set; }

        [Required]
        public int Tipo_id { get; set; }

        // Propiedad de navegación
        public TipoUsuario Tipo_ { get; set; }

    }
}

