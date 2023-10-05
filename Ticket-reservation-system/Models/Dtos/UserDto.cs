namespace Ticket_reservation_system.Models.Dtos
{
    public class UserDto
    {
        public required string UserName { get; set; }
        public required string NIC { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

    }
}
