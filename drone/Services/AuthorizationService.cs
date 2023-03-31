using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IHackingService _hackingService;
        private readonly IEmailService _emailService;
        private string SaltySalt;
        private string PrivateKey;
        private string VerificationPrivateKey;
        private Random randomNumber = new Random(); //Super secret code

        public AuthorizationService(IAuthorizationRepository authorizationRepository, IEmailService emailService, IHackingService hackingService, string salt, string key, string veriKey) {
            _authorizationRepository = authorizationRepository;
            PrivateKey = key;
            VerificationPrivateKey = veriKey;
            SaltySalt = salt;
            _emailService = emailService;
            _hackingService = hackingService;
        }

        public string Auth(LoginRequest request)
        {
            string token = "";
            try
            {
                var securePassword = GenerateSaltedHash(Encoding.ASCII.GetBytes(request.Password), Encoding.ASCII.GetBytes(SaltySalt));
                var userInfo = _authorizationRepository.GetUserInfoByName(request.UserName);
                Console.WriteLine(Encoding.ASCII.GetString(securePassword));
                var sep = Encoding.ASCII.GetString(securePassword);
                if (userInfo.Password == Encoding.ASCII.GetString(securePassword) && userInfo.VerificationCode == "")
                {
                    token = GenerateToken(request.UserName, PrivateKey);
                }
            }
            catch(Exception e) {
                _hackingService.UpsertHackEntry();
            }
            return token;
        }

          
        public string Register(RegisterRequest request)
        {
            string response = "";
            try
            {
                if (!_authorizationRepository.UsernameExists(request.Username))
                {
                    var userInfo = new UserInfo() { UserName = request.Username, Password = request.Password, Email = request.Email };
                    var securePassword = GenerateSaltedHash(Encoding.ASCII.GetBytes(request.Password), Encoding.ASCII.GetBytes(SaltySalt));
                    userInfo.Password = Encoding.ASCII.GetString(securePassword);
                    var code = randomNumber.Next(1000000, 10000000);
                    userInfo.VerificationCode = code.ToString();
                    sendVerificationEmail(request.Username, request.Email, code.ToString());
                    _authorizationRepository.CreateUserInfo(userInfo);
                    response = "Check mail ;)";
                }
                else
                {
                    response = "Username already exists!";
                }
            }
            catch(Exception er) {
                _hackingService.UpsertHackEntry();
                response = "Kazka jau cia himicini ( IP logged >:( )"; // not really
            }
            return response;
        }

        public string Verify(VerificationRequest request)
        {
            string response = "";
            if (request?.Name != "" && request?.VerifyCode != "")
            {
                try
                {
                    if (_authorizationRepository.UsernameExists(request.Name))
                    {

                        var user = _authorizationRepository.GetUserInfoByName(request.Name);

                        if (user.VerificationCode == request.VerifyCode)
                        {
                            _authorizationRepository.VerifyAccount(request.Name);
                            response = "Nice! You can now login ;)";
                        }
                        else
                        {
                            response = "Incorrect token!";
                        }
                    }
                    else {
                        response = "Incorrect token!";
                    }
                }
                catch(Exception e) //Quadruples!
                {
                    _hackingService.UpsertHackEntry();
                    Console.WriteLine(e);
                    response = "Incorrect token!";
                }

            }
            else
            {
                response = "Incorrect token!";
            }
            return response;
        }

        private void sendVerificationEmail(string username, string email, string code)
        {
            var message = $@"
                             <p>Please use the below token to verify your email address with the <code>/v1/Auth/verify</code> api route:</p>
                             <p><code>{code}</code></p>";
            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h1>Ey {username}<h1>
                         {message}"
            );
        }

        public static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
        public static string GenerateToken(string username, string key, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(key);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username)
                }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public string VerifyEmail(VerificationRequest request)
        {
            return Verify(request);
        }

        public void CleanUpTokens(string token)
        {
            if (token == VerificationPrivateKey)
            {
                _authorizationRepository.CleanVerifyTokens();
            }
        }

        public List<UserInfo> DumpUserData(string token)
        {
            List<UserInfo> list = new List<UserInfo>();
            if (token == VerificationPrivateKey)
            {
                list = _authorizationRepository.GetUserData();
            }
            else {
                _hackingService.UpsertHackEntry();
            }
            return list;
        }
    }
}
