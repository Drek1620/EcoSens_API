namespace EcoSens_API.Models
{
    public class EmpleadoUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string TipoEmpleado { get; set; } = "";
        public string Area { get; set; } = "";
        public string Foto { get; set; } = "";

    }
}
