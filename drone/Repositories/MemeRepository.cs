using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class MemeRepository : IMemeRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Meme> _memeCollection;

        public MemeRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _memeCollection = _mongoClient.GetDatabase("memeHub").GetCollection<Meme>("memes");
        }
        public List<Meme> GetMemes(int limit, int offest) {
            var memes = _memeCollection.Find(x => true).Limit(limit).Skip(offest);
            return memes.ToList();
        }
        public Meme GetMeme(Guid id) {
            var meme = _memeCollection.Find(x => x.Id == id).FirstOrDefault();
            return meme;
        }

        public string UploadMeme(Meme request)
        {
            _memeCollection.InsertOne(request);
            var meme = _memeCollection.Find(x => x.Id == request.Id).FirstOrDefault();
            return meme.Id.ToString();
        }
        public void UpdateImagePath(Meme request)
        {
            var meme = _memeCollection.Find(x => x.UploaderName == request.UploaderName && x.Id == request.Id).FirstOrDefault();
            meme.PathToMeme = request.PathToMeme;
            _memeCollection.ReplaceOne(x => x.Id == meme.Id, meme);
        }

        public void UpdateMeme(Meme newMeme)
        {
            _memeCollection.ReplaceOne(x => x.Id == newMeme.Id, newMeme);
        }

        public void DeleteMeme(Guid id)
        {
            _memeCollection.DeleteOne(x => x.Id == id);
        }
    }
}
