using MongoDB.Bson;

namespace Ticket_reservation_system.Models.Dtos
{
    public class ScheduleDto
    {
        public required string Type { get; set; }
        public required string TrainId { get; set; }
        public required string TrainName { get; set; }
        public required string Status { get; set; }
        public required string StartingStation { get; set; }
        public required TimeOnly DepartureTime { get; set; }
        public required DateOnly DepartureDate { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        public required int AvailableTicketCount { get; set; }
    }

    public class DestinationDto
    {
        public required string Name { get; set; }
        public required TimeOnly ReachTime { get; set; }
        public required decimal Price { get; set; }
    }
}