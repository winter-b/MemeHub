using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class MemeUpdateRequest
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string PathToMeme { get; set; }
        public string Caption { get; set; }
        public string MemeMessage { get; set; }
    }
}
