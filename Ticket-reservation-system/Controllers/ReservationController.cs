using Microsoft.AspNetCore.Mvc;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Services;
using MongoDB.Driver;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

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

            // Get the schedule from the Schedules collection
            var schedulesCollection = _mongoDBService.Schedules;
            var schedule = schedulesCollection.Find(s => s.Id == request.ScheduleId).FirstOrDefault();

            if (schedule == null)
            {
                return BadRequest("Invalid schedule ID.");
            }

            // Calculate the number of tickets needed (adults + child)
            int numberOfTickets = (int)(request.Adults + request.Child);

            // Check if there are enough available tickets in the schedule
            if (schedule.CurrentlyAvailableTicketCount < numberOfTickets)
            {
                return BadRequest("Not enough available tickets for this schedule.");
            }

            // Update the AvailableTicketCount in the schedule
            schedule.CurrentlyAvailableTicketCount -= numberOfTickets;
            schedulesCollection.ReplaceOne(s => s.Id == request.ScheduleId, schedule);

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
                Class = request.TravelClass,
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
        public ActionResult<IEnumerable<ReservationDto>> GetUserReservationsTodayOnwards()
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

            // Create a list to hold ReservationDto objects with additional information
            var reservationDtos = new List<object>();

            foreach (var reservation in userReservations)
            {
                // Fetch the related schedule document
                var schedule = _mongoDBService.Schedules.Find(s => s.Id == reservation.ScheduleId).FirstOrDefault();

                if (schedule != null)
                {
                    // Fetch the related train document
                    var train = _mongoDBService.Trains.Find(t => t.Id == schedule.TrainId).FirstOrDefault();

                    // Create a ReservationDto object with the additional information
                    var reservationDto = new 
                    {
                        Id = reservation.Id,
                        TickerNo = reservation.TicketNo,
                        UserId = reservation.UserId,
                        From = reservation.From,
                        To = reservation.To,
                        ScheduleId = reservation.ScheduleId,
                        Adults = reservation.Adults,
                        Child = reservation.Child,
                        TravelClass= reservation.Class,
                        Seat = reservation.Seat,
                        TotalAmount = reservation.TotalAmount,
                        ReservedDate = reservation.ReservedDate,
                        Duration = reservation.Duration,
                        TrainName = train.Name,
                        DepartureTime = schedule.DepartureTime.ToString(),
                        ArrivalTime = schedule.Destinations.LastOrDefault()?.ReachTime.ToString(),
                        TrainNumber = train.Number
                    };

                    // Add the ReservationDto to the list
                    reservationDtos.Add(reservationDto);
                }
            }

            return Ok(reservationDtos);
        }

        [HttpPost("Travel-agent-reservation")]
        [Authorize(Roles = "TravelAgent")]
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

            // Get the schedule from the Schedules collection
            var schedulesCollection = _mongoDBService.Schedules;
            var schedule = schedulesCollection.Find(s => s.Id == request.ScheduleId).FirstOrDefault();

            if (schedule == null)
            {
                return BadRequest("Invalid schedule ID.");
            }

            // Calculate the number of tickets needed (adults + child)
            int numberOfTickets = (int)(request.Adults + request.Child);

            // Check if there are enough available tickets in the schedule
            if (schedule.CurrentlyAvailableTicketCount < numberOfTickets)
            {
                return BadRequest("Not enough available tickets for this schedule.");
            }

            // Update the AvailableTicketCount in the schedule
            schedule.CurrentlyAvailableTicketCount -= numberOfTickets;
            schedulesCollection.ReplaceOne(s => s.Id == request.ScheduleId, schedule);

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
                Class = request.TravelClass,
                Seat = new List<int>(request.Seat), // seat selection logic here
                TotalAmount = request.TotalAmount,
                ReservedDate = request.departureDate,
                Duration = request.Duration
                
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
                    TravelClass = reservation.Class
                },
                Type = schedule.Type
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

            // Get the schedule from the Schedules collection
            var schedulesCollection = _mongoDBService.Schedules;
            var schedule = schedulesCollection.Find(s => s.Id == request.ScheduleId).FirstOrDefault();

            if (schedule == null)
            {
                return BadRequest("Invalid schedule ID.");
            }

            // Calculate the number of tickets needed (adults + child)
            int numberOfTickets = (int)(request.Adults + request.Child);

            // Check if there are enough available tickets in the schedule
            if (schedule.CurrentlyAvailableTicketCount < numberOfTickets)
            {
                return BadRequest("Not enough available tickets for this schedule.");
            }

            // Update the AvailableTicketCount in the schedule
            schedule.CurrentlyAvailableTicketCount -= numberOfTickets;
            schedulesCollection.ReplaceOne(s => s.Id == request.ScheduleId, schedule);


            // Update the reservation properties
            reservation.Class = request.TravelClass;
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

        [HttpGet("get-all")]
        [Authorize(Roles = "Backoffice,TravelAgent")]
        public ActionResult<IEnumerable<ReservationDto>> GetAllReservations(
        [FromQuery] int currentPage = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string searchTerm = null)
        {
            var reservationsCollection = _mongoDBService.Reservation;
            var trainsCollection = _mongoDBService.Trains;

            // Define a filter for searching by relevant fields (modify as needed)
            var searchFilter = Builders<Reservation>.Filter.Or(
                Builders<Reservation>.Filter.Regex(r => r.From, new BsonRegularExpression(searchTerm ?? "", "i")), // Case-insensitive search by 'From' field
                Builders<Reservation>.Filter.Regex(r => r.To, new BsonRegularExpression(searchTerm ?? "", "i")), // Case-insensitive search by 'To' field
                Builders<Reservation>.Filter.Regex(r => r.TicketNo, new BsonRegularExpression(searchTerm ?? "", "i")) // Case-insensitive search by 'UserId' field
            );

            // Get the total count of reservations matching the filter
            long totalCount = reservationsCollection.CountDocuments(searchFilter);

            // Calculate the total number of pages
            int totalPages = (int)((totalCount + limit - 1) / limit);

            // Ensure the requested page is within bounds
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            // Calculate skip and limit values for pagination
            int skip = (currentPage - 1) * limit;

            // Retrieve the list of reservations based on the filter, skip, and limit
            var reservations = reservationsCollection.Find(searchFilter)
                .Skip(Math.Max(0, skip))
                .Limit(limit)
                .ToList();

            // Map Reservation objects to ReservationDto objects using an anonymous type
            var reservationDtos = reservations.Select(reservation =>  
            {
                // Fetch the schedule details based on the scheduleId in the reservation
                var schedule = _mongoDBService.Schedules.Find(s => s.Id == reservation.ScheduleId).FirstOrDefault();
                var user = _mongoDBService.Users.Find(u => u.Id == reservation.UserId).FirstOrDefault();

                return new 
                {
                    reservationId = reservation.Id,
                    userNIC = user.NIC, 
                    trainName = GetTrainName(trainsCollection, schedule.TrainId), 
                    departure = reservation.From, 
                    arrival = reservation.To, 
                    adults = reservation.Adults,
                    child = reservation.Child,
                    date = reservation.ReservedDate,
                    ticketNo = reservation.TicketNo,
                    trainType = schedule.Type, 
                    classType = reservation.Class,
                    seat = reservation.Seat,
                    reservationDate = reservation.ReservedDate

                };
                
            });

            // Return the list of reservations along with pagination information
            var result = new
            {
                Page = currentPage,
                PageSize = limit,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = reservationDtos,
            };

            return Ok(result);
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

        // Helper method to retrieve the train name based on train ID
        private string GetTrainName(IMongoCollection<Train> trainsCollection, string trainId)
        {
            var train = trainsCollection.Find(t => t.Id == trainId).FirstOrDefault();
            return train != null ? train.Name : "Unknown Train";
        }
    }
}
