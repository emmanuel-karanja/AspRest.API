using System.ComponentModel.DataAnnotations;

namespace AspRest.API.Models.UserAccounts
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token{get;set;}

        [Required]
        [MinLength(6)]
        public string Password{get;set;}

        [Required]
        [Compare("Password")]
        public string ConfirmPassword{get;set;}
    }
}