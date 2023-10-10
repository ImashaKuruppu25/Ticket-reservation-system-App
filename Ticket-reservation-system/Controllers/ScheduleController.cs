using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Ticket_reservation_system.Models;
using Ticket_reservation_system.Models.Dtos;
using Ticket_reservation_system.Services;

namespace Ticket_reservation_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        public static Schedule train = new Schedule();
        private readonly MongoDBService _mongoDBService;

        public ScheduleController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("new-schedule")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult CreateSchedule(ScheduleDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid schedule data.");
            }

            // Check if the associated train exists
            var trainsCollection = _mongoDBService.Trains;
            var trainFilter = Builders<Train>.Filter.Eq(t => t.Id, request.TrainId.ToString());
            var existingTrain = trainsCollection.Find(trainFilter).FirstOrDefault();

            if (existingTrain == null)
            {
                return BadRequest("The associated train does not exist.");
            }

            var destinations = request.Destinations.Select(destinationDto => new Destination
            {
                Name = destinationDto.Name,
                ReachTime = destinationDto.ReachTime,
                Price = destinationDto.Price
            }).ToList();

            // Create a new schedule from the request data
            var newSchedule = new Schedule
            {
                Type = request.Type,
                TrainId = request.TrainId,
                TrainName = request.TrainName,
                Status = request.Status,
                StartingStation = request.StartingStation,
                DepartureTime = request.DepartureTime,
                DepartureDate = request.DepartureDate,
                Destinations = destinations,
                AvailableTicketCount = request.AvailableTicketCount
            };

            // Insert the new schedule into the Schedules collection
            var schedulesCollection = _mongoDBService.Schedules;
            schedulesCollection.InsertOne(newSchedule);

            return Ok(newSchedule);
        }

        [HttpPut("{scheduleId}")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult UpdateSchedule(string scheduleId, ScheduleDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid schedule data.");
            }

            // Check if the specified schedule exists
            var schedulesCollection = _mongoDBService.Schedules;
            var scheduleFilter = Builders<Schedule>.Filter.Eq(s => s.Id, scheduleId);
            var existingSchedule = schedulesCollection.Find(scheduleFilter).FirstOrDefault();

            if (existingSchedule == null)
            {
                return NotFound("The specified schedule does not exist.");
            }

            // Check if the associated train exists
            var trainsCollection = _mongoDBService.Trains;
            var trainFilter = Builders<Train>.Filter.Eq(t => t.Id, request.TrainId.ToString());
            var existingTrain = trainsCollection.Find(trainFilter).FirstOrDefault();

            if (existingTrain == null)
            {
                return BadRequest("The associated train does not exist.");
            }

            var destinations = request.Destinations.Select(destinationDto => new Destination
            {
                Name = destinationDto.Name,
                ReachTime = destinationDto.ReachTime,
                Price = destinationDto.Price
            }).ToList();

            // Update the existing schedule with the request data
            existingSchedule.Type = request.Type;
            existingSchedule.TrainId = request.TrainId;
            existingSchedule.TrainName = request.TrainName;
            existingSchedule.Status = request.Status;
            existingSchedule.StartingStation = request.StartingStation;
            existingSchedule.DepartureTime = request.DepartureTime;
            existingSchedule.DepartureDate = request.DepartureDate;
            existingSchedule.Destinations = destinations;
            existingSchedule.AvailableTicketCount = request.AvailableTicketCount;

            // Replace the existing schedule with the updated schedule
            schedulesCollection.ReplaceOne(scheduleFilter, existingSchedule);

            return Ok(existingSchedule);
        }

        [HttpGet()]
        [Authorize(Roles = "Backoffice")]
        public ActionResult<IEnumerable<ScheduleDto>> GetAllSchedules([FromQuery] int currentPage = 1, [FromQuery] int limit = 10, [FromQuery] string searchTerm = null)
        {
            var schedulesCollection = _mongoDBService.Schedules;

            // Define a filter to search by schedule properties (modify as needed)
            var searchFilter = Builders<Schedule>.Filter.Or(
                Builders<Schedule>.Filter.Regex(s => s.TrainName, new MongoDB.Bson.BsonRegularExpression(searchTerm ?? "", "i")), // Case-insensitive search by Train name
                Builders<Schedule>.Filter.Regex(s => s.StartingStation, new MongoDB.Bson.BsonRegularExpression(searchTerm ?? "", "i")) // Case-insensitive search by StartingStation
            );

            // Get the total count of schedules matching the filter
            long totalCount = schedulesCollection.CountDocuments(searchFilter);

            // Calculate the total number of pages
            int totalPages = (int)((totalCount + limit - 1) / limit);

            // Ensure the requested page is within bounds
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            // Calculate skip and limit values for pagination
            int skip = (currentPage - 1) * limit;

            // Retrieve the list of schedules based on the filter, skip, and limit
            var schedules = schedulesCollection.Find(searchFilter)
                .Skip(Math.Max(0, skip))
                .Limit(limit)
                .ToList();

            
            // Map the Schedule objects to ScheduleDto objects using an anonymous type
            var scheduleDtos = schedules.Select(schedule => new AllScheculeDto
            {
                Id = schedule.Id,
                Type = schedule.Type,
                TrainId = schedule.TrainId,
                TrainName= schedule.TrainName,
                Status = schedule.Status,
                StartingStation = schedule.StartingStation,
                DepartureTime = schedule.DepartureTime,
                DepartureDate = schedule.DepartureDate,
                Destinations = schedule.Destinations.Select(destination => new DestinationDto
                {
                    Name = destination.Name,
                    ReachTime = destination.ReachTime,
                    Price = destination.Price
                }).ToList(),
                Duration= CalculateDuration(schedule.DepartureTime, schedule.Destinations.LastOrDefault()?.ReachTime ?? schedule.DepartureTime),
                AvailableTicketCount = schedule.AvailableTicketCount
            });

            // Return the list of schedules along with pagination information
            var result = new
            {
                Page = currentPage,
                PageSize = limit,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = scheduleDtos
            };

            return Ok(result);
        }

        [HttpGet("active-schedules")]
        public ActionResult<IEnumerable<ScheduleDto>> GetAllActiveSchedules()
        {
            var schedulesCollection = _mongoDBService.Schedules;

            // Define a filter to search for active schedules
            var filter = Builders<Schedule>.Filter.Eq(s => s.Status, "active");

            // Retrieve the list of active schedules based on the filter
            var activeSchedules = schedulesCollection.Find(filter).ToList();

            // Map the Schedule objects to ScheduleDto objects using an anonymous type
            var scheduleDtos = activeSchedules.Select(schedule => new AllScheculeDto
            {
                Id = schedule.Id,
                Type = schedule.Type,
                TrainId = schedule.TrainId,
                TrainName = schedule.TrainName,
                Status = schedule.Status,
                StartingStation = schedule.StartingStation,
                DepartureTime = schedule.DepartureTime,
                DepartureDate = schedule.DepartureDate,
                Destinations = schedule.Destinations.Select(destination => new DestinationDto
                {
                    Name = destination.Name,
                    ReachTime = destination.ReachTime,
                    Price = destination.Price
                }).ToList(),
                AvailableTicketCount = schedule.AvailableTicketCount
            });

            return Ok(scheduleDtos);
        }

        [HttpGet("get-details")]
        public IActionResult GetScheduleDetails([FromQuery] string from, [FromQuery] string to, [FromQuery] DateOnly date)
        {
            var schedulesCollection = _mongoDBService.Schedules;
            var filterBuilder = Builders<Schedule>.Filter;

            // Define a case-insensitive regex pattern for 'from' and 'to'
            var caseInsensitivePattern = new BsonRegularExpression(from, "i");

            // Define the filter to retrieve available schedules based on input parameters
            var filter = filterBuilder.And(
                filterBuilder.Eq(s => s.Status, "active"), // Filter for active schedules
                filterBuilder.Gte(s => s.DepartureDate, date), // Filter for schedules on or after the input date
                filterBuilder.Or(
                    filterBuilder.Regex(s => s.StartingStation, caseInsensitivePattern), // Case-insensitive regex for 'StartingStation'
                    filterBuilder.AnyEq(s => s.Destinations.Select(d => d.Name), from) // Filter for schedules with 'from' as a destination
                ),
                filterBuilder.Or(
                    filterBuilder.Regex(s => s.StartingStation, caseInsensitivePattern), // Case-insensitive regex for 'StartingStation'
                    filterBuilder.AnyEq(s => s.Destinations.Select(d => d.Name), to) // Filter for schedules with 'to' as a destination
                ),
                filterBuilder.Lt(s => s.DepartureDate, date.AddDays(1)) // Filter for schedules before the next day
            );

            var availableSchedules = schedulesCollection.Find(filter).ToList();

            // Filter schedules to find valid routes from 'from' to 'to'
            availableSchedules = availableSchedules.Where(s =>
                (s.StartingStation.Equals(from, StringComparison.OrdinalIgnoreCase) || s.Destinations.Any(d => d.Name.Equals(from, StringComparison.OrdinalIgnoreCase))) && // Starting from 'from' or has 'from' as a destination
                (s.StartingStation.Equals(to, StringComparison.OrdinalIgnoreCase) || s.Destinations.Any(d => d.Name.Equals(to, StringComparison.OrdinalIgnoreCase))) && // Starting from 'to' or has 'to' as a destination
                s.Destinations.FindIndex(d => d.Name.Equals(from, StringComparison.OrdinalIgnoreCase)) < s.Destinations.FindIndex(d => d.Name.Equals(to, StringComparison.OrdinalIgnoreCase)) // 'from' comes before 'to' in destinations
            ).ToList();

            if (availableSchedules.Count == 0)
            {
                return NotFound("No available schedules found.");
            }

            // Calculate total price based on destinations' prices
            var scheduleDetails = availableSchedules.Select(s =>
            {
                decimal scheduleTotalPrice = 0m;
                var destinationTime = s.Destinations.LastOrDefault().ReachTime;

                if (s.StartingStation.Equals(from, StringComparison.OrdinalIgnoreCase))
                {
                    // 'from' station is the starting station, find 'to' in destinations
                    var toDestination = s.Destinations.FirstOrDefault(d => d.Name.Equals(to, StringComparison.OrdinalIgnoreCase));
                    if (toDestination != null)
                    {
                        scheduleTotalPrice = toDestination.Price;
                        destinationTime = toDestination.ReachTime;
                    }
                }
                else
                {
                    // 'from' station is not the starting station
                    var fromDestination = s.Destinations.FirstOrDefault(d => d.Name.Equals(from, StringComparison.OrdinalIgnoreCase));
                    var toDestination = s.Destinations.FirstOrDefault(d => d.Name.Equals(to, StringComparison.OrdinalIgnoreCase));

                    if (fromDestination != null && toDestination != null)
                    {
                        // Reduce the 'to' price by the 'from' price
                        scheduleTotalPrice = toDestination.Price - fromDestination.Price;
                        destinationTime = toDestination.ReachTime;
                    }
                }

                // Calculate duration
                var duration = CalculateDuration(s.DepartureTime, destinationTime);


                return new
                {
                    Schedule = s,
                    TotalPrice = scheduleTotalPrice,
                    Duration = duration
                };
            }).ToList();

            return Ok(scheduleDetails);
        }

        [HttpGet("today-onward-schedules")]
        public ActionResult<IEnumerable<ScheduleResponseDto>> GetTodayOnwardSchedules([FromQuery] DateOnly date)
        {
            var schedulesCollection = _mongoDBService.Schedules;
            var trainsCollection = _mongoDBService.Trains;
            var filterBuilder = Builders<Schedule>.Filter;

            // Define the filter to retrieve active schedules with departure date on or after the input date
            var filter = filterBuilder.And(
                filterBuilder.Eq(s => s.Status, "active"),
                filterBuilder.Gte(s => s.DepartureDate, date)
            );

            var availableSchedules = schedulesCollection.Find(filter).ToList();
           
            // Create a list of ScheduleResponseDto objects based on the retrieved schedules
            var scheduleResponseDtos = availableSchedules.Select(schedule =>
            {
                var lastDestination = schedule.Destinations.LastOrDefault();
                var trainFilter = Builders<Train>.Filter.Eq(t => t.Id, schedule.TrainId);
                var train = trainsCollection.Find(trainFilter).FirstOrDefault();

                return new ScheduleResponseDto
                {
                    TrainId = schedule.TrainId,
                    TrainName = schedule.TrainName,
                    TrainNumber= train.Number,
                    ScheduleId = schedule.Id,
                    From = schedule.StartingStation,
                    To = lastDestination?.Name ?? schedule.StartingStation,
                    DepartureTime = schedule.DepartureTime.ToString(),
                    DepartureDate = schedule.DepartureDate.ToString(),
                    ArrivalTime = lastDestination?.ReachTime.ToString() ?? schedule.DepartureTime.ToString(),
                    Duration = CalculateDuration(schedule.DepartureTime, lastDestination?.ReachTime ?? schedule.DepartureTime),
                    Type = schedule.Type,
                    Availability = GetAvailabilityStatus(schedule.AvailableTicketCount),
                    Price = lastDestination.Price
                };
            }).ToList();

            return Ok(scheduleResponseDtos);
        }

        // Helper method to calculate duration
        private string CalculateDuration(TimeOnly departureTime, TimeOnly arrivalTime)
        {
            // Calculate the duration based on departure and arrival times
            var hours = arrivalTime.Hour - departureTime.Hour;
            var minutes = arrivalTime.Minute - departureTime.Minute;

            return $"{hours}h {minutes}m";
        }

        // Helper method to get availability status
        private string GetAvailabilityStatus(int availableTicketCount)
        {
            return availableTicketCount > 0 ? availableTicketCount.ToString() : "Sold Out";
        }
    }
}
