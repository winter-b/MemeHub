using MongoDB.Bson.Serialization.Attributes;

namespace WebApi.Models
{
    public class LoginRequest
    { 
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}