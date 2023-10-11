namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationUpdateDto
    {

        public string TravelClass { get; set; }
        public int Adults { get; set; }
        public int? Child { get; set; }
        public string? ScheduleId { get; set; }
    }
}
