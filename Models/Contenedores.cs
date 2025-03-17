using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Contenedores
    {
        public int Id_Contenedor { get; set; }
        [Required]
        public decimal Dimensiones { get; set; }
        [Required]
        public decimal Peso_T { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        public int Id_Tipo { get; set; }
        [Required]
        public int  Id_Conjunto { get; set; }
    }
}
