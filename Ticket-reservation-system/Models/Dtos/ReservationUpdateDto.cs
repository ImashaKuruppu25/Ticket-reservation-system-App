namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationUpdateDto
    {
        public string Class { get; set; }
        public int Adults { get; set; }
        public int? Child { get; set; }
    }
}
