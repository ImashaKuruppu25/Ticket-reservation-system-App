/******************************************************************************
* File:     ScheduleResponseDto.cs
* Brief:    This file contains the ScheduleResponseDto class, which is a data transfer
*           object (DTO) for representing details about a schedule in the Ticket Reservation
*           System. It includes properties such as the associated train, departure and
*           arrival information, schedule type, available ticket count, and price.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class ScheduleResponseDto
    {
        public string TrainId { get; set; }
        public string TrainName { get; set; }
        public int TrainNumber { get; set; }
        public string ScheduleId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureTime { get; set; }
        public DateOnly DepartureDate { get; set; }
        public string ArrivalTime { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
        public int AvailableTicketCount { get; set; }
        public decimal Price { get; set; }
    }
}
