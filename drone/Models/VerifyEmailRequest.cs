namespace WebApi.Models
{
    public class VerifyEmailRequest
    {
        public string Token { get; set; }
        public string Code { get; set; }
    }
}