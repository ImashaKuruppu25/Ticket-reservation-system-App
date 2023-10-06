using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using Ticket_reservation_system.Models;

namespace Ticket_reservation_system.Models
{
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; } // "express" or "slow"

        [BsonElement("TrainId")]
        public string TrainId { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // "active" or "inactive"

        [BsonElement("StartingStation")]
        public string StartingStation { get; set; }

        [BsonElement("DepartureTime")]
        public string DepartureTime { get; set; }

        [BsonElement("DepartureDate")]
        public string DepartureDate { get; set; }

        [BsonElement("Destinations")]
        public List<Destination> Destinations { get; set; }

        [BsonElement("AvailableTicketCount")]
        public int AvailableTicketCount { get; set; }
    }

    public class Destination
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("ReachTime")]
        public string ReachTime { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }
    }
}





