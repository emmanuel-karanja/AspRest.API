using System.ComponentModel.DataAnnotations;

namespace AspRest.API.Models.UserAccounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email{get;set;}
    }
}