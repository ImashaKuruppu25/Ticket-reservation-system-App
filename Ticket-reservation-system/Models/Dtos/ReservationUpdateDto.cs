/******************************************************************************
* File:     ReservationUpdateDto.cs
* Brief:    This file contains the ReservationUpdateDto class, which is a data
*           transfer object (DTO) representing the data required to update a
*           reservation in the Ticket Reservation System. It includes properties
*           for specifying the travel class, the number of adults and children,
*           and an optional schedule ID for the reservation update operation.
******************************************************************************/
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
