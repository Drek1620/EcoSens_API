using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Conjuntos
    {
        public int IdConjuntos { get; set; }
        [Required]
        public string IP_ESP32 { get; set; }
    }
}
