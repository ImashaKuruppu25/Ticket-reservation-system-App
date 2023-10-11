using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ticket_reservation_system.Models
{
    public class CanceledReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Reservation")]
        public Reservation Reservation { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }
    }
}
