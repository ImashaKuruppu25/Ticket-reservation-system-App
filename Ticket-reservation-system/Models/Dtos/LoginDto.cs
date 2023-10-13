/******************************************************************************
* File:     LoginDto.cs
* Brief:    This file contains the LoginDto class, which is a data transfer
*           object (DTO) used for handling user login information in the Ticket
*           Reservation System. It includes properties for the user's NIC (National
*           Identity Card) and password for authentication purposes.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class LoginDto
    {
        public string NIC { get; set; }
        public string Password { get; set; }
    }
}
