using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class TipoEmpleado
    {
        public int Id { get; set; }
        [Required]
        public string Nombre_Tipo { get; set; }
    }
}
