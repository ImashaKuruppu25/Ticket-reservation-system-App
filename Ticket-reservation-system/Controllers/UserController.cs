﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;
using System.Security.Claims;
using MongoDB.Bson;
using System.Text.RegularExpressions;

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

        // POST: api/users/create
        [HttpPost("create")]
        [Authorize(Roles = "Backoffice,TravelAgent")]
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

        // PUT: api/users/update/{nic}
        [HttpPut("update/{nic}")]
        [Authorize(Roles = "Backoffice,TravelAgent,Traveler")]
        public ActionResult UpdateUserByNIC(string nic, UserDto updatedData)
        {
            var usersCollection = _mongoDBService.Users;
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);

            // Check if the user exists
            var existingUser = usersCollection.Find(filter).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Update user details, e.g., username, NIC, and role
            existingUser.NIC = updatedData.NIC;
            existingUser.PreferredName = updatedData.PreferredName;
            existingUser.Email = updatedData.Email;
            existingUser.Role = updatedData.Role;

            usersCollection.ReplaceOne(filter, existingUser);

            return Ok("User updated");
        }

        // DELETE: api/users/delete/{nic}
        [HttpDelete("delete/{nic}")]
        [Authorize(Roles = "Backoffice,TravelAgent,Traveler")]
        public ActionResult DeleteUserByNIC(string nic)
        {
            var usersCollection = _mongoDBService.Users;
            var filter = Builders<User>.Filter.Eq(u => u.NIC, nic);

            // Check if the user exists
            var existingUser = usersCollection.Find(filter).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            usersCollection.DeleteOne(filter);

            return Ok("User deleted");
        }

        // GET: api/users/me
        [HttpGet("me")]
        [Authorize]
        public ActionResult<User> GetUser()
        {
            // Get the user's claims from the token
            var userNIC = User.FindFirst(ClaimTypes.PrimarySid);
            var userNameClaim = User.FindFirst(ClaimTypes.GivenName);
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);

            if (userNIC == null || userRoleClaim == null )
            {
                return BadRequest("Invalid token claims");
            }

            // Create a user object using the token claims
            var user = new User
            {
                NIC = userNIC.Value,
                PreferredName = userNameClaim.Value,
                Role = userRoleClaim.Value,
                Email = userEmailClaim.Value
               
            };

            return Ok(user);
        }

        // GET: api/users/travelers
        [HttpGet("travelers")]
        [Authorize(Roles = "Backoffice,TravelAgent")]
        public ActionResult<IEnumerable<UserDto>> GetAllTravelers([FromQuery] int currentPage = 1, [FromQuery] int limit = 10, [FromQuery] string searchTerm = null)
        {
            var usersCollection = _mongoDBService.Users;

            // Define a filter to search by NIC or username (modify as needed)
            var searchFilter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.NIC, new MongoDB.Bson.BsonRegularExpression(searchTerm ?? "", "i")), // Case-insensitive NIC search
                Builders<User>.Filter.Regex(u => u.PreferredName, new MongoDB.Bson.BsonRegularExpression(searchTerm ?? "", "i")) // Case-insensitive username search
            );

            // Define a filter to check the role is "Traveler"
            var roleFilter = Builders<User>.Filter.Eq(u => u.Role, "Traveler");

            // Combine the search and role filters using an AND operation
            var filter = Builders<User>.Filter.And(searchFilter, roleFilter);

            // Get the total count of users matching the filter
            long totalCount = usersCollection.CountDocuments(filter);

            // Calculate the total number of pages
            int totalPages = (int)((totalCount + limit - 1) / limit);

            // Ensure the requested page is within bounds
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            // Calculate skip and limit values for pagination
            int skip = (currentPage - 1) * limit;
            
            // Retrieve the list of travelers based on the filter, skip, and limit
            var travelers = usersCollection.Find(filter)
                .Skip(skip)
                .Limit(limit)
                .ToList();

            // Map the User objects to UserDto objects using an anonymous type
            var travelerDtos = travelers.Select(user => new
            {
                user.Id,
                user.NIC,
                user.PreferredName,
                user.Email,
                user.Role,
                user.Active
            });

            // Return the list of travelers along with pagination information
            var result = new
            {
                Page = currentPage,
                PageSize = limit,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = travelerDtos
            };

            return Ok(result);
        }
    }
}
