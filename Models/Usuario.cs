using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
namespace EcoSens_API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required,EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Tipo { get; set; }
    }
}
