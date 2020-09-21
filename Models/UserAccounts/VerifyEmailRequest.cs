using System.ComponentModel.DataAnnotations;

namespace AspRest.API.Models.UserAccounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token {get;set;}
    }
}