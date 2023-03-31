using System.Collections.Generic;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IAuthorizationService
    {
        string Auth(LoginRequest request);
        string Register(RegisterRequest request);
        string VerifyEmail(VerificationRequest request);
        void CleanUpTokens(string token);
        List<UserInfo> DumpUserData(string token);
    }
}