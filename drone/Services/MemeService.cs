using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class MemeService : IMemeService
    {
        private readonly IMemeRepository _memeRepository;
        private readonly IHackingService _hackingService;
        private string _key;
        private string _JWTkey;

        public MemeService(IMemeRepository memeRepository, IHackingService hackingService, string key, string jwtKey)
        {
            _memeRepository = memeRepository;
            _key = key;
            _JWTkey = jwtKey;
            _hackingService = hackingService;
        }
        public List<Meme> GetMemes(int limit, int offest)
        {
            if (offest < 0) {
                offest = offest * -1;
            }
            var memes = _memeRepository.GetMemes(limit, offest);
            return memes;
        }

        public Meme GetMeme(Guid id)
        {
            return _memeRepository.GetMeme(id);
        }

        public string UplaodMeme(MemeRequest request)
        {
            try
            {
                ValidateToken(request.Token);
                var name = GetClaimFromToken(request.Token);
                var memeToUplaod = new Meme()
                {
                    UploaderName = name,
                    Caption = request.Caption,
                    PathToMeme = request.PathToMeme,
                    MemeMessage = request.MemeMessage,
                    Dislikes = 0,
                    Likes = 0,
                    uploadedAt = DateTime.Now.Date.ToShortDateString()
                };
                return _memeRepository.UploadMeme(memeToUplaod);
            }
            catch(Exception e) {
                Console.WriteLine(e);
                _hackingService.UpsertHackEntry();
                return "IP logged >:(";
            }
        }
        public async void UplaodImage(MemeImageRequest requestas, IFormFile image)
        {
            try
            {
                ValidateToken(requestas.Token);
                var name = GetClaimFromToken(requestas.Token);
                byte[] fileBytes;
                using var ms = new MemoryStream();
                image.CopyTo(ms);
                fileBytes = ms.ToArray();

                string base64ImageRepresentation = Convert.ToBase64String(fileBytes);
                Root jsonResponse = new Root();
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.imgbb.com/1/upload?key=" + _key))
                    {
                        var multipartContent = new MultipartFormDataContent();
                        multipartContent.Add(new StringContent(base64ImageRepresentation), "image");
                        request.Content = multipartContent;

                        var response = await httpClient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            jsonResponse = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
                string path = jsonResponse?.data?.url;

                var memeToUplaod = new Meme() { UploaderName = name, PathToMeme = path, Id = requestas.Id };

                _memeRepository.UpdateImagePath(memeToUplaod);
            }
            catch(Exception e) {
                Console.WriteLine(e);
                _hackingService.UpsertHackEntry();
            }
        }

        public string GetClaimFromToken(string token) {
            string name = "";
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            name = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
            return name;
        }
        public bool ValidateToken(string token)
        {
            var symmetricKey = Convert.FromBase64String(_JWTkey);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            return true;
        }

        public Guid UpdateMeme(MemeUpdateRequest request)
        {
            Guid response = Guid.Empty;
            try
            {
                ValidateToken(request.Token);
                var meme = _memeRepository.GetMeme(request.Id);
                var newMeme = new Meme(){ 
                    Id = request.Id,
                    Caption = request.Caption,
                    Dislikes = meme.Dislikes,
                    Likes = meme.Likes,
                    MemeMessage = request.MemeMessage,
                    PathToMeme = request.PathToMeme,
                    uploadedAt = meme.uploadedAt,
                    UploaderName = meme.UploaderName};
                if (meme.UploaderName == GetClaimFromToken(request.Token))
                {
                    _memeRepository.UpdateMeme(newMeme);
                }
                else {
                    throw new Exception();
                }
                response = request.Id;
            }
            catch (Exception e) {
                _hackingService.UpsertHackEntry();
            }
            return response;
        }

        public void DeleteMeme(string token, Guid id)
        {
            try {
                ValidateToken(token);
                var meme = _memeRepository.GetMeme(id);
                if (meme.UploaderName == GetClaimFromToken(token))
                {
                    _memeRepository.DeleteMeme(id);
                }
                else {
                    throw new Exception();
                }
            }
            catch (Exception e) {
                _hackingService.UpsertHackEntry();
            }
        }
    }
    public class Image
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }

    public class Thumb
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }

    public class Medium
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url_viewer { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string size { get; set; }
        public string time { get; set; }
        public string expiration { get; set; }
        public Image image { get; set; }
        public Thumb thumb { get; set; }
        public Medium medium { get; set; }
        public string delete_url { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }



}
