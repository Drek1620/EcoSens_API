namespace EcoSens_API.Models
{
    public class UsuarioEmpleadoDatos
    {
        public int UsuarioId { get; set; }
        // Usuario
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int TipoId { get; set; }

        // Empleado
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public int AreaId { get; set; }
        public string Foto { get; set; }
    }
}
