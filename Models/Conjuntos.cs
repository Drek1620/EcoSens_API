using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Conjuntos
    {
        public int Id { get; set; }
        [Required]
        public string Mac_ESP32 { get; set; }
        public int Area_id { get; set; }
        [Required]
        public string Clavesecreta { get; set; }

        public Area Area_ { get; set; }

        public ICollection<Contenedores> Contenedores { get; set; }
    }
}
