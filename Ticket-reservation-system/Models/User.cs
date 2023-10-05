using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ticket_reservation_system.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FirstName")]
        public string UserName { get; set; }

        [BsonElement("NIC")]
        public string NIC { get; set; }

        [BsonElement("HashedPassword")]
        public string HashedPassword { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }
       

    }
}
