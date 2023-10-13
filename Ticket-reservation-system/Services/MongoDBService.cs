/******************************************************************************
* File:     MongoDBService.cs
* Brief:    This file contains the MongoDBService class, which serves as the
*           service for interacting with the MongoDB database in the Ticket
*           Reservation System. It provides access to various collections
*           such as Users, Trains, Schedules, Reservations, and CanceledReservations.
******************************************************************************/

using MongoDB.Driver;
using Ticket_reservation_system.Models;

namespace Ticket_reservation_system.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService(IConfiguration configuration) 
        {
            var connectionString = configuration.GetSection("MongoDBSettings:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoDBSettings:DatabaseName").Value;

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

         }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Train> Trains => _database.GetCollection<Train>("Trains");
        public IMongoCollection<Schedule> Schedules => _database.GetCollection<Schedule>("Schedules");
        public IMongoCollection<Reservation> Reservation => _database.GetCollection<Reservation>("Reservations");
        public IMongoCollection<CanceledReservation> CanceledReservations => _database.GetCollection<CanceledReservation>("CanceledReservations");
    }
}
