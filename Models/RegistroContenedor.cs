using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Bson;

namespace EcoSens_API.Models
{
    public class RegistroContenedor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("id_contenedor")]
        public int Id_contenedor { get; set; }
        [BsonElement("estado")]
        public string Estado { get; set; }
        [BsonElement("peso")]
        public double Peso { get; set; }
        [BsonElement("fecha")]
        public DateTime FechaYHora { get; set; }
        [BsonElement("altura")]
        public double Altura { get; set; }
    }
}
