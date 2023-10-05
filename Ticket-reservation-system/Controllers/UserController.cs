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

        // PUT: api/users/reactivate/{userId}
        [HttpPut("reactivate/{userId}")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult ReactivateUser(string userId)
        {
            var usersCollection = _mongoDBService.Users;
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);

            // Check if the user exists
            var existingUser = usersCollection.Find(filter).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Check if the user is already deactivated
            if (existingUser.Active)
            {
                return BadRequest("User is already activated");
            }

            var update = Builders<User>.Update.Set(u => u.Active, true);

            var updateResult = usersCollection.UpdateOne(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return NotFound("Something went wrong!");
            }

            return Ok("User reactivated");
        }

        // PUT: api/users/deactivate/{userId}
        [HttpPut("deactivate/{userId}")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult DeactivateUser(string userId)
        {
            var usersCollection = _mongoDBService.Users;
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);

            // Check if the user exists
            var existingUser = usersCollection.Find(filter).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Check if the user is already deactivated
            if (!existingUser.Active)
            {
                return BadRequest("User is already deactivated");
            }

            var update = Builders<User>.Update.Set(u => u.Active, false);

            var updateResult = usersCollection.UpdateOne(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                return NotFound("Something went wrong!");
            }

            return Ok("User deactivated");
        }

    }
}
