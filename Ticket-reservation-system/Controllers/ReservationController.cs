using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;

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
            if ((request.ReservedDate - DateTime.Now).TotalDays > 30)
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
                UserId = request.UserId,
                From = request.From,
                To = request.To,
                ScheduleId = request.ScheduleId,
                Adults = request.Adults,
                Child = request.Child,
                Class = request.Class,
                Seat = new List<int>(), // seat selection logic here
                TotalAmount = "0", // Calculate total amount
                ReservedDate = DateTime.Now
            };

            // Insert the new reservation into the Reservations collection
            reservationsCollection.InsertOne(newReservation);

            return Ok(newReservation);

        }
       
    }
}
