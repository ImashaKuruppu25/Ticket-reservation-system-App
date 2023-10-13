/******************************************************************************
* File:     TrainDto.cs
* Brief:    This file contains the TrainDto class, which is a data transfer
*           object (DTO) for representing details about a train in the Ticket Reservation
*           System. It includes properties such as the train's ID, name, and number.
******************************************************************************/
using MongoDB.Bson;

namespace Ticket_reservation_system.Models.Dtos
{
    public class TrainDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required int Number { get; set; }
        
    }
}
