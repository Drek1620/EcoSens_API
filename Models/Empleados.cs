using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Empleados
    {
        public int Id { get; set; }
        [Required]
        public int Usuario_id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required]
        public int Area_trabajo { get; set; }
    }
}
