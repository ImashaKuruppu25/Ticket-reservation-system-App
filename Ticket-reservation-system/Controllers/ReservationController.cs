using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ticket_reservation_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public ReservationController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("traveler-reservation")]
        public ActionResult CreateReservation(ReservationDto request)
        {
            // Perform input validation here
            if (request == null)
            {
                return BadRequest("Invalid reservation data.");
            }

            // Check reservation date within 30 days from the booking date
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now); // Current date
            DateOnly thirtyDaysFromNow = currentDate.AddDays(30);


            if (request.departureDate > thirtyDaysFromNow)
            {
                return BadRequest("Reservation date should be within 30 days from the booking date.");
            }

            // Check maximum 4 reservations per userID
            var reservationsCollection = _mongoDBService.Reservation;
            var userReservations = reservationsCollection.Find(r => r.UserId == request.UserId).ToList();


            if (userReservations.Count >= 4)
            {
                return BadRequest("You have reached the maximum limit of 4 reservations.");
            }

            // Create a new reservation from the request data
            var newReservation = new Reservation
            {
                TicketNo = Guid.NewGuid().ToString(),
                ScheduleId = request.ScheduleId,
                UserId = request.UserId,
                From = request.From,
                To = request.To,
                Adults = request.Adults,
                Child = request.Child,
                Class = request.Class,
                Seat = new List<int>(request.Seat), // seat selection logic here
                TotalAmount = request.TotalAmount,
                ReservedDate = request.departureDate
            };

            // Insert the new reservation into the Reservations collection
            reservationsCollection.InsertOne(newReservation);

            return Ok(newReservation);

        }

        [HttpGet("user-reservations-today-onwards")]
        public ActionResult<IEnumerable<Reservation>> GetUserReservationsTodayOnwards()
        {
            // Get the current date as a DateOnly object
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);

            // Define a filter to get reservations for the current user with a reserved date from today onwards
            var filter = Builders<Reservation>.Filter.And(
                Builders<Reservation>.Filter.Eq(r => r.UserId, User.FindFirstValue(ClaimTypes.PrimarySid)), // Filter by user
                Builders<Reservation>.Filter.Gte(r => r.ReservedDate, currentDate) // Filter by today onwards
            );

            System.Diagnostics.Debug.WriteLine(filter);
            // Retrieve the filtered reservations
            var userReservations = _mongoDBService.Reservation.Find(filter).ToList();

            return Ok(userReservations);
        }
    }
}
