using System.ComponentModel.DataAnnotations;

namespace AspRest.API.Models.UserAccounts
{
    public class ValidateResetTokenRequest
    {
       [Required] 
       public string Token{get;set;}
    }
}