/******************************************************************************
* File:     LoginResponseDto.cs
* Brief:    This file contains the LoginResponseDto class, which is a data transfer
*           object (DTO) representing the response returned after a user logs in
*           to the Ticket Reservation System. It includes properties such as the
*           user's preferred name, NIC (National Identity Card), user ID, role,
*           email, active status, and an authentication token.
******************************************************************************/
namespace Ticket_reservation_system.Models.Dtos
{
    public class LoginResponseDto
    {
        public string PreferredName { get; set; }
        public string NIC { get; set; }
        public string UserID { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public Boolean Active { get; set; }
        public string Token { get; set; }
    }
}
