using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IMemeService
    {
        List<Meme> GetMemes(int limit, int offest);
        Meme GetMeme(Guid id);
        string UplaodMeme(MemeRequest request);
        void UplaodImage(MemeImageRequest memeImageRequest, IFormFile image);
        Guid UpdateMeme(MemeUpdateRequest request);
        void DeleteMeme(string token, Guid id);
    }
}
