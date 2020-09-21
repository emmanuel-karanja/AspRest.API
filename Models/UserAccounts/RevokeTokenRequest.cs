using System.ComponentModel.DataAnnotations;

namespace AspRest.API.Models.UserAccounts
{
    public class RevokeTokenRequest
    {
        public string Token{get;set;}
    }
}