namespace EcoSens_API.Models
{
    public class ConjuntoConEstadosDTO
    {
        public int Id { get; set; }
        public string Mac_ESP32 { get; set; }
        public string Clavesecreta { get; set; }
        public string Area { get; set; }
        public int AreaId { get; set; }
        public string Contenedor_plastico { get; set; }
        public string Contenedor_metal { get; set; }
    }
}
