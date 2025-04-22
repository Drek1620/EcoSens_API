using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Contenedores
    {
        public int Id { get; set; }
        [Required]
        public decimal Dimensiones { get; set; }
        public decimal Peso_T { get; set; }
        public string Estado { get; set; }
        public int Id_Tipo { get; set; }
        public int  Id_Conjunto { get; set; }
    }
}
