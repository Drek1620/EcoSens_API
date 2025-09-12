namespace EcoSens_API.Models
{
    public class ConjuntoConContenedoresDto
    {
        public int Id { get; set; }
        public string Mac_ESP32 { get; set; }
        public string Clavesecreta { get; set; }
        public int Area_Id { get; set; }

        public string Area { get; set; }
        public List<Contenedores> Contenedores { get; set; }
    }

}
