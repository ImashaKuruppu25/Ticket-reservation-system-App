/******************************************************************************
* File:     Schedule.cs
* Brief:    This file contains the Schedule and Destination classes, which represent
*           schedule information in the Ticket Reservation System. The Schedule class
*           contains details about train schedules, including type, status, departure
*           information, and available tickets. The Destination class represents
*           information about destinations within a schedule, including name, reach time,
*           and price.
******************************************************************************/
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
        [BsonRepresentation(BsonType.ObjectId)]
        public string TrainId { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } // "active" or "inactive"

        [BsonElement("StartingStation")]
        public string StartingStation { get; set; }

        [BsonElement("DepartureTime")]
        public TimeOnly DepartureTime { get; set; }

        [BsonElement("DepartureDate")]
        public DateOnly DepartureDate { get; set; }

        [BsonElement("Destinations")]
        public List<Destination> Destinations { get; set; }

        [BsonElement("AvailableTicketCount")]
        public int AvailableTicketCount { get; set; }

        [BsonElement("CurrentlyAvailableTicketCount")]
        public int CurrentlyAvailableTicketCount { get; set; }
    }

    public class Destination
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("ReachTime")]
        public TimeOnly ReachTime { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }
    }
}





