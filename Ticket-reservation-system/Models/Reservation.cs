/******************************************************************************
* File:     Reservation.cs
* Brief:    This file contains the Reservation class, which represents a reservation
*           in the Ticket Reservation System. It includes properties such as the user ID,
*           travel details, and reservation information.
******************************************************************************/
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
        public float TotalAmount { get; set; }

        [BsonElement("ReservedDate")]
        public DateOnly ReservedDate { get; set; }

        [BsonElement("Duration")]
        public string Duration { get; set; }
    }
}
