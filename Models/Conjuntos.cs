using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Conjuntos
    {
        public int Id { get; set; }
        [Required]
        public string Mac_ESP32 { get; set; }
        public int area_id { get; set; }
    }
}
