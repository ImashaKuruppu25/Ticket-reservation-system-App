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
        public ActionResult<Train>CreateTrain(TrainDto request)
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

    }
}
