/******************************************************************************
* File:     ReservationDto.cs
* Brief:    This file contains the ReservationDto class, which is a data transfer
*           object (DTO) representing the details of a reservation made in the
*           Ticket Reservation System. It includes properties such as the schedule ID,
*           user ID, departure and destination stations, departure date, number of
*           adults and children, travel class, reserved seats, total amount, and
*           duration of the trip.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationDto
    {

        public required string ScheduleId { get; set; }
        public required string UserId { get; set; }
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
