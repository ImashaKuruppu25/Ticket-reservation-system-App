using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ticket_reservation_system.Models
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("TicketNo")]
        public string TicketNo { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("From")]
        public string From { get; set; }

        [BsonElement("To")]
        public string To { get; set; }

        [BsonElement("ScheduleId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ScheduleId { get; set; }

        [BsonElement("Adults")]
        public int Adults { get; set; }

        [BsonElement("Child")]
        public int? Child { get; set; }

        [BsonElement("Class")]
        public string Class { get; set; }

        [BsonElement("Seat")]
        public List<int> Seat { get; set; }

        [BsonElement("TotalAmount")]
        public string TotalAmount { get; set; }

        [BsonElement("ReservedDate")]
        public DateTime ReservedDate { get; set; }
    }
}
