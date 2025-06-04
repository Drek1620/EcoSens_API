using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Contenedores
    {
        public int Id { get; set; }
        [Required]
        public decimal Dimensiones { get; set; }
        public decimal Peso_Total { get; set; }
        public string Estado { get; set; }
        public int Tipocont_id { get; set; }
        public int  Conjunto_id { get; set; }
        public Conjuntos Conjunto_ { get; set; }
    }
}
