﻿using MongoDB.Bson;

namespace Ticket_reservation_system.Models.Dtos
{
    public class TrainDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required int Number { get; set; }
        
    }
}
