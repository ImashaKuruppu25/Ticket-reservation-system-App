namespace Ticket_reservation_system.Models.Dtos
{
    public class TravelerReservationsDto
    {
        public required string ScheduleId { get; set; }
        public required string NIC { get; set; }
        public required string From { get; set; }
        public required string To { get; set; }
        public DateOnly departureDate { get; set; }
        public required int Adults { get; set; }
        public int? Child { get; set; }
        public required string Class { get; set; }
        public required List<int> Seat { get; set; }
        public required float TotalAmount { get; set; }
    }
}
