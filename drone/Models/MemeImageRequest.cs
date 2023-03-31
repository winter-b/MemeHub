using Microsoft.AspNetCore.Http;
using System;

namespace WebApi.Models
{
    public class MemeImageRequest
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
    }
}