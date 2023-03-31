using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApi.Models
{
    public class Hacks
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("Count")]
        public int Count { get; set; }
        [BsonElement("Date")]
        public string Date { get; set; }

    }
}