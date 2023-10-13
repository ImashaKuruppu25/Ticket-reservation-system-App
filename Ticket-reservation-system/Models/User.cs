/******************************************************************************
* File:     User.cs
* Brief:    This file contains the User class, which represents user information
*           in the Ticket Reservation System. The User class includes details
*           such as the user's NIC, preferred name, email, hashed password, role,
*           and active status.
******************************************************************************/
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Ticket_reservation_system.Models.Dtos;

namespace Ticket_reservation_system.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("NIC")]
        public string NIC { get; set; }

        [BsonElement("PreferredName")]
        public string PreferredName { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("HashedPassword")]
        public string HashedPassword { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        [BsonElement("Active")]
        public Boolean Active { get; set; } = true;

    }
}
