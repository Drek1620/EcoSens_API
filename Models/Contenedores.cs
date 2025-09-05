using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoSens_API.Models
{

    public class Contenedores
    {
        public int Id { get; set; }

        [Required]
        public decimal Dimensiones { get; set; }

        public decimal Peso_Total { get; set; }
        public string Estado { get; set; }
        public int Tipocont_Id { get; set; }
        public int Conjunto_Id { get; set; }

        [JsonIgnore]   // ⬅️ Ignorar para que no cause conflicto
        public Conjuntos Conjunto_ { get; set; }
    }

}
