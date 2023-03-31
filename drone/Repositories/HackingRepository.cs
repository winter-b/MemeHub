using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class HackingRepository : IHackingRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Hacks> _hackCollection;
        public HackingRepository(IMongoClient mongoClient) {
            _mongoClient = mongoClient;
            _hackCollection = _mongoClient.GetDatabase("memeHub").GetCollection<Hacks>("hacks");
        }

        public void CreateHackEntry(string date)
        {
            Hacks hack = new Hacks() { Count = 0, Date = date };
            _hackCollection.InsertOne(hack);
        }

        public List<Hacks> GetHackAttempts()
        {
            var hack = _hackCollection.Find(x => true).ToList();
            return hack;
        }

        public bool GetHackByDate(string date)
        {
           return _hackCollection.Find(x => x.Date == date).Any();
        }

        public void Inc(string date)
        {
            var hack = _hackCollection.Find(x => x.Date == date).FirstOrDefault();
            hack.Count++;
            _hackCollection.ReplaceOne(x => x.Date == date, hack);
        }
    }
}
