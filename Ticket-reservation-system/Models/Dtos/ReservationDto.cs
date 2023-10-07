﻿namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationDto
    {
        public required string TicketNo { get; set; }
        public required string UserId { get; set; }
        public required string From { get; set; }
        public required string To { get; set; }
        public required string ScheduleId { get; set; }
        public required int Adults { get; set; }
        public int? Child { get; set; }
        public required string Class { get; set; }
        public required List<int> Seat { get; set; }
        public required string TotalAmount { get; set; }
        public required DateTime ReservedDate { get; set; }
    }
}