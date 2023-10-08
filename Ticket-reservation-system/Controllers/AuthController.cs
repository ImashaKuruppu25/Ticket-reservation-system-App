using Ticket_reservation_system.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Ticket_reservation_system.Services;
using MongoDB.Driver;
using Ticket_reservation_system.Models.Dtos;

namespace Ticket_reservation_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly MongoDBService _mongoDBService;

        public AuthController(IConfiguration configuration, MongoDBService mongoDBService)
        {
            _configuration = configuration;
            _mongoDBService = mongoDBService;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
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
                PreferredName = request.PreferredName,
                NIC = request.NIC,
                Email = request.Email,
                HashedPassword = passwordHash,
                Role = request.Role

            };

            // Save the user to MongoDB
            var usersCollection = _mongoDBService.Users;
            usersCollection.InsertOne(user);

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<User> Login(LoginDto request)
        {
            var usersCollection = _mongoDBService.Users;
            var user = usersCollection.Find(u => u.NIC == request.NIC).FirstOrDefault();

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
            {
                return BadRequest("Password is wrong!");     
            }

            if (user.Active == false)
            {
                return BadRequest("User account deactivate!");
            }


            string token = CreateToken(user);

            var responseDto = new LoginResponseDto
            {
                PreferredName = user.PreferredName,
                NIC = user.NIC,
                UserID = user.Id,
                Role = user.Role,
                Token = token
            };

            return Ok(responseDto);
        }

        private string CreateToken(User user)
        {
            // Create claims (user identity and authorization role)
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, user.PreferredName),
                new Claim(ClaimTypes.PrimarySid , user.NIC),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // Create a security key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create a JWT token with claims, expiration, and signing credentials
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            // Serialize token to a string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
           
            return jwt;
        }
         

    }
}
