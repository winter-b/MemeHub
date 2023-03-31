using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    [BsonIgnoreExtraElements]
    public class UserInfo
    {
        [BsonId]
        public Guid Id;
        [BsonElement("Username")]
        public string UserName { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("VerificationCode")]
        public string VerificationCode { get; set; }
    }
}
