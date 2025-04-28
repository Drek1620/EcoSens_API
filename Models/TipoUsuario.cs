using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class TipoUsuario
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
