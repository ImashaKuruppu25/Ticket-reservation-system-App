/******************************************************************************
* File:     AllScheduleDto.cs
* Brief:    This file contains the AllScheduleDto class, which represents a
*           data transfer object (DTO) for displaying information about a
*           schedule in the Ticket Reservation System. It includes details
*           about the schedule's ID, type, associated train, status, starting
*           station, departure time, departure date, destinations, duration,
*           and available ticket count.
******************************************************************************/
using MongoDB.Bson;

namespace Ticket_reservation_system.Models.Dtos
{
    public class AllScheculeDto
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required string TrainId { get; set; }
        public required string TrainName { get; set; }
        public required string Status { get; set; }
        public required string StartingStation { get; set; }
        public required TimeOnly DepartureTime { get; set; }
        public required DateOnly DepartureDate { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        public string Duration { get; set; }
        public required int AvailableTicketCount { get; set; }
    }

}