using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<UserInfo> _userCollection;

        public AuthorizationRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _userCollection = _mongoClient.GetDatabase("memeHub").GetCollection<UserInfo>("user");
        }

        public string GetPasswordByUsername(string username)
        {
            var exists = UsernameExists(username);
            if (exists)
            {
                var result = _userCollection.Find(x => x.UserName == username).FirstOrDefault();
                return result.Password;
            }            
            return "";
        }

        public bool UsernameExists(string username)
        {
            var exists = _userCollection.Find(x => x.UserName == username).Any();
            if (exists) {
                return true;
            }
            return false;
        }

        public void CreateUserInfo(UserInfo request)
        {
            _userCollection.InsertOne(request);
        }

        public UserInfo GetUserInfoByName(string name)
        {
            return _userCollection.Find(x => x.UserName == name).FirstOrDefault();
        }

        public void VerifyAccount(string name)
        {
            var current = GetUserInfoByName(name);
            current.VerificationCode = "";
            UpdateUserInfo(current);
        }

        private void UpdateUserInfo(UserInfo current)
        {
            _userCollection.ReplaceOne(x=>x.UserName == current.UserName, current);
        }

        public void CleanVerifyTokens()
        {
            _userCollection.DeleteMany(x => x.VerificationCode != "");
        }

        public List<UserInfo> GetUserData()
        {
            return _userCollection.Find(x => true).ToList();
        }
    }
}
