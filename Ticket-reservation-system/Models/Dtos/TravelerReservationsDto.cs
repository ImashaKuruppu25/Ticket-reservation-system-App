/******************************************************************************
* File:     TravelerReservationsDto.cs
* Brief:    This file contains the TravelerReservationsDto class, which is a data
*           transfer object (DTO) for representing details about reservations made by travelers
*           in the Ticket Reservation System. It includes properties such as the schedule ID, NIC of the traveler,
*           departure details, the number of adults and children, travel class, seat information, total amount,
*           and the duration of the trip.
******************************************************************************/
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
        public required string TravelClass { get; set; }
        public required List<int> Seat { get; set; }
        public required float TotalAmount { get; set; }
        public required string Duration { get; set; }
    }
}
