using System.Collections.Generic;
using WebApi.Models;

namespace WebApi.Repositories
{
    public interface IAuthorizationRepository
    {
        string GetPasswordByUsername(string username);
        bool UsernameExists(string username);
        void CreateUserInfo(UserInfo request);
        UserInfo GetUserInfoByName(string name);
        void VerifyAccount(string name);
        void CleanVerifyTokens();
        List<UserInfo> GetUserData();
    }
}