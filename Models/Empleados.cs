using System.ComponentModel.DataAnnotations;

namespace EcoSens_API.Models
{
    public class Empleados
    {
        public int Id { get; set; }

        [Required]
        public int Usuario_Id { get; set; }   // si siempre debe existir un usuario, mantenlo requerido

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        // si en SQL `Telefono` puede ser NULL → hazlo nullable
        public string? Telefono { get; set; }

        // este ya estaba bien
        public int? AreaId { get; set; }
        public string? Foto { get; set; }

        // navegación
        public Usuario? Usuario_ { get; set; }   // hazlo nullable si el join puede no existir
        public Area? Area { get; set; }          // igual aquí
    }

}
