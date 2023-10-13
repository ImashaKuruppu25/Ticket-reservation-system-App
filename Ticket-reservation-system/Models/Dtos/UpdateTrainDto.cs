/******************************************************************************
* File:     UpdateTrainDto.cs
* Brief:    This file contains the UpdateTrainDto class, which is a data
*           transfer object (DTO) for updating details of a train in the Ticket Reservation System.
*           It includes properties such as the train's name and number.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class UpdateTrainDto
    {
        public required string Name { get; set; }
        public required int Number { get; set; }
    }
}
