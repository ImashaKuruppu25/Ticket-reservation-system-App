/******************************************************************************
* File:     ReservationResponseDto.cs
* Brief:    This file contains the ReservationResponseDto class, which is a data
*           transfer object (DTO) representing the response data for a reservation
*           made in the Ticket Reservation System. It includes properties such as
*           departure and destination stations, reserved date, departure and arrival
*           times, user information, ticket number, passenger information, trip
*           duration, and type of reservation.
******************************************************************************/
using Ticket_reservation_system.Models;

namespace Ticket_reservation_system.Models.Dtos
{
    public class ReservationResponseDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateOnly ReservedDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public object User { get; set; }
        public string TicketNumber { get; set; }
        public PassengerInfo Passenger { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
    }
    public class PassengerInfo
    {
        public int Adult { get; set; }
        public int? Child { get; set; }
        public object Seat { get; set; } // Use object type for flexibility (number or number[])
        public string TravelClass { get; set; }
    }
}
