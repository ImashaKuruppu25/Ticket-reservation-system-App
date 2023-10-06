using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
