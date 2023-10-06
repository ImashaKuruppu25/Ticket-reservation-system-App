using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ticket_reservation_system.Models
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Number")]
        public int Number { get; set; }

    }
}
