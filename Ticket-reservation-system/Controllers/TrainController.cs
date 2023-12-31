﻿/******************************************************************************
* File:     TrainController.cs
* Brief:    This file contains the TrainController class, which manages trains
*           in the Ticket Reservation System. The TrainController enables the
*           creation, retrieval, modification, and deletion of train data. It
*           also provides endpoints to list all train names and numbers.
******************************************************************************/
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
    public class TrainController : ControllerBase
    {
        public static Train train = new Train();
        private readonly MongoDBService _mongoDBService;

        public TrainController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult<Train>CreateTrain(UpdateTrainDto request)
        {
            // Check if a train with the same number already exists
            var existingTrain = _mongoDBService.Trains.Find(train => train.Number == request.Number).FirstOrDefault();

            if (existingTrain != null)
            {
                return Conflict("A train with the same number already exists.");
            }

            var newTrain = new Train
            {
                Name = request.Name,
                Number = request.Number
                
            };

            var trainsCollection = _mongoDBService.Trains;
            trainsCollection.InsertOne(newTrain);

            return Ok(newTrain);
        }

        [HttpGet()]
        [Authorize(Roles = "Backoffice")]
        public ActionResult<IEnumerable<TrainDto>> GetAllTrains([FromQuery] int currentPage = 1, [FromQuery] int limit = 10, [FromQuery] string searchTerm = null)
        {
            var trainsCollection = _mongoDBService.Trains;

            // Validate currentPage and limit
            if (currentPage < 1)
            {
                currentPage = 1;
            }

            if (limit < 1)
            {
                limit = 10; // Set a default value if limit is invalid
            }

            // Define a filter to search by train name or description (modify as needed)
            var searchFilter = Builders<Train>.Filter.Or(
                Builders<Train>.Filter.Regex(t => t.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm ?? "", "i")) // Case-insensitive train name search
            );

            // Get the total count of trains matching the filter
            long totalCount = trainsCollection.CountDocuments(searchFilter);

            // Calculate the total number of pages
            int totalPages = (int)((totalCount + limit - 1) / limit);

            // Ensure the requested page is within bounds
            if (currentPage > totalPages)
            {
                currentPage = totalPages;
            }

            // Calculate skip value, ensuring it's non-negative
            int skip = (currentPage - 1) * limit;
            if (skip < 0)
            {
                skip = 0;
            }

            // Retrieve the list of trains based on the filter and limit
            var trains = trainsCollection.Find(searchFilter)
                .Skip(Math.Max(0, skip)) // Ensure skip is non-negative
                .Limit(limit)
                .ToList();

            // Map the Train objects to TrainDto objects using an anonymous type
            var trainDtos = trains.Select(train => new TrainDto
            {
                Id = train.Id.ToString(),
                Name = train.Name,
                Number = train.Number

            });

            // Return the list of trains along with pagination information
            var result = new
            {
                Page = currentPage,
                PageSize = limit,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = trainDtos
            };

            return Ok(result);
        }

        [HttpDelete("{trainID}")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult DeleteTrainByNumber(string trainID)
        {
            var trainsCollection = _mongoDBService.Trains;
            var schedulesCollection = _mongoDBService.Schedules;

            //check if the deleted train is associated with any schedule
            var scheduleFilter = Builders<Schedule>.Filter.Eq(s => s.TrainId, trainID);
            var scheduleCount = schedulesCollection.CountDocuments(scheduleFilter);

            if (scheduleCount > 0)
            {
                // If the train is associated with at least one schedule, roll back the deletion
                return BadRequest("Train is associated with at least one schedule and cannot be deleted.");
            }

            // Define a filter to find the train by its number
            var filter = Builders<Train>.Filter.Eq(t => t.Id, trainID);

            // Find and delete the train by its number
            var deleteResult = trainsCollection.DeleteOne(filter);

            if (deleteResult.DeletedCount == 0)
            {
                return NotFound("Train not found");
            }

            return Ok("Train deleted successfully");
        }

        [HttpPut("{trainID}")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult UpdateTrainByNumber(string trainID, [FromBody] UpdateTrainDto request)
        {
            var trainsCollection = _mongoDBService.Trains;

            // Define a filter to find the train by its number
            var filter = Builders<Train>.Filter.Eq(t => t.Id, trainID);

            // Find the train by its number
            var existingTrain = trainsCollection.Find(filter).FirstOrDefault();

            if (existingTrain == null)
            {
                return NotFound("Train not found");
            }

            // Update the train information based on the request
            existingTrain.Name = request.Name;
            existingTrain.Number = request.Number;

            // Perform the update operation
            var updateResult = trainsCollection.ReplaceOne(filter, existingTrain);

            if (updateResult.ModifiedCount == 0)
            {
                return BadRequest("Train update failed");
            }

            return Ok("Train updated successfully");
        }

        [HttpGet("all-names")]
        [Authorize(Roles = "Backoffice")]
        public ActionResult<IEnumerable<TrainDto>> GetAllTrainNamesAndNumbers()
        {
            var trainsCollection = _mongoDBService.Trains;

            // Retrieve all trains
            var trainDtos = trainsCollection.Find(_ => true)
                .ToList()
                .Select(train => new TrainDto
                {
                    Id = train.Id.ToString(),
                    Name = train.Name,
                    Number = train.Number
                })
                .ToList();

            return Ok(trainDtos);
        }
    }
}
