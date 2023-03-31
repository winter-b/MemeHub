using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class HackingController
    {
        private readonly IHackingService _hackingService;
        public HackingController(IHackingService hackingService)
        {
            _hackingService = hackingService;
        }
        [Route("hacks")]    
        [HttpGet]
        public async Task<string> GetHacks()
        {
            var hacks = _hackingService.GetHacks();
            return hacks;
        }
    }
} 
