using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    [BsonIgnoreExtraElements]
    public class Meme
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("UploaderName")]
        public string UploaderName { get; set; }
        [BsonElement("PathToMeme")]
        public string PathToMeme { get; set; }
        [BsonElement("Caption")]
        public string Caption { get; set; }
        [BsonElement("MemeMessage")]
        public string MemeMessage { get; set; }
        [BsonElement("Likes")]
        public int Likes { get; set; }
        [BsonElement("Dislikes")]
        public int Dislikes { get; set; }
        [BsonElement("uploadedAt")]
        public string uploadedAt { get; set; }
    }
}
