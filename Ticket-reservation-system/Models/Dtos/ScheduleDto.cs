/******************************************************************************
* File:     ScheduleDto.cs
* Brief:    This file contains the ScheduleDto class, which represents a data
*           transfer object (DTO) for defining a schedule in the Ticket Reservation
*           System. It includes properties such as schedule type, associated train,
*           status, departure information, destinations, and available ticket count.
******************************************************************************/
using MongoDB.Bson;

namespace Ticket_reservation_system.Models.Dtos
{
    public class ScheduleDto
    {
        public required string Type { get; set; }
        public required string TrainId { get; set; }
        public required string Status { get; set; }
        public required string StartingStation { get; set; }
        public required TimeOnly DepartureTime { get; set; }
        public required DateOnly DepartureDate { get; set; }
        public List<DestinationDto> Destinations { get; set; }
        public required int AvailableTicketCount { get; set; }
    }

    /******************************************************************************
    * File:     DestinationDto.cs
    * Brief:    This file contains the DestinationDto class, which is a data transfer
    *           object (DTO) for specifying details about a destination in a schedule.
    *           It includes properties for the destination name, time of arrival, and
    *           the price associated with reaching that destination.
    ******************************************************************************/
    public class DestinationDto
    {
        public required string Name { get; set; }
        public required TimeOnly ReachTime { get; set; }
        public required decimal Price { get; set; }
    }
}