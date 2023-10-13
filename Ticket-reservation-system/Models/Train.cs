/******************************************************************************
* File:     Train.cs
* Brief:    This file contains the Train class, which represents information about
*           trains in the Ticket Reservation System. The Train class includes
*           details such as the train's name and number.
******************************************************************************/
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ticket_reservation_system.Models
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Number")]
        public int Number { get; set; }

    }
}
