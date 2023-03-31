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
    public class MemeController
    {
        private readonly IMemeService _memeService;
        public MemeController(IMemeService memeService)
        {
            _memeService = memeService;
        }
        [Route("memes")]    
        [HttpGet]
        public async Task<List<Meme>> GetMemes(int limit = 10, int offset = 0)
        {
            var memes = _memeService.GetMemes(limit, offset);
            return memes;
        }

        [Route("meme")]
        [HttpGet]
        public async Task<Meme> GetMeme(Guid id)
        {
            var meme = _memeService.GetMeme(id);
            return meme;
        }
        [Route("meme")]
        [HttpPost]
        public async Task<string> UploadMeme([FromForm]MemeRequest request)
        {
            var id = _memeService.UplaodMeme(request);
            return id;
        }
        [Route("meme")]
        [HttpPut]
        public async Task<Guid> UpdateMeme([FromForm] MemeUpdateRequest request)
        {
            var id = _memeService.UpdateMeme(request);
            return id;
        }
        [Route("meme")]
        [HttpDelete]
        public async Task DeleteMeme(string token, Guid id)
        {
            _memeService.DeleteMeme(token, id);
        }

        [Route("memeImage")]
        [HttpPost]
        public async Task UploadImage([FromForm]MemeImageRequest memeImageRequest, IFormFile image)
        {
            try
            {
                _memeService.UplaodImage(memeImageRequest, image);
            }
            catch (Exception e){
                Console.WriteLine($"Failed to upload message {e}");
            }
        }
    }
} 
