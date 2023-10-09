﻿using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Identity;

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
                ReservedDate = request.departureDate,
                Duration=request.Duration
               
            };

            // Insert the new reservation into the Reservations collection
            reservationsCollection.InsertOne(newReservation);

            return Ok(newReservation);

        }

        [HttpGet("user-reservations-today-onwards")]
        [Authorize]
        public ActionResult<IEnumerable<Reservation>> GetUserReservationsTodayOnwards()
        {
            // Get the current date as a DateOnly object
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            string userId = User.FindFirstValue(ClaimTypes.Sid);

            // Define a filter to get reservations for the current user with a reserved date from today onwards
            var filter = Builders<Reservation>.Filter.And(
                Builders<Reservation>.Filter.Eq(r => r.UserId, userId), 
                Builders<Reservation>.Filter.Gte(r => r.ReservedDate, currentDate) // Filter by today onwards
            );

            // Retrieve the filtered reservations
            var userReservations = _mongoDBService.Reservation.Find(filter).ToList();

            return Ok(userReservations);
        }

        [HttpPost("Travel-agent-reservation")]
        //[Authorize (Roles = "Travel-agent")]
        public ActionResult CreateTravelerReservation(TravelerReservationsDto request)
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
            
            // Get the user ID from the NIC
            var usersCollection = _mongoDBService.Users;
            var user =  usersCollection.Find(u => u.NIC == request.NIC).FirstOrDefault();
           
            if (user == null)
            {
                return BadRequest("User not found with the provided NIC.");
            }

            // Check maximum 4 reservations per userID
            var reservationsCollection = _mongoDBService.Reservation;
            var userReservations = reservationsCollection.Find(r => r.UserId == user.Id).ToList();


            if (userReservations.Count >= 4)
            {
                return BadRequest("User have reached the maximum limit of 4 reservations.");
            }

            
            // Create a new reservation from the request  data
            var newReservation = new Reservation
            {
                TicketNo = Guid.NewGuid().ToString(),
                ScheduleId = request.ScheduleId,
                UserId = user.Id.ToString(),
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

        [HttpGet()]
        public ActionResult<ReservationResponseDto> GetReservationById(string reservationId)
        {
           
            // Find the reservation by ID in the Reservations collection
            var reservation = _mongoDBService.Reservation.Find(r => r.Id == reservationId).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            // Retrieve the associated schedule using ScheduleId
            var schedule = GetScheduleById(reservation.ScheduleId);

            if (schedule == null)
            {
                return BadRequest("Schedule not found.");
            }

            // Calculate the ArrivalTime based on destination times
            string arrivalTime = CalculateArrivalTime(schedule, reservation.To);

           
            // Create a ReservationResponseDto based on the reservation data
            var responseDto = new ReservationResponseDto
            {
                From = reservation.From,
                To = reservation.To,
                ReservedDate = reservation.ReservedDate,
                DepartureTime = schedule.DepartureTime,
                ArrivalTime = arrivalTime,
                User = GetUserById(reservation.UserId), // Implement GetUserById to fetch user details by ID
                TicketNumber = reservation.TicketNo,
                Duration = reservation.Duration,
                Passenger = new PassengerInfo
                {
                    Adult = reservation.Adults,
                    Child = reservation.Child,
                    Seat = reservation.Seat,
                    Class = reservation.Class
                }
            };

            return Ok(responseDto);
        }

        [HttpPatch("update-reservation")]
        public IActionResult UpdateReservation(string reservationId, ReservationUpdateDto request)
        {
            // Perform input validation here
            if (request == null)
            {
                return BadRequest("Invalid update data.");
            }

            // Find the reservation by ID
            var reservationsCollection = _mongoDBService.Reservation;
            var reservation = reservationsCollection.Find(r => r.Id == reservationId).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }


            // Update the reservation properties
            reservation.Class = request.Class;
            reservation.Adults = request.Adults;
            reservation.Child = request.Child;

            // Save the updated reservation
            reservationsCollection.ReplaceOne(r => r.Id == reservationId, reservation);

            return Ok(reservation); 
        }

        [HttpGet("reservations-history")]
        [Authorize]
        public ActionResult<IEnumerable<Reservation>> GetUserReservationsBeforeToday()
        {
            // Get the current date as a DateOnly object
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            string userId = User.FindFirstValue(ClaimTypes.Sid);

            // Define a filter to get reservations for the current user with a reserved date before today
            var filter = Builders<Reservation>.Filter.And(
                Builders<Reservation>.Filter.Eq(r => r.UserId, userId),
                Builders<Reservation>.Filter.Lt(r => r.ReservedDate, currentDate) // Filter by before today
            );

            // Retrieve the filtered reservations
            var userReservations = _mongoDBService.Reservation.Find(filter).ToList();

            return Ok(userReservations);
        }

        private object GetUserById(string userId)
        {
            // Implement logic to fetch user details by ID from the Users collection
            var user = _mongoDBService.Users.Find(u => u.Id == userId).FirstOrDefault();

            if (user == null)
            {
                return null; // User not found
            }

            var userData = new
            {
                _id = user.Id,
                name = user.PreferredName,
                NIC = user.NIC,
                role = user.Role,
                email = user.Email
            };

            return userData;
        }
        private Schedule GetScheduleById(string scheduleId)
        {
            // Implement logic to fetch the Schedule by ScheduleId from your collection
            var schedule = _mongoDBService.Schedules.Find(s => s.Id == scheduleId).FirstOrDefault();
            return schedule;
        }
        private string CalculateArrivalTime(Schedule schedule, string destination)
        {
            // Implement logic to calculate the ArrivalTime based on Schedule and destination
            var destinationInfo = schedule.Destinations.FirstOrDefault(d => d.Name == destination);
            return destinationInfo != null ? destinationInfo.ReachTime.ToString() : "";
        }

    }
}
