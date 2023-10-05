using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;

namespace Ticket_reservation_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly MongoDBService _mongoDBService;

        public UserController(IConfiguration configuration, MongoDBService mongoDBService)
        {
            _configuration = configuration;
            _mongoDBService = mongoDBService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult<User> CreateUser(UserDto request)
        {
            // Check if the NIC is exist
            var existingUser = _mongoDBService.Users.Find(u => u.NIC == request.NIC).FirstOrDefault();
            if (existingUser != null)
            {
                // NIC is already in use, return a conflict response or handle it as needed
                return Conflict("NIC is already taken.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                NIC = request.NIC,
                PreferredName = request.PreferredName,
                Email = request.Email,
                HashedPassword = passwordHash,
                Role = request.Role,
      
            };

            // Save the user to MongoDB
            var usersCollection = _mongoDBService.Users;
            usersCollection.InsertOne(user);

            return Ok(user);
        }

    }
}
