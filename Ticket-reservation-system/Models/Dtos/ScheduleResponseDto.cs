namespace Ticket_reservation_system.Models.Dtos
{
    public class ScheduleResponseDto
    {
        public string TrainId { get; set; }
        public string ScheduleId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
        public string Availability { get; set; }
    }
}
