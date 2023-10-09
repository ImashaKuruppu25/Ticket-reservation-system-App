using Ticket_reservation_system.Models;

namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationResponseDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateOnly ReservedDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public object User { get; set; }
        public string TicketNumber { get; set; }
        public PassengerInfo Passenger { get; set; }
        public string Duration { get; set; }
    }
    public class PassengerInfo
    {
        public int Adult { get; set; }
        public int? Child { get; set; }
        public object Seat { get; set; } // Use object type for flexibility (number or number[])
        public string Class { get; set; }
    }
}
