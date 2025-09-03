namespace EcoSens_API.Models
{
    public class ConjuntoConContenedoresDto
    {
        public string Mac_ESP32 { get; set; }
        public string Clavesecreta { get; set; }
        public int Area_Id { get; set; }

        public List<Contenedores> Contenedores { get; set; }
    }

}
