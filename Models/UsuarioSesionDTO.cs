namespace EcoSens_API.Models
{
    public class UsuarioSesionDTO
    {
        public int TipoId { get; set; }

        public string Nombre { get; set; }
        public string Foto { get; set; }
        public int Notificaciones { get; set; }
    }
}
