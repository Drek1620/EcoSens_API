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
        [BsonElement("peso")]
        public decimal Peso { get; set; }
        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }
        [BsonElement("hora")]
        public TimeOnly Hora { get; set; }
        [BsonElement("altura")]
        public string Altura { get; set; }
    }
}
