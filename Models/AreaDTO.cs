using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class AreaDTO
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
    }
}
