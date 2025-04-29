using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Notificaciones
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ContenedorId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public bool Leido { get; set; } = false;
    }
}
