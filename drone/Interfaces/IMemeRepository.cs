using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IMemeRepository
    {
        public List<Meme> GetMemes(int limit, int offset);
        public Meme GetMeme(Guid id);
        public string UploadMeme(Meme request);
        public void UpdateImagePath(Meme request);
        void UpdateMeme(Meme newMeme);
        void DeleteMeme(Guid id);
    }
}
