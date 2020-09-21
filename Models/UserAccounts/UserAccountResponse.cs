using System.ComponentModel.DataAnnotations;
using AspRest.API.Entities;
using System;

//DTO for the responses, we don't send back passwords
namespace AspRest.API.Models.UserAccounts
{
   public class UserAccountResponse
   {
       public long Id{get;set;}
       public string Title{get;set;}
       public string FirstName{get;set;}
       public string LastName{get;set;}
       public string Email{get;set;}
       public string Role{get;set;}
       public DateTime? CreatedOn{get;set;}
       public DateTime? UpdatedOn{get;set;}
       public DateTime? LastLoginOn {get;set;}
       public bool IsVerified{get;set;}

   }
}