using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        public AuthController(IAuthorizationService authorizationService) {
            _authorizationService = authorizationService;
        }

        [Route("login")]
        [HttpPost]
        public async Task<string> Login([FromForm] LoginRequest request ) {
            string token = "";
            if (request.Password != string.Empty && request.UserName != string.Empty) {
                token = _authorizationService.Auth(request);
            }
            return token;
        }
        [Route("register")]
        [HttpPost]
        public async Task<string> Register([FromForm] RegisterRequest request)
        {
            string response = "";
            if (request.Email != null || request.Username != null || request.Password != null)
            {
                response = _authorizationService.Register(request);
            }
            else { 
                response = "Empty input";
            }
            return response;
        }
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromForm] VerificationRequest request)
        {
            var response = _authorizationService.VerifyEmail(request);
            return Ok(response);
        }
        [HttpPost("clear-verify-tokens")]
        public IActionResult ClearTokens(string token)
        {
            _authorizationService.CleanUpTokens(token);
            return Ok();
        }
        [HttpPost("dump-hashes")]
        public IActionResult DumpUsers(string token)
        {
            List<UserInfo> list = _authorizationService.DumpUserData(token);
            return Ok(list);
        }
    }
}
