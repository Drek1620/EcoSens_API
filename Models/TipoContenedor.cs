using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class TipoContenedor
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}
