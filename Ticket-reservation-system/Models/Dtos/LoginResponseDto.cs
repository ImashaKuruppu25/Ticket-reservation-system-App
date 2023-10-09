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
