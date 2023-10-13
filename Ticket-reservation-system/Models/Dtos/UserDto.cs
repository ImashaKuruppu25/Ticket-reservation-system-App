/******************************************************************************
* File:     UserDto.cs
* Brief:    This file contains the UserDto class, which is a data transfer object (DTO)
*           representing user information for registration in the Ticket Reservation System.
*           It includes properties such as the NIC, preferred name, email, password, and role.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class UserDto
    {
        public required string NIC { get; set; }
        public required string PreferredName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

    }
}
