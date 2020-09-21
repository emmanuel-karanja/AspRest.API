using System.ComponentModel.DataAnnotations;
using AspRest.API.Entities;
using System;
using System.Text.Json.Serialization;

namespace AspRest.API.Models.UserAccounts
{
   public class AuthenticateUserResponse
   {
       public long Id{get;set;}
       public string Title{get;set;}
       public string FirstName{get;set;}
       public string LastName{get;set;}
       public string Email{get;set;}
       public string Role{get;set;}
       public DateTime CreatedOn{get;set;}
       public DateTime? UpdatedOn {get;set;}
       public bool IsVerified {get;set;}
       public string JwtToken {get;set;}

       [JsonIgnore] //stored in the cookie
       public string RefreshToken{get;set;}

   }  
}